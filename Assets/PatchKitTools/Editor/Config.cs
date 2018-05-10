using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using App = PatchKit.Api.Models.Main.App;

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
            public string LinuxTools;
            public string LinuxX8664Tools;
        }
        
        public Api.ApiConnectionSettings ConnectionSettings;

        public static readonly ToolsConfig ToolsLocations = new ToolsConfig{
            WindowsTools = "PatchKitTools/ToolsPackages/patchkit-tools-win32.zip",
            OsxTools = "PatchKitTools/ToolsPackages/patchkit-tools-osx.zip",
            LinuxTools = "PatchKitTools/ToolsPackages/patchkit-tools-linux-x86.zip",
        };

        [SerializeField]
        public AppCache Cache = new AppCache();
        
        public const string ToolsExtractLocation = "Temp/";
    }
}