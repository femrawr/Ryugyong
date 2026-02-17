import fs from 'fs';
import path from 'path';

import common from './common.js';

import { UploadStatus } from '../enums.js';

const UploadServer = {
    CatBox: 0,
    ZeroXZero: 1
};

export default {
    _mode: UploadServer.CatBox,

    _endpoints: {
        [UploadServer.CatBox]: 'https://litterbox.catbox.moe/api.php',
        [UploadServer.ZeroXZero]: 'https://0x0.st'
    },

    async upload(file) {
        console.log(file); // asd
        const oldDir = path.dirname(file);
        const newName = path.join(oldDir, common.genStr(7));
        console.log(newName); // asd
        fs.renameSync(file, newName);

        const buff = fs.readFileSync(newName);
        const blob = new Blob([buff], { type: 'text/plain' });

        const form = new FormData();

        if (this._mode === UploadServer.CatBox) {
            form.append('reqtype', 'fileupload');
            form.append('time', '4');
            form.append('fileToUpload', blob, newName);
        } else {
            form.append('expires', '4'); 
            form.append('file', blob, newName);
        }

        console.log('uploading file...');

        const upload = await fetch(this._endpoints[this._mode], {
            method: 'POST',
            body: form
        });

        if (!upload.ok) {
            console.warn(`upload: upload was not ok (status ${upload.status})`);
            return UploadStatus.RequestNotOK;
        }

        const data = await upload.text();
        if (!data.startsWith('https://')) {
            console.warn('upload: failed to upload file -', data);
            return UploadStatus.UnexpectedData;
        }

        console.log('upload: file uploaded to -', data);
        return { uploaded: data, newFile: newName };
    }
};