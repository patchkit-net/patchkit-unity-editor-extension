using UnityEditor;
using UnityEngine;
using PatchKit.Api;
using PatchKit.Api.Models.Main;
using PatchKit.Network;
using System.Linq;
using System.IO;

namespace PatchKit.Tools.Integration
{
    public class BuildAndPublish : EditorWindow
    {
        private ApiKey _apiKey;
        private ApiUtils _api = null;
        private App? _selectedApp = null;
        private AppCache _appCache;

        private bool _reimportLock = false;

        private Views.IView _currentView;

        [MenuItem("File/Build and Publish")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(BuildAndPublish), false, "Build & Publish");
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
        }

        private void OnKeyResolved(ApiKey key)
        {
            _apiKey = key;

            ApiKey.Cache(_apiKey);

            _api = new ApiUtils(_apiKey);

            _selectedApp = _api.GetAppInfo(_appCache.AppByPlatform(EditorUserBuildSettings.activeBuildTarget));

            if (_selectedApp.HasValue)
            {
                BeginBuildView();
            }
            else
            {
                BeginSelectAppView();
            }
        }

        private void OnAppSelected(Api.Models.Main.App app)
        {
            _selectedApp = app;
            _appCache.UpdateEntry(EditorUserBuildSettings.activeBuildTarget, app);

            BeginBuildView();
        }

        private void BeginBuildView()
        {
            var build = new Views.BuildApp();

            build.OnSuccess += OnBuildSuccess;
            build.OnFailure += OnBuildFailed;

            _currentView = build;
        }

        private void BeginSelectAppView()
        {
            var selectApp = new Views.SelectApp(_api);

            selectApp.OnAppSelected += OnAppSelected;

            _currentView = selectApp;
        }

        private string ResolveBuildDir()
        {
            return Path.GetDirectoryName(EditorUserBuildSettings.GetBuildLocation(EditorUserBuildSettings.activeBuildTarget));
        }

        private void OnBuildSuccess()
        {
            var publishApp = new Views.Publish(_apiKey, _selectedApp.Value.Secret, ResolveBuildDir());

            publishApp.OnPublishStart += OnPublishStart;

            _currentView = publishApp;
        }

        private void OnBuildFailed(string errorMessage)
        {
            UnityEngine.Debug.LogError(errorMessage);
            _currentView = new Views.Message("Build failed, " + errorMessage, MessageType.Error);
        }

        private void OnPublishStart()
        {
            _currentView = new Views.Message("A console window should open shortly\nKeep the console window open until an appropriate message appears. \nIt is now safe to close this window.", MessageType.Info);
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