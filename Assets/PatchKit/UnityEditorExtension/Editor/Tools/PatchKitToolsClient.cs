using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PatchKit.UnityEditorExtension.Core;
using UnityEngine.Assertions;
using Environment = PatchKit.UnityEditorExtension.Core.Environment;

namespace PatchKit.UnityEditorExtension.Tools
{
public class PatchKitToolsClient : IDisposable
{
    private const string TempLocation = "Temp/patchkit-tools";

    public PatchKitToolsClient()
    {
        Clear();

        Zip.Unarchive(
            PatchKitToolsPackages.GetPath(Environment.Platform),
            TempLocation);

        FileSystem.AddDirExecutablePermissionsRecursive(TempLocation);
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
        var arguments = new List<string>
        {
            "--secret",
            appSecret,
            "-a",
            apiKey,
            "--label",
            label,
            "--changelog",
            string.IsNullOrEmpty(changelog)
                ? "\"\""
                : changelog.Replace("\n", "\\n"),
            "--files",
            buildDir
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

        string[] argumentsArray = arguments.ToArray();

        Execute("make-version", argumentsArray);

        UnityEngine.Debug.Log(
            "make-version " + string.Join(" ", argumentsArray));
    }

    public void Execute(string tool, string[] args)
    {
        string command = tool;

        if (args != null)
        {
            command += " ";
            foreach (string arg in args)
            {
                Assert.IsNotNull(arg);
                if (arg.Contains(' '))
                {
                    command += "\"" + arg + "\" ";
                }
                else
                {
                    command += arg + " ";
                }
            }
        }

        switch (Environment.Platform)
        {
            case EnvironmentPlatform.Windows:
                ExecuteWindows(command);
                break;
            case EnvironmentPlatform.Linux:
                ExecuteLinux(command);
                break;
            case EnvironmentPlatform.Mac:
                ExecuteMac(command);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void ExecuteLinux(string command)
    {
        string path =
            Path.GetFullPath(Path.Combine(TempLocation, "patchkit-tools"));

        PluginAssert.IsTrue(File.Exists(path));

        string executable = string.Format("bash -c '{0} {1}'", path, command);

        new Terminal(executable);
    }

    private void ExecuteMac(string command)
    {
        string path =
            Path.GetFullPath(Path.Combine(TempLocation, "patchkit-tools"));

        PluginAssert.IsTrue(File.Exists(path));

        string executable = string.Format("{0} {1}", path, command);

        new Terminal(executable);
    }

    private void ExecuteWindows(string command)
    {
        string path = Path.GetFullPath(
            Path.Combine(TempLocation, "patchkit-tools.bat"));

        PluginAssert.IsTrue(File.Exists(path));

        string executable = path + " " + command;

        new Terminal(executable);
    }

    private void Clear()
    {
        if (Directory.Exists(TempLocation))
        {
            Directory.Delete(TempLocation, true);
        }
    }

    public void Dispose()
    {
        //Clear();
    }
}
}