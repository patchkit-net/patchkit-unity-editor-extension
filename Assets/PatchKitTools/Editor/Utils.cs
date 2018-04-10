using System;
using System.Diagnostics;
using System.IO;
using Ionic.Zip;
using UnityEditor;
using UnityEngine;

namespace PatchKit.Tools.Integration
{
    public static class Utils
    {
        public static string ToPatchKitString(this BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.StandaloneWindows:
                    return "windows_x86";

                case BuildTarget.StandaloneWindows64:
                    return "windows_x86_64";

                case BuildTarget.StandaloneLinux:
                    return "linux_x86";

                case BuildTarget.StandaloneLinux64:
                case BuildTarget.StandaloneLinuxUniversal:
                    return "linux_x86_64";

                case BuildTarget.StandaloneOSXIntel:
                    return "mac_x86";

                case BuildTarget.StandaloneOSXIntel64:
                case BuildTarget.StandaloneOSX:
                    return "mac_x86_64";

                default:
                    throw new ArgumentException("Unsupported build target");
            }
        }

        public static string ToolsExtractLocation()
        {
            const string postfix = "pk-tools";
            string basePath = null;
            
            var config = Config.Instance();

            if (string.IsNullOrEmpty(config.ToolsExtractLocation))
            {
                basePath = Application.persistentDataPath;
            }
            else
            {
                basePath = config.ToolsExtractLocation;
            }

            return Path.Combine(basePath, postfix);
        }

        public static string PlatformToToolsSource(RuntimePlatform platform)
        {
            var config = Config.Instance();
            string basePath = null;
            
            switch (platform)
            {
                case RuntimePlatform.LinuxEditor:
                    basePath = config.ToolsLocations.LinuxX86Tools;
                    break;
                
                case RuntimePlatform.WindowsEditor:
                    basePath = config.ToolsLocations.WindowsTools;
                    break;
                    
                case RuntimePlatform.OSXEditor:
                    basePath = config.ToolsLocations.OsxTools;
                    break;
                    
                default:
                    throw new ArgumentException("Unsupported platform");
            }

            return Path.Combine(Application.dataPath, basePath);
        }

        public static void ExtractTools(string sourceZip, string target)
        {
            using (var zip = ZipFile.Read(sourceZip))
            {                
                zip.ExtractAll(target);
            }
        }

        public static void AddExecutablePermissions(string target, bool recursive)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "chmod",
                Arguments = "+x " + target + (recursive ? " -R" : ""),
            };

            using (var process = Process.Start(processInfo))
            {
                process.WaitForExit();
            }
        }
    }
}