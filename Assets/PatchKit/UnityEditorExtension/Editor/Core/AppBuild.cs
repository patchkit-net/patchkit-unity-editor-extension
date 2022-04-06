using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
#if UNITY_2018_1_OR_NEWER
using UnityEditor.Build.Reporting;
#endif
using UnityEngine.Assertions;

namespace PatchKit.UnityEditorExtension.Core
{
public static class AppBuild
{
    public static AppPlatform? Platform
    {
        get
        {
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.StandaloneWindows:
                    return AppPlatform.Windows32;

                case BuildTarget.StandaloneWindows64:
                    return AppPlatform.Windows64;

                case BuildTarget.StandaloneLinux:
                    return AppPlatform.Linux32;

                case BuildTarget.StandaloneLinux64:
                    return AppPlatform.Linux64;

#if UNITY_2017_3_OR_NEWER
                case BuildTarget.StandaloneOSX:
#else
                case BuildTarget.StandaloneOSXIntel64:
#endif
                    return AppPlatform.Mac64;

                default:
                    return null;
            }
        }
    }

    [NotNull]
    public static IEnumerable<string> Scenes
    {
        get
        {
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;

            Assert.IsNotNull(scenes);

            return scenes.Where(x => x != null).Select(s => s.path);
        }
    }

    [NotNull]
    public static IEnumerable<string> WarningFiles = Array.Empty<string>();
    
    public static string Location
    {
        get
        {
            string value = EditorUserBuildSettings.GetBuildLocation(
                EditorUserBuildSettings.activeBuildTarget);

            string validationError = GetLocationValidationError(value);

            if (validationError != null)
            {
                Debug.LogError(validationError);
            }

            return value;
        }
        set
        {
            string validationError = GetLocationValidationError(value);

            if (validationError != null)
            {
                throw new ValidationException(validationError);
            }

            EditorUserBuildSettings.SetBuildLocation(
                EditorUserBuildSettings.activeBuildTarget,
                value);
        }
    }

    [NotNull]
    private static IEnumerable<string> FilesOutsideOfBuildEntries(
        [NotNull] string location,
        params string[] buildFiles)
    {
        if (location == null)
        {
            throw new ArgumentNullException(nameof(location));
        }

        if (buildFiles == null)
        {
            throw new ArgumentNullException(nameof(buildFiles));
        }

        string parentDirPath = Path.GetDirectoryName(location);

        Assert.IsNotNull(parentDirPath);

        string[] entries = Directory.GetFileSystemEntries(parentDirPath, "*");

        return entries.Where(
            x =>
            {
                string fileName = Path.GetFileName(x);

                if (Path.GetDirectoryName(x) != parentDirPath)
                {
                    return false;
                }

                if (buildFiles.Contains(fileName))
                {
                    return false;
                }

                if (Platform.IsWindows() && fileName.ToLower().EndsWith(".pdb"))
                {
                    return false;
                }

                return true;
            });
    }

    
    [NotNull]
    private static IEnumerable<string> GetLocationValidationWarningFiles(string location)
    {
        switch (Platform)
        {
            case AppPlatform.Windows32:
            case AppPlatform.Windows64:
                string winBuildFileName = Path.GetFileName(location);
                string winBuildDirName =
                    winBuildFileName.Replace(".exe", "_Data");

                return FilesOutsideOfBuildEntries(
                    location,
                    winBuildFileName,
                    winBuildDirName,
                    "MonoBleedingEdge",
                    "Mono",
                    "UnityCrashHandler32.exe",
                    "UnityCrashHandler64.exe",
                    "UnityPlayer.dll",
                    "WinPixEventRuntime.dll");
            case AppPlatform.Linux32:
            case AppPlatform.Linux64:
                break;
            case AppPlatform.Mac64:
                break;
            case null:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return Array.Empty<string>();
    }

    private static string GetLocationValidationError(string location)
    {
        if (location == null)
        {
            return "Build location cannot be null.";
        }

        if (string.IsNullOrEmpty(location))
        {
            return "Build location cannot be empty.";
        }

        switch (Platform)
        {
            case AppPlatform.Windows32:
            case AppPlatform.Windows64:
                if (!location.EndsWith(".exe"))
                {
                    return
                        "Invalid build location file extension. Should be .exe.";
                }

                break;
            case AppPlatform.Linux32:
            case AppPlatform.Linux64:
                break;
            case AppPlatform.Mac64:
                if (!location.EndsWith(".app"))
                {
                    return
                        "Invalid build location file extension. Should be .app.";
                }

                string macBuildFileName = Path.GetFileName(location);

                if (FilesOutsideOfBuildEntries(location, macBuildFileName).Any())
                {
                    return "Build location must be an empty directory.";
                }

                break;
            case null:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return null;
    }

    public static bool TryCreate(bool removePdbFiles)
    {
        bool success;
#if UNITY_2018_1_OR_NEWER
        BuildReport report = BuildPipeline.BuildPlayer(
            Scenes.ToArray(),
            Location,
            EditorUserBuildSettings.activeBuildTarget,
            BuildOptions.None);

        Assert.IsNotNull(report);

        success = report.summary.result == BuildResult.Succeeded;
#else
        success = string.IsNullOrEmpty(
            BuildPipeline.BuildPlayer(
                Scenes.ToArray(),
                Location,
                EditorUserBuildSettings.activeBuildTarget,
                BuildOptions.None));
#endif

        if (!success)
        {
            return false;
        }

        WarningFiles = GetLocationValidationWarningFiles(Location);
        foreach (string warningFile in WarningFiles)
        {
            Debug.LogWarning("Unknown file in build location: " + warningFile);
        }
            
        if (removePdbFiles)
        {
            RemovePdbFiles();
        }

        return true;
    }

    private static void RemovePdbFiles()
    {
        switch (Platform)
        {
            case AppPlatform.Windows32:
            case AppPlatform.Windows64:
                string parentDirPath = Path.GetDirectoryName(Location);
                Assert.IsNotNull(parentDirPath);
                string[] entries = Directory.GetFileSystemEntries(parentDirPath, "*");
                
                foreach (string pdbFile in entries.Where(e => e.ToLower().EndsWith(".pdb")))
                {
                    if (File.Exists(pdbFile))
                    {
                        File.Delete(pdbFile);
                    }
                }

                break;
            case AppPlatform.Linux32:
                break;
            case AppPlatform.Linux64:
                break;
            case AppPlatform.Mac64:
                break;
            case null:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static void OpenLocationDialog()
    {
        string extension = string.Empty;

        switch (Platform)
        {
            case AppPlatform.Windows32:
            case AppPlatform.Windows64:
                extension = "exe";
                break;
            case AppPlatform.Linux32:
            case AppPlatform.Linux64:
                break;
            case AppPlatform.Mac64:
                extension = "app";
                break;
            case null:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        bool retry;

        do
        {
            retry = false;

            string location = EditorUtility.SaveFilePanel(
                "Select build location:",
                "",
                "",
                extension);

            if (!string.IsNullOrEmpty(location))
            {
                string validationError = GetLocationValidationError(location);

                if (validationError == null)
                {
                    Location = location;
                    WarningFiles = GetLocationValidationWarningFiles(location);
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", validationError, "Ok");
                    retry = true;
                }
            }
        } while (retry);
    }
}
}