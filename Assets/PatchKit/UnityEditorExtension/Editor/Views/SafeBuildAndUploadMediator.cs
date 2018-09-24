using System;
using System.IO;
using JetBrains.Annotations;
using PatchKit.Api;
using PatchKit.UnityEditorExtension.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace PatchKit.UnityEditorExtension.Views
{
public class SafeBuildAndUploadMediator
{
    [NotNull]
    private readonly SafeBuildAndUploadView _view;

    public SafeBuildAndUploadMediator([NotNull] SafeBuildAndUploadView view)
    {
        if (view == null)
        {
            throw new ArgumentNullException("view");
        }

        _view = view;
    }

    public void Initialize()
    {
        LinkedAppName = string.Empty;
        LinkedAppSecret = string.Empty;
        PublishOnUpload = true;
        OverwriteDraftVersion = true;
        VersionLabel = string.Empty;
        VersionChangelog = string.Empty;

        AppSecret? appSecret = Config.GetLinkedAppSecret(Platform);

        Assert.IsTrue(appSecret.HasValue);

        try
        {
            LinkedAppName = Core.Api.GetAppInfo(appSecret.Value).Name;
            LinkedAppSecret = appSecret.Value.Value;
        }
        catch (ApiConnectionException e)
        {
            Debug.LogWarning(e);
        }
        catch (ApiResponseException e)
        {
            Debug.LogWarning(e);

            Config.UnlinkApp(Platform);
        }
    }

    public AppPlatform Platform
    {
        get
        {
            Assert.IsTrue(AppBuild.Platform.HasValue);

            return AppBuild.Platform.Value;
        }
    }

    public string LinkedAppName { get; private set; }

    public string LinkedAppSecret { get; private set; }

    public string BuildLocation
    {
        get { return AppBuild.Location; }
    }

    public bool IsBuildLocationSelected
    {
        get { return !string.IsNullOrEmpty(BuildLocation); }
    }

    public string VersionLabel { get; set; }

    public string VersionLabelValidationError
    {
        get { return AppVersionLabel.GetValidationError(VersionLabel); }
    }

    public string VersionChangelog { get; set; }

    public string VersionChangelogValidationError
    {
        get { return AppVersionChangelog.GetValidationError(VersionChangelog); }
    }

    public bool PublishOnUpload { get; set; }

    public bool OverwriteDraftVersion { get; set; }

    public void SwitchPlatform()
    {
        EditorWindow.GetWindow(
            Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
    }

    public void SwitchScenes()
    {
        EditorWindow.GetWindow(
            Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
    }

    public void ChangeBuildLocation()
    {
        AppBuild.OpenLocationDialog();
    }

    public void SwitchLinkedApp()
    {
        _view.Window.Push(
            new LinkAppScreen(
                Platform,
                (v, app) =>
                {
                    Assert.IsNotNull(v);
                    _view.Window.Pop(v);

                    LinkedAppName = app.Name;
                    LinkedAppSecret = app.Secret;
                },
                v =>
                {
                    Assert.IsNotNull(v);
                    _view.Window.Pop(v);
                },
                _view.Window));
    }

    public void BuildAndUpload()
    {
        Assert.IsTrue(IsBuildLocationSelected);
        Assert.IsNull(VersionLabelValidationError);
        Assert.IsNull(VersionChangelogValidationError);

        if (string.IsNullOrEmpty(AppBuild.Create()))
        {
            ApiKey? apiKey = Config.GetLinkedAccountApiKey();
            Assert.IsTrue(apiKey.HasValue);

            EditorUtility.DisplayProgressBar(
                "Preparing upload...",
                "",
                0.0f);

            EditorApplication.delayCall += () =>
            {
                try
                {
                    using (var tools = new Tools.PatchKitToolsClient())
                    {
                        tools.MakeVersion(
                            apiKey.Value.Value,
                            LinkedAppSecret,
                            VersionLabel,
                            VersionChangelog,
                            Path.GetDirectoryName(BuildLocation),
                            PublishOnUpload,
                            OverwriteDraftVersion);
                    }
                }
                finally
                {
                    EditorUtility.ClearProgressBar();
                }

                EditorUtility.DisplayDialog(
                    "Uploading",
                    "Your game has been successfully built and is being uploaded right now.\n\n" +
                    "You can track the progress in console window.",
                    "OK");

                _view.Window.Close();
            };
        }
    }
}
}