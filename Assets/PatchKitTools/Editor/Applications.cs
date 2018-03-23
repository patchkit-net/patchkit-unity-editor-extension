using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using App = PatchKit.Api.Models.Main.App;

namespace PatchKit.Tools.Integration
{
    public class Applications : EditorWindow
    {
        private ApiKey _apiKey;

        private ApiUtils _api;

        private List<Views.App> _appViews;

        private App? _cachedApp = null;
        private Views.App _cachedAppView;

        [MenuItem("Window/PatchKit/Applications")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(Applications), false, "Applications");
        }

        private void Init()
        {
            if (_apiKey == null)
            {
                _apiKey = ApiKey.LoadCached();
            }

            _api = new ApiUtils(_apiKey);

            _appViews = _api.GetApps()
                .Select(appData => new Views.App(appData))
                .ToList();

            _cachedApp = AppCache.LoadCachedApp(_api);
            if (_cachedApp.HasValue)
            {
                _cachedAppView = new Views.App(_cachedApp.Value);
            }
        }

        private void Awake()
        {
            Init();
        }

        private void OnGUI()
        {
            if (_apiKey == null)
            {
                EditorGUILayout.HelpBox("Please resolve the API key.", MessageType.Error);
            }

            if (GUILayout.Button("Reload"))
            {
                Init();
            }

            if (_appViews != null)
            {
                GUILayout.Label("Apps:", EditorStyles.boldLabel);
                foreach (var appView in _appViews)
                {
                    appView.Show();
                }
            }

            if (_cachedApp.HasValue)
            {
                GUILayout.Label("Currently cached app:", EditorStyles.boldLabel);
                _cachedAppView.Show();
            }
        }
    }
}