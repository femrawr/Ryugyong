import fs from 'fs';
import path from 'path';

import common from './common.js';

import { UploadStatus } from '../enums.js';

export default {
    api: 'https://catbox.moe/user/api.php',

    async upload(file) {
        const oldDir = path.dirname(file);
        const newName = path.join(oldDir, common.genStr(7));
        fs.renameSync(file, newName);

        const buff = fs.readFileSync(newName);
        const blob = new Blob([buff], { type: 'text/plain' });

        const form = new FormData();
        form.append('reqtype', 'fileupload');
        form.append('fileToUpload', blob, newName);

        console.log('uploading file...');

        const upload = await fetch(this.api, {
            method: 'POST',
            body: form
        });

        if (!upload.ok) {
            console.error('upload: upload was not ok');
            return UploadStatus.RequestNotOK;
        }

        const data = await upload.text();
        if (!data.startsWith('https://')) {
            console.error('upload: failed to upload file -', data);
            return UploadStatus.UnexpectedData;
        }

        console.log('upload: file uploaded to -', data);
        return { uploaded: data, newFile: newName };
    }
};