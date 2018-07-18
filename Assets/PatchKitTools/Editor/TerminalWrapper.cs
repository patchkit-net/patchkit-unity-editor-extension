using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityEngine;

namespace PatchKit.Tools.Integration
{
    public class TerminalWrapper
    {
        public enum Type
        {
            Unknown,
            GnomeTerminal,
            XTerm,
            Konsole,
            Cmd
        }

        private readonly RuntimePlatform _platform;

        private readonly Type _type;
        
        public TerminalWrapper(RuntimePlatform platform)
        {
            _platform = platform;
            _type = FindEmulator(platform);
        }

        public static Type FindEmulator(RuntimePlatform platform)
        {
            if (platform == RuntimePlatform.WindowsEditor)
            {
                return Type.Cmd;
            }
            else if (platform == RuntimePlatform.LinuxEditor)
            {
                if (File.Exists("/usr/bin/gnome-terminal"))
                {
                    return Type.GnomeTerminal;
                }
                
                if (File.Exists("/usr/bin/xterm"))
                {
                    return Type.XTerm;
                }
                
                if (File.Exists("/usr/bin/konsole"))
                {
                    return Type.Konsole;
                }
            }

            return Type.Unknown;
        }

        public static string ExecutableByType(Type type)
        {
            switch (type)
            {
                case Type.Cmd:
                    return "cmd.exe";
                
                case Type.GnomeTerminal:
                    return "gnome-terminal";
                
                case Type.XTerm:
                    return "xterm";
                
                case Type.Konsole:
                    return "konsole";
                
                default:
                    BuildAndPublish.messagesView.AddMessage("Unknown or unsupported terminal.", UnityEditor.MessageType.Warning);
                    throw new ArgumentException("Unknown or unsupported terminal.");
            }
        }

        public static string FindEmulatorExecutable(RuntimePlatform platform)
        {
            return ExecutableByType(FindEmulator(platform));
        }

        public void Launch(string argumentsString)
        {
            string args = "";
            string filename = ExecutableByType(_type);
            
            if (_type == Type.Cmd)
            {
                args = "/K " + argumentsString; // /C wyłącza okno, /K zostawia okno
            }
            else if (_type == Type.GnomeTerminal)
            {
                args = "-x " + argumentsString;
            }
            else if (_type == Type.XTerm || _type == Type.Konsole)
            {
                args = "-e " + argumentsString;
            }
            
            var processInfo = new ProcessStartInfo
            {
                FileName = filename,
                Arguments = args
            };

            UnityEngine.Debug.Log(string.Format("Launching {0} {1}", filename, args));
            BuildAndPublish.messagesView.AddMessage(string.Format("Launching {0} {1}", filename, args), UnityEditor.MessageType.Info);
            var process = Process.Start(processInfo);
            
            if (process.HasExited && process.ExitCode != 0)
            {
                BuildAndPublish.messagesView.AddMessage(string.Format("Non zero ({0}) return code.", process.ExitCode), UnityEditor.MessageType.Warning);
                throw new Exception(string.Format("Non zero ({0}) return code.", process.ExitCode));
            }

            new Thread(() => {
                process.WaitForExit();
            });
        }
        
    }
}