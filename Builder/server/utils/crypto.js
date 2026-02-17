import fs from 'fs';
import path from 'path';
import crypto from 'crypto';

import { ChaCha20Poly1305 } from '@stablelib/chacha20poly1305';
import { hash as sha256 } from '@stablelib/sha256';

import common from './common.js';
import config from '../config.js';

export default {
    _saltLen: 20,
    _nonceLen: 12,
    _tagLen: 16,

    _chunk: 1 * 1024 * 1024,

    _key: '',

    setCryptoKey(key) {
        if (this._key !== '') {
            console.warn('setCryptoKey: attempted to call twice');
            return;
        }

        this._key = key;
    },

    encryptString(string) {
        if (!this._key) {
            console.error('encryptString: key not set');
            return '';
        }

        const { key, salt } = this._genKey();

        const nonce = crypto.randomBytes(this._nonceLen);
        const data = Buffer.from(string, 'utf-8');

        const chacha = new ChaCha20Poly1305(key);
        const sealed = chacha.seal(nonce, data);

        const joined = Buffer.concat([salt, nonce, sealed])
        return joined.toString('base64');
    },

    async encryptFile(file) {
        if (!this._key) {
            console.error('encryptFile: key not set');
            return '';
        }

        const oldExt = path.extname(file);
        const oldDir = path.dirname(file);
        const newFile = path.join(oldDir, common.genStr(7) + oldExt);

        const { key, salt } = this._genKey();

        const read = fs.createReadStream(file, { highWaterMark: this._chunk });
        const write = fs.createWriteStream(newFile);

        write.write(salt);

        const chacha = new ChaCha20Poly1305(key);

        const __stats = fs.statSync(file);
        const __size = __stats.size;
        const __chunks = Math.ceil(__size / this._chunk);
        let __chunksDone = 0;

        return new Promise((resolve, reject) => {
            read.on('data', (chunk) => {
                __chunksDone++;

                if (config.debug) {
                    console.log(`encryptFile: encrypting "${file}" ${__chunksDone}/${__chunks}`);
                }

                const nonce = crypto.randomBytes(this._nonceLen);
                const sealed = chacha.seal(nonce, chunk);

                write.write(nonce);
                write.write(sealed);
            });

            read.on('end', () => {
                write.end();
            });

            write.on('finish', () => {
                resolve(newFile);
            });

            read.on('error', reject);
            write.on('error', reject);
        });
    },

    _genKey() {
        const saltBytes = crypto.randomBytes(this._saltLen);
        const keyBytes = Buffer.from(this._key, 'utf8');

        const joined = Buffer.concat([keyBytes, saltBytes]);

        return {
            key: sha256(joined),
            salt: saltBytes
        };
    }
};