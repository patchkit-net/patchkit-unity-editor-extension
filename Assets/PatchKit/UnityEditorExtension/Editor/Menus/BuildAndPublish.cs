using System.IO;
using PatchKit.Api.Models.Main;
using PatchKit.UnityEditorExtension.Core;
using UnityEditor;

namespace PatchKit.UnityEditorExtension.Menus
{
public class BuildAndPublish : EditorWindow
{
    private App _selectedApp;

    private bool _reimportLock;

    private Views.IView _currentView;
    public static Views.MessagesView messagesView = new Views.MessagesView();

    [MenuItem("Tools/PatchKit/Build And Publish %&#b", false, 51)]
    public static void ShowWindow()
    {
        EditorWindow window = GetWindow(
            typeof(BuildAndPublish),
            false,
            "Build & Publish");
        window.maxSize = new UnityEngine.Vector2(410, 2000);
        messagesView.ClearList();
    }

    private void Awake()
    {
        LockReload();

        if (Config.GetSavedApiKey() == null)
        {
            var submitKey = new Views.SubmitKey();

            submitKey.OnKeyResolve += OnKeyResolved;

            _currentView = submitKey;
        }
        else
        {
            AppSecret? selectedAppSecret =
                Config.GetSavedAppSecret(AppBuild.Platform.Value);

            if (!selectedAppSecret.HasValue)
            {
                BeginSelectAppView();
            }
            else
            {
                _selectedApp = Core.Api.GetAppInfo(selectedAppSecret.Value);
                BeginBuildView();
            }
        }

        messagesView.OnChangeApp += BeginBuildView;
    }

    private void OnKeyResolved()
    {
        var appSecret = Config.GetSavedAppSecret(AppBuild.Platform.Value);

        if (appSecret.HasValue)
        {
            _selectedApp = Core.Api.GetAppInfo(appSecret.Value);
        }

        BeginSelectAppView();
    }

    private void OnAppSelected(App app)
    {
        _selectedApp = app;

        Config.SetSavedAppSecret(
            new AppSecret(app.Secret),
            AppBuild.Platform.Value);

        BeginBuildView();
    }

    private void BeginBuildView() // *** 3 view *** //
    {
        Views.BuildApp build = new Views.BuildApp(_selectedApp);

        build.OnSuccess += OnBuildSuccess;
        build.OnFailure += OnBuildFailed;
        build.OnChangeApp += OnBuildChangeApp;

        _currentView = build;

        messagesView.ClearList();
    }

    private void BeginSelectAppView() // *** 1 view *** //
    {
        Views.ChooseApp chooseApp = new Views.ChooseApp();

        chooseApp.OnCreateApp += OnCreateApp;
        chooseApp.OnSelectApp += OnSelectApp;

        _currentView = chooseApp;
    }

    private void OnSelectApp() // *** 2.1 view *** //
    {
        Views.SelectApp selectApp = new Views.SelectApp();

        selectApp.OnAppSelected += OnAppSelected;
        selectApp.OnChangeApp += BeginSelectAppView;

        _currentView = selectApp;
    }

    private void OnCreateApp() // *** 2.2 view *** //
    {
        Views.CreateApp createApp = new Views.CreateApp();

        createApp.OnAppSelected += OnAppSelected;
        createApp.OnChangeApp += BeginSelectAppView;

        _currentView = createApp;
    }

    private string ResolveBuildDir()
    {
        return Path.GetDirectoryName(AppBuild.Location);
    }

    private void OnBuildSuccess() // *** 4 view *** //
    {
        var publishApp = new Views.Publish(ResolveBuildDir(), _selectedApp);

        publishApp.OnPublishStart += OnPublishStart;
        publishApp.OnChangeApp += OnBuildChangeApp;
        publishApp.OnChangeApp += BeginSelectAppView;
        _currentView = publishApp;
    }

    private void OnBuildFailed(string errorMessage)
    {
        messagesView.AddMessage(
            "Build failed, " + errorMessage,
            MessageType.Error);
        _currentView = messagesView;
    }

    private void OnBuildChangeApp()
    {
        BeginSelectAppView();
    }

    private void OnPublishStart()
    {
        messagesView.AddMessage("Processing...", MessageType.Info);
        _currentView = messagesView;
    }

    private void OnGUI()
    {
        if (_currentView != null)
        {
            _currentView.Show();
        }
        else
        {
            Close();
        }

        PreventReloadIfLocked();

        Repaint();
    }

    private void OnDestroy()
    {
        if (_reimportLock)
        {
            UnlockReload();
        }
    }

    private void LockReload()
    {
        EditorApplication.LockReloadAssemblies();
        _reimportLock = true;
    }

    private void UnlockReload()
    {
        EditorApplication.UnlockReloadAssemblies();
        _reimportLock = false;
    }

    private void PreventReloadIfLocked()
    {
        if (EditorApplication.isCompiling && _reimportLock)
        {
            EditorApplication.LockReloadAssemblies();
        }
    }
}
}