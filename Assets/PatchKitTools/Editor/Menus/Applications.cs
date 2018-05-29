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
        private Dictionary<BuildTarget, Views.EditableApp> _cachedAppsView;

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
                if (_apiKey == null)
                {
                    return;
                }
            }

            if (_api == null)
            {
                _api = new ApiUtils(_apiKey);
            }

            _appCache = Config.Instance().Cache;

            _cachedAppsView = _appCache.AppsByPlatform()
                .Select(entry => new KeyValuePair<BuildTarget, Views.EditableApp>(entry.Key, new Views.EditableApp(entry.Key, _api, entry.Value)))
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

            if (_apiKey == null)
            {
                EditorGUILayout.HelpBox("Please resolve the API key using the Account window.", MessageType.Error);
            }
            else
            {
                if (_cachedAppsView != null && _cachedAppsView.Count > 0)
                {
                    GUILayout.Label("Currently cached app:", EditorStyles.boldLabel);

                    foreach (var entry in _cachedAppsView)
                    {
                        GUILayout.Label("For " + entry.Key.ToString(), EditorStyles.boldLabel);
                        entry.Value.Show();
                        if (GUILayout.Button("Remove entry"))
                        {
                            _appCache.RemoveEntry(entry.Key);
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
}