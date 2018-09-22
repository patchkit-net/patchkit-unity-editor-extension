using System;
using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;
using UnityEngine.Assertions;

namespace PatchKit.UnityEditorExtension.Core
{
public static class FileSystem
{
    public static void AddDirExecutablePermissionsRecursive(
        [NotNull] string dirPath)
    {
        if (dirPath == null)
        {
            throw new ArgumentNullException("dirPath");
        }

        if (!Directory.Exists(dirPath))
        {
            throw new DirectoryNotFoundException(
                "Directory not found - " + dirPath);
        }

        if (Environment.Platform == EnvironmentPlatform.Windows)
        {
            return;
        }

        var processInfo = new ProcessStartInfo
        {
            FileName = "chmod",
            Arguments = string.Format("+x \"{0}\" -R", dirPath)
        };

        using (Process process = Process.Start(processInfo))
        {
            Assert.IsNotNull(process);
            process.WaitForExit();
        }
    }
}
}