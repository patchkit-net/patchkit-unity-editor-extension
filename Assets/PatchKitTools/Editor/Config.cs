using System;
using UnityEngine;
using UnityEditor;

namespace PatchKit.Tools.Integration
{
    public class Config : ScriptableObject
    {
        #region SingletonImpl
        private const string Path = "Assets/PatchKitTools/Config.asset";

        private static Config CreateMyAsset()
        {
            Config asset = ScriptableObject.CreateInstance<Config>();

            AssetDatabase.CreateAsset(asset, Path);
            AssetDatabase.SaveAssets();

            return asset;
        }

        private static Config _instance = null;

        public static Config Instance()
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

        [Serializable]
        public struct ToolsConfig
        {
            public string WindowsTools;
            public string OsxTools;
            public string LinuxX86Tools;
            public string LinuxX8664Tools;
        }
        
        public Api.ApiConnectionSettings ConnectionSettings;

        public string LocalCachePath = ".appcache";

        [Header("Tools options")]
        public bool CloseConsoleWindowAutomatically = false;
        public bool ForceOverrideDraftVersion = true;
        public bool AutoPublishAfterUpload = false;

        public ToolsConfig ToolsLocations;
        
        public string ToolsExtractLocation;
    }
}