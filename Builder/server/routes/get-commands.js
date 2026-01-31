import express from 'express';
import path from 'path';
import fs from 'fs';

import format from '../utils/format.js';

const router = express.Router();

router.get('/get-commands', async (req, res) => {
    const commandsDir = path.resolve('../Main/Source/Bot/Commands');
    if (!fs.existsSync(commandsDir)) {
        return res
            .status(500)
            .send('failed to find commands dir');
    }

    const files = fs.readdirSync(commandsDir);
    const commands = [];

    for (const file of files) {
        const thePath = path.join(commandsDir, file);
        const content = fs.readFileSync(thePath, 'utf-8');

        const info = content.match(/public\s+override\s+string\s+Info\s*=>\s*"([^"]+)"/);
        if (!info) {
            continue;
        }

        commands.push({
            name: format.fromPascal(file.replaceAll('.cs', '')),
            info: info[1]
        });
    }

    return res.json(commands);
});

export default router;