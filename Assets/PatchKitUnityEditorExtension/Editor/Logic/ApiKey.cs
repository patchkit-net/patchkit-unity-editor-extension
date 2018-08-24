using UnityEngine;
using System.Linq;

namespace PatchKit.Tools.Integration
{
    public class ApiKey
    {
        public readonly string Key;
        private const string PrefsKey = "patchkit-tools-integration-api-key";
        private const string EncryptionPassword = "42";
        
        public string Encrypt()
        {            
            byte[] encBytes = Encryption.EncryptString(Key, EncryptionPassword);
            
            return Encryption.EncryptedBytesToString(encBytes);
        }

        public static string Decrypt(string encrypted)
        {
            byte[] encBytes = Encryption.EncryptedStringToBytes(encrypted);

            return Encryption.DecryptString(encBytes, EncryptionPassword);
        }

        public ApiKey(string keyString)
        {
            Key = keyString;
        }

        public static ApiKey LoadCached()
        {
            if (PlayerPrefs.HasKey(PrefsKey))
            {
                string encrypted = PlayerPrefs.GetString(PrefsKey);
                
                return new ApiKey(Decrypt(encrypted));
            }

            return null;
        }

        public static void Cache(ApiKey key)
        {
            var enc = key.Encrypt();
            
            PlayerPrefs.SetString(PrefsKey, enc);
            PlayerPrefs.Save();
        }

        public static void ClearCached()
        {
            PlayerPrefs.DeleteKey(PrefsKey);
            PlayerPrefs.Save();
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Key) && Key.All(char.IsLetterOrDigit) && Key.Length ==32;
        }
    }
}