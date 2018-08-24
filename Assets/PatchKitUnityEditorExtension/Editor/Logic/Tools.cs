using System;
using System.IO;
using System.Linq;
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
            if(platform != RuntimePlatform.WindowsEditor)
            {
                Utils.AddExecutablePermissions(_toolsLocation, true);
            }
            
        }
        
        public void MakeVersion(
            string apiKey, 
            string appSecret, 
            string label, 
            string changelog, 
            string buildDir, 
            bool autoPublishAfterUpload,
            bool forceOverrideDraftVersion)
        {
            var arguments = new List<string> {
                "--secret", appSecret,
                "-a", apiKey,
                "--label", label,
                "--changelog", string.IsNullOrEmpty(changelog) ? "\"\"" : changelog.Replace("\n", "\\n"),
                "--files", buildDir,
                "--host", Config.Instance().ConnectionSettings.MainServer.Host,
                "--https", "true"
            };

            if (forceOverrideDraftVersion)
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
            string lista = "";
            for (int i = 0; i < arguments.Count; i++) lista += arguments[i] + " ";
            UnityEngine.Debug.Log("make-version " + lista);
        }

        public void Execute(string tool, string[] toolArguments)
        {
            switch (_platform)
            {
                case RuntimePlatform.WindowsEditor:
                    BuildAndPublish.messagesView.AddMessage("Executing for windows...", UnityEditor.MessageType.Info);
                    UnityEngine.Debug.Log("Executing for windows...");
                    ExecuteWindows(tool, toolArguments);
                    break;
                
                case RuntimePlatform.LinuxEditor:
                    BuildAndPublish.messagesView.AddMessage("Executing for linux...", UnityEditor.MessageType.Info);
                    UnityEngine.Debug.Log("Executing for linux...");
                    ExecuteLinux(tool, toolArguments);
                    break;
                
                case RuntimePlatform.OSXEditor:
                    throw new NotImplementedException();
                
                default:
                    BuildAndPublish.messagesView.AddMessage("Unsupported platform", UnityEditor.MessageType.Info);
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