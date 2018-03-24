using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using App = PatchKit.Api.Models.Main.App;

namespace PatchKit.Tools.Integration
{
    public static class Tools
    {
        private const string PATCHKIT_TOOLS_MAIN = "Tools";
        private const string PATCHKIT_WIN32_SCRIPT = "win32/patchkit-tools.bat";

        public static void MakeVersion(string apiKey, string appSecret, string label, string changelog, string buildDir)
        {
            Execute("make-version", new string[] {
                "--secret", appSecret,
                "--apikey", apiKey,
                "--label", label,
                "--changelog", string.IsNullOrEmpty(changelog) ? "\"\"" : changelog.Replace("\n", "\\n"),
                "--files", buildDir,
                "--host", Config.instance().connectionSettings.MainServer.Host
                }, 
                true);
        }

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
                case BuildTarget.StandaloneOSXUniversal:
                    return "mac_x86_64";

                default:
                    throw new ArgumentException("Unsupported build target");
            }
        }

        public static void MakeVersionHeadless(string apiKey, string appSecret, string label, string changelog, string buildDir, Action onFinish)
        {
            var thread = new Thread(
                () => {
                    MakeVersion(apiKey, appSecret, label, changelog, buildDir);
                    onFinish();
                }
            );

            thread.Start();
        }

        public static void Execute(string tool, string[] toolArguments, bool openConsole = false)
        {
            var path = Path.GetFullPath(Path.Combine("Assets/PatchKitTools/Tools", "win32/patchkit-tools.bat"));
            var processArguments = "/K " + path + " " + tool;

            if (toolArguments != null)
            {
                processArguments += " ";
                foreach (string arg in toolArguments)
                {
                    if (arg.Contains(' '))
                    {
                        processArguments += "\"" + arg + "\" ";
                    }
                    else
                    {
                        processArguments += arg + " ";
                    }
                }
            }

            var processInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = processArguments,
                CreateNoWindow = !openConsole,
                UseShellExecute = false,
                RedirectStandardOutput = !openConsole
            };

            UnityEngine.Debug.Log("Executing: " + processArguments);

            using (var process = Process.Start(processInfo))
            {
                process.WaitForExit();

                if (!openConsole)
                {
                    var output = process.StandardOutput.ReadToEnd();
                    UnityEngine.Debug.Log("Output: " + output);
                }
            }
        }
    }
}