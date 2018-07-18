using System;
using System.Diagnostics;
using System.IO;
using Ionic.Zip;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

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
                    return "linux_x86_64";

#if UNITY_2017_1_OR_NEWER
                case BuildTarget.StandaloneOSXIntel:
                    //case BuildTarget.StandaloneOSX:  //tak bylo
#else
                case BuildTarget.StandaloneOSXIntel64:
#endif
                    return "mac_x86_64";

                default:
                    throw new ArgumentException("Unsupported build target");
            }
        }

        public static string ToolsExtractLocation()
        {
            const string postfix = "pk-tools";
           // string basePath = null;
            
            Assert.IsFalse(string.IsNullOrEmpty(Config.ToolsExtractLocation));

            return Path.Combine(Config.ToolsExtractLocation, postfix);
        }

        public static string PlatformToToolsSource(RuntimePlatform platform)
        {
            string basePath = null;
            
            switch (platform)
            {
                case RuntimePlatform.LinuxEditor:
                    basePath = Config.ToolsLocations.LinuxTools;
                    break;
                
                case RuntimePlatform.WindowsEditor:
                    basePath = Config.ToolsLocations.WindowsTools;
                    break;
                    
                case RuntimePlatform.OSXEditor:
                    basePath = Config.ToolsLocations.OsxTools;
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