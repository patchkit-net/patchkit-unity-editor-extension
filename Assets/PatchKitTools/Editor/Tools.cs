using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;

using App = PatchKit.Api.Models.Main.App;

namespace PatchKit.Tools.Integration
{
    public class ToolsWrapper
    {
        private const string PATCHKIT_TOOLS_MAIN = "Tools";
        private const string PATCHKIT_WIN32_SCRIPT = "win32/patchkit-tools.bat";

        public static void MakeVersion(string apiKey, string appSecret, string label, string changelog, string buildDir)
        {
            Execute("make-version", new string[] {
                "--secret", appSecret,
                "--apikey", apiKey,
                "--label", label,
                "--changelog", changelog.Replace("\n", "\\n"),
                "--files", buildDir
                }, 
                true);
        }

        public static void MakeVersionHeadless(string apiKey, string appSecret, string label, string changelog, string buildDir)
        {
            var thread = new Thread(
                () => {
                    MakeVersion(apiKey, appSecret, label, changelog, buildDir);
                }
            );

            thread.Start();
        }

        public static void Execute(string tool, string[] toolArguments, bool openConsole = false)
        {
            var path = Path.GetFullPath(Path.Combine("Assets/PatchKitTools/Tools", "win32/patchkit-tools.bat"));
            var processArguments = "/C " + path + " " + tool;

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