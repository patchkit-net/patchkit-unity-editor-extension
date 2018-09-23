using System;
using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;
using PatchKit.UnityEditorExtension.Core;
using PatchKit.UnityEditorExtension.Menus;
using UnityEngine.Assertions;
using Environment = PatchKit.UnityEditorExtension.Core.Environment;

namespace PatchKit.UnityEditorExtension.Tools
{
public class Terminal
{
    private readonly string _startCommand;

    public Terminal(string startCommand)
    {
        _startCommand = startCommand;

        ProcessStartInfo processStartInfo = GetInfo();

        UnityEngine.Debug.Log(
            string.Format(
                "Launching {0} {1}",
                processStartInfo.FileName,
                processStartInfo.Arguments));

        BuildAndPublish.messagesView.AddMessage(
            string.Format(
                "Launching {0} {1}",
                processStartInfo.FileName,
                processStartInfo.Arguments),
            UnityEditor.MessageType.Info);

        Process process = Process.Start(processStartInfo);
        Assert.IsNotNull(process);
    }

    [NotNull]
    private ProcessStartInfo GetInfo()
    {
        switch (Environment.Platform)
        {
            case EnvironmentPlatform.Windows:
                return GetWindowsInfo();
            case EnvironmentPlatform.Linux:
                return GetLinuxInfo();
            case EnvironmentPlatform.Mac:
                return GetMacInfo();
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    [NotNull]
    private ProcessStartInfo GetWindowsInfo()
    {
        // /K - Carries out the command specified by String and continues.
        return new ProcessStartInfo("cmd.exe", "/K " + _startCommand);
    }

    [NotNull]
    private ProcessStartInfo GetLinuxInfo()
    {
        if (File.Exists("/usr/bin/gnome-terminal"))
        {
            return GetGnomeTerminalInfo();
        }

        if (File.Exists("/usr/bin/xterm"))
        {
            return GetXTermTerminalInfo();
        }

        if (File.Exists("/usr/bin/konsole"))
        {
            return GetKonsoleTerminalInfo();
        }

        BuildAndPublish.messagesView.AddMessage(
            "Unknown or unsupported terminal.",
            UnityEditor.MessageType.Warning);

        throw new InvalidOperationException();
    }

    [NotNull]
    private ProcessStartInfo GetGnomeTerminalInfo()
    {
        return new ProcessStartInfo("gnome-terminal", "-x " + _startCommand);
    }

    [NotNull]
    private ProcessStartInfo GetXTermTerminalInfo()
    {
        return new ProcessStartInfo("xterm", "-e " + _startCommand);
    }

    [NotNull]
    private ProcessStartInfo GetKonsoleTerminalInfo()
    {
        return new ProcessStartInfo("konsole", "-e " + _startCommand);
    }

    [NotNull]
    private ProcessStartInfo GetMacInfo()
    {
        File.WriteAllText(OsxLaunchScriptPath, OsxLaunchScript);

        FileSystem.AddFileExecutablePermissions(OsxLaunchScriptPath);

        return new ProcessStartInfo("/bin/bash",
            string.Format("-c \"sh {0} '{1}' '{2}'\"",
                          OsxLaunchScriptPath,
                          _startCommand,
                          Directory.GetCurrentDirectory()));
    }

    private const string OsxLaunchScriptPath = "Temp/osx_terminal.sh";

    private const string OsxLaunchScript =
        "#!/bin/bash\n"+
        "cmd=\"$1\";\n"+
        "dir=\"$2\";\n" +
        "osascript <<EOF\n" +
        "    tell application \"Terminal\" to do script \"cd $dir; $cmd\"\n" +
        "EOF\n" +
        "exit 0\n";
}
}