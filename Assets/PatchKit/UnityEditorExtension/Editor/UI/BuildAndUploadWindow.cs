using JetBrains.Annotations;
using PatchKit.UnityEditorExtension.Core;
using UnityEditor;
using UnityEngine;

namespace PatchKit.UnityEditorExtension.UI
{
public class BuildAndUploadWindow : Window
{
    [MenuItem("Tools/PatchKit/Build and Upload %&#b", false, 51)]
    public static void ShowWindow()
    {
        GetWindow(typeof(BuildAndUploadWindow), true, "Build & Upload");
    }

    [SerializeField]
    private AppPlatform _lastPlatform;

    [SerializeField]
    private bool _hasPlatform;

    [UsedImplicitly]
    private void Awake()
    {
        ResetView();
    }

    private void Update()
    {
        if (AppBuild.Platform.HasValue)
        {
            if (!_hasPlatform || _lastPlatform != AppBuild.Platform.Value)
            {
                ResetView();
            }

            _hasPlatform = true;
        }
        else
        {
            _hasPlatform = false;
        }
    }

    private void ResetView()
    {
        if (!AppBuild.Platform.HasValue)
        {
            return;
        }

        _lastPlatform = AppBuild.Platform.Value;

        ClearAndPush<BuildAndUploadScreen>().Initialize(_lastPlatform);
    }
}
}