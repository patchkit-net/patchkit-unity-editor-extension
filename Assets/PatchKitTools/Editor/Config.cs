using UnityEngine;
using UnityEditor;

namespace PatchKit.Tools.Integration
{
    public class Config : ScriptableObject
    {
        #region SingletonImpl
        private const string Path = "Assets/PatchKitTools/Config.asset";
        public static Config CreateMyAsset()
        {
            Config asset = ScriptableObject.CreateInstance<Config>();

            AssetDatabase.CreateAsset(asset, Path);
            AssetDatabase.SaveAssets();

            return asset;
        }

        private static Config _instance = null;

        public static Config instance()
        {
            if (_instance != null)
            {
                return _instance;
            }

            var existingConfig = AssetDatabase.LoadAssetAtPath<Config>(Path);

            if (existingConfig == null)
            {
                existingConfig = CreateMyAsset();
            }

            _instance = existingConfig;

            return existingConfig;
        }
        #endregion

        public Api.ApiConnectionSettings connectionSettings;
    }
}