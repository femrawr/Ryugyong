import fs from 'fs';
import path from 'path';

import { ChaCha20Poly1305 } from '@stablelib/chacha20poly1305';
import { hash as sha256 } from '@stablelib/sha256';

import common from './common.js';
import config from '../config.js';

export default {
    _saltLen: 100,
    _nonceLen: 12,
    _tagLen: 16,

    _chunk: 1 * 1024 * 1024,

    encryptString(string, key) {
        const { theKey, saltBytes } = this._genKey(key);

        const dataBytes = Buffer.from(string, 'utf-8');

        const nonce = common.genStr(this._nonceLen);
        const nonceBytes = Buffer.from(nonce, 'utf-8');

        const chacha = new ChaCha20Poly1305(theKey);
        const sealed = chacha.seal(nonceBytes, dataBytes);

        const cipher = sealed.slice(0, sealed.length - this._tagLen);
        const tag = sealed.slice(sealed.length - this._tagLen);

        const joined = Buffer.concat([saltBytes, nonceBytes, cipher, tag])
        return joined.toString('base64');
    },

    async encryptFile(file, key) {
        const oldDir = path.dirname(file);
        const newFile = path.join(oldDir, common.genStr(7));

        const { theKey, saltBytes } = this._genKey(key);

        const read = fs.createReadStream(file, { highWaterMark: this._chunk });
        const write = fs.createWriteStream(newFile);

        write.write(saltBytes);

        const chacha = new ChaCha20Poly1305(theKey);

        const __stats = fs.statSync(file);
        const __size = __stats.size;
        const __chunks = Math.ceil(__size / this._chunk);
        let __chunksDone = 0;

        return new Promise((_, rej) => {
            read.on('data', (chunk) => {
                __chunksDone++;

                if (config.debug) {
                    console.log(`encrypting file "${file}" ${__chunksDone}/${__chunks}`);
                }

                const nonce = common.genStr(this._nonceLen);
                const nonceBytes = Buffer.from(nonce, 'utf-8');

                const sealed = chacha.seal(nonceBytes, chunk);

                const cipher = sealed.slice(0, sealed.length - this._tagLen);
                const tag = sealed.slice(sealed.length - this._tagLen);

                write.write(nonceBytes);
                write.write(cipher);
                write.write(tag);
            });

            read.on('end', () => {
                write.end();
            });

            write.on('finish', () => {
                if (!config.debug) {
                    return;
                }

                console.log(`"${file}" has been encrypted to "${newFile}"`);
            });

            read.on('error', rej);
            write.on('error', rej);
        });
    },

    _genKey(key) {
        const salt = common.genStr(this._saltLen);
        const saltBytes = Buffer.from(salt,'utf-8');

        const keyBytes = Buffer.from(key, 'utf8');

        const joined = Buffer.concat([keyBytes, saltBytes]);

        return {
            theKey: sha256(joined),
            saltBytes: saltBytes
        };
    }
};