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

        private AppCache _appCache;
        private Dictionary<BuildTarget, Views.App> _cachedAppsView;

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

            if (_api == null)
            {
                _api = new ApiUtils(_apiKey);
            }

            _appCache = new AppCache(Config.instance().localCachePath);

            _cachedAppsView = _appCache.AppsByPlatform()
                .Select(entry => new KeyValuePair<BuildTarget, Views.App>(entry.Key, new Views.App(entry.Value)))
                .ToDictionary(entry => entry.Key, entry => entry.Value);
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

            if (_cachedAppsView != null && _cachedAppsView.Count > 0)
            {
                GUILayout.Label("Currently cached app:", EditorStyles.boldLabel);

                foreach (var entry in _cachedAppsView)
                {
                    GUILayout.Label("For " + entry.Key.ToString());
                    entry.Value.Show();
                    if (GUILayout.Button("Reset"))
                    {
                        _appCache.RemoveEntry(entry.Key, entry.Value.Data);
                        Init();
                    }

                    GUILayout.Space(50);
                }
            }
        }
    }
}