using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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

            _appCache = Config.Instance().Cache;

            _cachedAppsView = _appCache.AppsByPlatform()
                .Select(entry => new KeyValuePair<BuildTarget, Views.App>(entry.Key, new Views.App(_api.GetAppInfo(entry.Value))))
                .ToDictionary(entry => entry.Key, entry => entry.Value);
        }

        private void Awake()
        {
            Init();
        }

        private void OnFocus()
        {
            Init();
        }

        private void OnGUI()
        {
            if (_apiKey == null)
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

                    EditorGUILayout.Separator();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No cached apps.", MessageType.Info);
            }
        }
    }
}