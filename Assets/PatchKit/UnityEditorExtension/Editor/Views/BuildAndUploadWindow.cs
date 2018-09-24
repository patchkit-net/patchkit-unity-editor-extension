using JetBrains.Annotations;
using PatchKit.UnityEditorExtension.Core;
using UnityEditor;
using UnityEngine;

namespace PatchKit.UnityEditorExtension.Views
{
public class BuildAndUploadWindow : Window
{
    [MenuItem("Tools/PatchKit/Build And Publish %&#b", false, 51)]
    public static void ShowWindow()
    {
        GetWindow(typeof(BuildAndUploadWindow), true, "Build & Upload");
    }

    [SerializeField]
    private AppPlatform? _lastPlatform;

    [UsedImplicitly]
    private void Awake()
    {
        ResetView();
    }

    private void Update()
    {
        if (_lastPlatform != AppBuild.Platform)
        {
            ResetView();
        }
    }

    private void ResetView()
    {
        _lastPlatform = AppBuild.Platform;

        if (_lastPlatform.HasValue)
        {
            ClearAndPush(new BuildAndUploadScreen(_lastPlatform.Value, this));
        }
    }
}
}