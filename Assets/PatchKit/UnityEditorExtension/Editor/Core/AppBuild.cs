using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
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

    public static string Location
    {
        get
        {
            string value = EditorUserBuildSettings.GetBuildLocation(
                EditorUserBuildSettings.activeBuildTarget);

            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            switch (Platform)
            {
                case AppPlatform.Windows32:
                case AppPlatform.Windows64:
                    if (value.EndsWith(".exe"))
                    {
                        return value;
                    }

                    break;
                case AppPlatform.Linux32:
                case AppPlatform.Linux64:
                    return value;
                case AppPlatform.Mac64:
                    if (value.EndsWith(".app"))
                    {
                        return value;
                    }

                    break;
                case null:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return null;
        }
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException();
            }

            switch (Platform)
            {
                case AppPlatform.Windows32:
                case AppPlatform.Windows64:
                    if (!value.EndsWith(".exe"))
                    {
                        throw new ArgumentException();
                    }

                    break;
                case AppPlatform.Linux32:
                case AppPlatform.Linux64:
                    break;
                case AppPlatform.Mac64:
                    if (!value.EndsWith(".app"))
                    {
                        throw new ArgumentException();
                    }

                    break;
                case null:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            EditorUserBuildSettings.SetBuildLocation(
                EditorUserBuildSettings.activeBuildTarget,
                value);
        }
    }

    public static string Create()
    {
#if UNITY_2018_1_OR_NEWER
        BuildReport report = BuildPipeline.BuildPlayer(
            Scenes.ToArray(),
            Location,
            EditorUserBuildSettings.activeBuildTarget,
            BuildOptions.None);

        Assert.IsNotNull(report);

        return report.summary.result == BuildResult.Succeeded
            ? null
            : "Build has failed.";
#else
return BuildPipeline.BuildPlayer(
            Scenes.ToArray(),
            Location,
            EditorUserBuildSettings.activeBuildTarget,
            BuildOptions.None);
        #endif
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

        EditorApplication.delayCall += () =>
        {
            string location = EditorUtility.SaveFilePanel(
                "Select build location:",
                "",
                "",
                extension);

            if (!string.IsNullOrEmpty(location))
            {
                Location = location;
            }
        };
    }
}
}