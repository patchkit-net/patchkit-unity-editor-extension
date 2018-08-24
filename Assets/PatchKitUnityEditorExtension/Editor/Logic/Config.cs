using System;
using UnityEngine;
using UnityEditor;

namespace PatchKit.Tools.Integration
{
    public class Config : ScriptableObject
    {
        #region SingletonImpl
         [SerializeField] private const string Path = "Assets/PatchKitUnityEditorExtension/Config.asset";

        private string GetPathToThis()
        {
            return AssetDatabase.GetAssetPath(this);
        }

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
            WindowsTools = "PatchKitUnityEditorExtension/ToolsPackages/patchkit-tools-win32.zip",
            OsxTools = "PatchKitUnityEditorExtension/ToolsPackages/patchkit-tools-osx.zip",
            LinuxTools = "PatchKitUnityEditorExtension/ToolsPackages/patchkit-tools-linux-x86.zip",
        };

        [SerializeField]
        public AppCache Cache = new AppCache();
        
        public const string ToolsExtractLocation = "Temp/";
    }
}