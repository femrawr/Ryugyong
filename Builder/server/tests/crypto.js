import crypto from '../utils/crypto.js';

const encryptionKey = '';
crypto.setCryptoKey(encryptionKey);

const unencryptedString = 'https://https://files.catbox.moe/ergbuh23786sdafz12';
const encryptedString = crypto.encryptString(unencryptedString);

console.log('encryption key -', encryptionKey);
console.log('unencrypted string -', unencryptedString);
console.log('encrypted string -', encryptedString);