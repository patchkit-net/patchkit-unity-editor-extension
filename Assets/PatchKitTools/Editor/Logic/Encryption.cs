using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PatchKit.Tools.Integration
{
    public static class Encryption
    {
        public static byte[] EncryptString(string plainText, string key)
        {            
            byte[] keyBytes = null;
            
            using (var sha256 = SHA256.Create())
            {
                keyBytes = sha256.ComputeHash(Encoding.ASCII.GetBytes(key));
            }
            
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            return Encrypt(plainTextBytes, keyBytes);
        }

        public static string DecryptString(byte[] encrypted, string key)
        {
            byte[] keyBytes = null;
            
            using (var sha256 = SHA256.Create())
            {
                keyBytes = sha256.ComputeHash(Encoding.ASCII.GetBytes(key));
            }

            return Encoding.UTF8.GetString(Decrypt(encrypted, keyBytes));
        }

        public static string EncryptedBytesToString(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        public static byte[] EncryptedStringToBytes(string str)
        {
            return Convert.FromBase64String(str);
        }
        
        public static byte[] Encrypt(byte[] plainText, byte[] key)
        {
            if (plainText == null)
            {
                throw new ArgumentNullException("plainText");
            }

            if (key == null)
            {
                throw new ArgumentNullException("key");
            }


            using (Aes aes = Aes.Create())
            {
                aes.Key = key;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                return aes.IV.ToList().Concat(PerformCryptography(encryptor, plainText).ToList()).ToArray();
            }
        }

        public static byte[] Decrypt(byte[] cipherText, byte[] key)
        {
            if (cipherText == null)
            {
                throw new ArgumentNullException("cipherText");
            }

            byte[] iv = cipherText.ToList().Take(16).ToArray();
            byte[] encryptedData = cipherText.ToList().Skip(16).ToArray();

            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                return PerformCryptography(decryptor, encryptedData);
            }
        }

        private static byte[] PerformCryptography(ICryptoTransform cryptoTransform, byte[] data)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(data, 0, data.Length);
                    cryptoStream.FlushFinalBlock();
                    return memoryStream.ToArray();
                }
            }
        }
    }
}