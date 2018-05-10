using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Linq;

namespace PatchKit.Tools.Integration
{
    public class ApiKey
    {
        public readonly string Key;
        private const string PrefsKey = "api-key";
        
        public ApiKey(string keyString)
        {
            Key = keyString;
        }

        public static ApiKey LoadCached()
        {
            if (PlayerPrefs.HasKey(PrefsKey))
            {
                return new ApiKey(PlayerPrefs.GetString(PrefsKey));
            }

            return null;
        }

        public static void Cache(ApiKey key)
        {
            PlayerPrefs.SetString(PrefsKey, key.Key);
            PlayerPrefs.Save();
        }

        public static void ClearCached()
        {
            PlayerPrefs.DeleteKey(PrefsKey);
            PlayerPrefs.Save();
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Key) && Key.All(char.IsLetterOrDigit);
        }
    }
}