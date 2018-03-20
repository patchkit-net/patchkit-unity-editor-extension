using UnityEditor;
using UnityEngine;
using System;
using System.IO;

namespace PatchKit.Tools.Integration
{
    public class ApiKey
    {
        public readonly string Key;
        private const string Filename = "api-key";
        
        public ApiKey(string keyString)
        {
            Key = keyString;
        }

        public static ApiKey LoadCached()
        {
            try
            {
                string apiKeyPath = ApiKeyFilePath();
                Debug.Log("Loading cached api key from " + apiKeyPath);

                if (!File.Exists(apiKeyPath))
                {
                    Debug.Log("Cached api key doesn't exist.");
                    return null;
                }

                var cachedKey = new ApiKey(File.ReadAllText(apiKeyPath));
                return cachedKey;
            }
            catch(Exception e)
            {
                Debug.LogError(e);
                return null;
            }
        }

        public static void Cache(ApiKey key)
        {
            try
            {
                string apiKeyPath = ApiKeyFilePath();
                string apiKeyParentDir = Directory.GetParent(apiKeyPath).ToString();

                if (!Directory.Exists(apiKeyParentDir))
                {
                    Directory.CreateDirectory(apiKeyParentDir);
                }

                File.WriteAllText(apiKeyPath, key.Key);
            }
            catch(Exception e)
            {
                Debug.LogError(e);
            }
        }

        public static void ClearCached()
        {
            string apiKeyPath = ApiKeyFilePath();
            if (File.Exists(apiKeyPath))
            {
                File.Delete(apiKeyPath);
            }
        }

        public bool IsValid()
        {
            return !Key.Contains(".");
        }

        public static string ApiKeyFilePath()
        {
            var sep = Path.DirectorySeparatorChar;

            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            var projectDataPath = Path.Combine(appDataPath, "PatchKit" + sep + "Tools" + sep + UnityEngine.Application.productName);
            var apiFilePath = Path.Combine(projectDataPath, Filename);
            return apiFilePath;
        }
    }
}