using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;

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

            Utils.AddExecutablePermissions(_toolsLocation, true);
        }
        
        public void MakeVersion(string apiKey, string appSecret, string label, string changelog, string buildDir, bool autoPublishAfterUpload)
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

            if (autoPublishAfterUpload)
            {
                arguments.Add("-p");
                arguments.Add("true");
            }

            Execute("make-version", arguments.ToArray());
        }

        public void Execute(string tool, string[] toolArguments)
        {
            switch (_platform)
            {
                case RuntimePlatform.WindowsEditor:
                    UnityEngine.Debug.Log("Executing for windows...");
                    ExecuteWindows(tool, toolArguments);
                    break;
                
                case RuntimePlatform.LinuxEditor:
                    UnityEngine.Debug.Log("Executing for linux...");
                    ExecuteLinux(tool, toolArguments);
                    break;
                
                case RuntimePlatform.OSXEditor:
                    throw new NotImplementedException();
                
                default:
                    throw new ArgumentException("Unsupported platform");
            }
        }

        private void ExecuteLinux(string tool, string[] toolArguments)
        {
            var path = Path.GetFullPath(Path.Combine(_toolsLocation, "patchkit-tools"));
                
            if (!File.Exists(path))
            {
                throw new ArgumentException("Executable does not exist");
            }
        
            string processArguments = "bash -c '" + path + " " + tool;

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
            
            var console = new TerminalWrapper(_platform);
            console.Launch(processArguments);
        }

        private void ExecuteWindows(string tool, string[] toolArguments, bool openConsole = false)
        {
            var path = Path.GetFullPath(Path.Combine(_toolsLocation, "patchkit-tools.bat"));

            var processArguments = path + " " + tool;

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
            
            var console = new TerminalWrapper(_platform);
            console.Launch(processArguments);
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