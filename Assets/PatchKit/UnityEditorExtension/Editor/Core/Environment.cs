using System;
using UnityEditor;
using UnityEngine;

namespace PatchKit.UnityEditorExtension.Core
{
public static class Environment
{
    public static EnvironmentPlatform Platform
    {
        get
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                    return EnvironmentPlatform.Windows;
                case RuntimePlatform.OSXEditor:
                    return EnvironmentPlatform.Mac;
#if UNITY_5_5_OR_NEWER
                case RuntimePlatform.LinuxEditor:
                    return EnvironmentPlatform.Linux;
#endif
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public static AppPlatform? BuildPlatform
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

#if UNITY_2017_1_OR_NEWER
                case BuildTarget.StandaloneOSXIntel:
#else
                case BuildTarget.StandaloneOSXIntel64:
#endif
                    return AppPlatform.Mac64;

                default:
                    return null;
            }
        }
    }
}
}