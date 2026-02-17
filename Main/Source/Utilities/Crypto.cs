using System.Security.Cryptography;
using System.Text;

namespace Main.Source.Utilities
{
    public class Crypto
    {
        private const int SaltLen = 20;
        private const int NonceLen = 12;
        private const int TagLen = 16;

        public static string Encrypt(string data)
        {
            byte[] saltBuffer = new byte[SaltLen];
            RandomNumberGenerator.Fill(saltBuffer);

            byte[] key = GenKey(saltBuffer);
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);

            byte[] nonce = new byte[NonceLen];
            RandomNumberGenerator.Fill(nonce);

            byte[] cipherBytes = new byte[dataBytes.Length];
            byte[] tag = new byte[TagLen];

            using var chacha = new ChaCha20Poly1305(key);
            chacha.Encrypt(nonce, dataBytes, cipherBytes, tag);

            byte[] joined = new byte[SaltLen + NonceLen + cipherBytes.Length + TagLen];
            Array.Copy(saltBuffer, 0, joined, 0, SaltLen);
            Array.Copy(nonce, 0, joined, SaltLen, NonceLen);
            Array.Copy(cipherBytes, 0, joined, SaltLen + NonceLen, cipherBytes.Length);
            Array.Copy(tag, 0, joined, SaltLen + NonceLen + cipherBytes.Length, TagLen);

            return Convert.ToBase64String(joined);
        }

        public static string Decrypt(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return "0";
            }

            byte[] joined = Convert.FromBase64String(data);

            byte[] salt = new byte[SaltLen];
            byte[] nonce = new byte[NonceLen];
            byte[] tag = new byte[TagLen];
            byte[] cipherBytes = new byte[joined.Length - SaltLen - NonceLen - TagLen];

            Array.Copy(joined, 0, salt, 0, SaltLen);
            Array.Copy(joined, SaltLen, nonce, 0, NonceLen);
            Array.Copy(joined, SaltLen + NonceLen, cipherBytes, 0, cipherBytes.Length);
            Array.Copy(joined, SaltLen + NonceLen + cipherBytes.Length, tag, 0, TagLen);

            byte[] key = GenKey(salt);
            byte[] plainBytes = new byte[cipherBytes.Length];

            using var chacha = new ChaCha20Poly1305(key);
            chacha.Decrypt(nonce, cipherBytes, tag, plainBytes);

            return Encoding.UTF8.GetString(plainBytes);
        }

        private static byte[] GenKey(byte[] salt)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(ConfigMisc.CRYPTO_KEY);

            byte[] joined = new byte[keyBytes.Length + salt.Length];
            Array.Copy(keyBytes, 0, joined, 0, keyBytes.Length);
            Array.Copy(salt, 0, joined, keyBytes.Length, salt.Length);

            return SHA256.HashData(joined);
        }
    }
}
