using UnityEditor;
using PatchKit.Api.Models.Main;
using System.IO;

namespace PatchKit.Tools.Integration
{
    public class BuildAndPublish : EditorWindow
    {
        private ApiKey _apiKey;
        private ApiUtils _api = null;
        private App _selectedApp;
        private AppCache _appCache;

        private bool _reimportLock = false;

        private Views.IView _currentView;
        public static Views.MessagesView messagesView = new Views.MessagesView();

        [MenuItem("Tools/PatchKit/Build And Publish %&#b", false, 51)]
        public static void ShowWindow()
        {

            EditorWindow window = EditorWindow.GetWindow(typeof(BuildAndPublish), false, "Build & Publish");
            window.maxSize = new UnityEngine.Vector2(410, 2000);
            messagesView.ClearList();

        }

        private void Awake()
        {
            LockReload();
            
            _appCache = Config.Instance().Cache;

            _apiKey = ApiKey.LoadCached();

            if (_apiKey == null)
            {
                var submitKey = new Views.SubmitKey();

                submitKey.OnKeyResolve += OnKeyResolved;

                _currentView = submitKey;
            }
            else
            {
                _api = new ApiUtils(_apiKey);
                string selectedAppSecret = _appCache.AppByPlatform(EditorUserBuildSettings.activeBuildTarget);

                if (string.IsNullOrEmpty(selectedAppSecret))
                {
                    BeginSelectAppView();
                }
                else
                {
                    _selectedApp = _api.GetAppInfo(selectedAppSecret);
                    BeginBuildView();
                }
            }
            messagesView.OnChangeApp += OnBuildChangeApp;
        }

        private void OnKeyResolved(ApiKey key)
        {
            _apiKey = key;

            ApiKey.Cache(_apiKey);

            _api = new ApiUtils(_apiKey);

            _selectedApp = _api.GetAppInfo(_appCache.AppByPlatform(EditorUserBuildSettings.activeBuildTarget));

            BeginSelectAppView();
        }

        private void OnAppSelected(Api.Models.Main.App app)
        {
            _selectedApp = app;
            _appCache.UpdateEntry(EditorUserBuildSettings.activeBuildTarget, app);

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
            Views.SelectApp selectApp = new Views.SelectApp(_api);

            selectApp.OnAppSelected += OnAppSelected;
            selectApp.OnChangeApp += BeginSelectAppView;

            _currentView = selectApp;
        }

        private void OnCreateApp() // *** 2.2 view *** //
        {
            Views.CreateApp createApp = new Views.CreateApp(_api);

            createApp.OnAppSelected += OnAppSelected;
            createApp.OnChangeApp += BeginSelectAppView;
 
            _currentView = createApp;
        }

        private string ResolveBuildDir()
        {
            return Path.GetDirectoryName(EditorUserBuildSettings.GetBuildLocation(EditorUserBuildSettings.activeBuildTarget));
        }

        private void OnBuildSuccess() // *** 4 view *** //
        {
            Views.Publish publishApp = new Views.Publish(_apiKey, _selectedApp.Secret, ResolveBuildDir(), _selectedApp);

            publishApp.OnPublishStart += OnPublishStart;
            publishApp.OnChangeApp += OnBuildChangeApp;
            publishApp.OnChangeApp += BeginSelectAppView;
            _currentView = publishApp;
        }

        private void OnBuildFailed(string errorMessage)
        {
            messagesView.AddMessage("Build failed, " + errorMessage, MessageType.Error);
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