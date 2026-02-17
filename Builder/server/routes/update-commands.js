import express from 'express';
import fs from 'fs';
import path from 'path';

const router = express.Router();

router.post('/update-commands', async (req, res) => {
    console.log('updating commands...');

    const body = req.body;

    

    console.log('updated commands');
    return res.sendStatus(200);
});

export default router;