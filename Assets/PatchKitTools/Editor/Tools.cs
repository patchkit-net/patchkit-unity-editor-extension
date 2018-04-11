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
    public class Tools : IDisposable
    {
        private readonly string _toolsLocation;
        private readonly RuntimePlatform _platform;
        
        public Tools(RuntimePlatform platform)
            : this(Utils.PlatformToToolsSource(platform), Utils.ToolsExtractLocation(), platform)
        {
        }

        public Tools(string sourceZip, string targetLocation, RuntimePlatform platform)
        {
            _toolsLocation = targetLocation;
            _platform = platform;

            if (Directory.Exists(_toolsLocation))
            {
                Clear();
            }
            
            Utils.ExtractTools(sourceZip, _toolsLocation);
        }
        
        public void MakeVersion(string apiKey, string appSecret, string label, string changelog, string buildDir)
        {
            var arguments = new List<string> {
                "--secret", appSecret,
                "--apikey", apiKey,
                "--label", label,
                "--changelog", string.IsNullOrEmpty(changelog) ? "\"\"" : changelog.Replace("\n", "\\n"),
                "--files", buildDir,
                "--host", Config.Instance().ConnectionSettings.MainServer.Host
            };

            if (Config.Instance().ForceOverrideDraftVersion)
            {
                arguments.Add("-x");
                arguments.Add("true");
            }

            if (Config.Instance().AutoPublishAfterUpload)
            {
                arguments.Add("-p");
                arguments.Add("true");
            }

            Execute("make-version", arguments.ToArray(), true);
        }

        public void Execute(string tool, string[] toolArguments, bool openConsole = false)
        {
            if (_platform == RuntimePlatform.WindowsEditor)
            {
                ExecuteWindows(tool, toolArguments, openConsole);
            }
            else
            {
                UnityEngine.Debug.Log("Executing non windows...");
                var path = Path.GetFullPath(Path.Combine(_toolsLocation, "patchkit-tools"));
                
                if (!File.Exists(path))
                {
                    throw new ArgumentException("Executable does not exist");
                }
            
                Utils.AddExecutablePermissions(_toolsLocation, true);

                string programPath = "gnome-terminal";
                string processArguments = "-x bash -c '" + path + " " + tool;

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

                processArguments += "'";

                UnityEngine.Debug.Log(string.Format("Launching {0} with arguments {1}", programPath, processArguments));
                
                var processInfo = new ProcessStartInfo
                {
                    FileName = programPath,
                    Arguments = processArguments,
                    CreateNoWindow = !openConsole,
                    UseShellExecute = false,
                    RedirectStandardOutput = !openConsole,
                    RedirectStandardError = !openConsole
                };

                using (var process = Process.Start(processInfo))
                {
                    if (process == null)
                    {
                        UnityEngine.Debug.LogError("Process is null");
                        return;
                    }
                    
                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        try
                        {
                            var err = process.StandardError.ReadToEnd();
                            UnityEngine.Debug.LogError("Error: " + err);
                        }
                        catch
                        {
                            // ignored
                        }

                        throw new Exception(string.Format("Non zero ({0}) return code.", process.ExitCode));
                    }

                    if (!openConsole)
                    {
                        var output = process.StandardOutput.ReadToEnd();
                        UnityEngine.Debug.Log("Output: " + output);
                    }
                }
            }
        }

        private void ExecuteWindows(string tool, string[] toolArguments, bool openConsole = false)
        {
            var path = Path.GetFullPath(Path.Combine(_toolsLocation, "patchkit-tools.bat"));

            string prefix = Config.Instance().CloseConsoleWindowAutomatically ? "/C" : "/K";

            var processArguments = prefix + path + " " + tool;

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

        private void Clear()
        {
            Directory.Delete(_toolsLocation, true);
        }

        public void Dispose()
        {
//            Clear();
        }
    }
}