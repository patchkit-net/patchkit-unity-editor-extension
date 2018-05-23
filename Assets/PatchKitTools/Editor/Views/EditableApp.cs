using System;
using UnityEditor;
using UnityEngine;

namespace PatchKit.Tools.Integration.Views
{
    public class EditableApp : IView
    {
        private readonly ApiUtils _api;
        
        private string _secret;
        private string _lastValidSecret;

        private readonly BuildTarget _buildTarget;

        private Api.Models.Main.App? _appData = null;

        public EditableApp(BuildTarget buildTarget, ApiUtils api, string secret)
        {
            _api = api;
            _secret = secret;
            _lastValidSecret = _secret;

            _buildTarget = buildTarget;

            _appData = _api.GetAppInfo(_secret);
        }
            
        public void Show()
        {
            var newSecret = EditorGUILayout.TextField(_secret);
            if (newSecret != _secret)
            {
                OnSecretChange(newSecret);
            }

            if (_appData.HasValue)
            {
                GUILayout.Label("Name: " + _appData.Value.Name);
            }
            else
            {
                EditorGUILayout.HelpBox("Failed to resolve an app with this secret.", MessageType.Error);

                if (GUILayout.Button("Reset"))
                {
                    OnSecretChange(_lastValidSecret);
                }
            }
        }

        private void OnSecretChange(string newSecret)
        {
            _appData = TryGetAppInfo(_api, newSecret);
            _secret = newSecret;

            if (_appData.HasValue)
            {
                Config.Instance().Cache.UpdateEntry(_buildTarget, _appData.Value);
                _lastValidSecret = _secret;
            }
        }

        private Api.Models.Main.App? TryGetAppInfo(ApiUtils api, string secret)
        {
            try
            {
                return api.GetAppInfo(secret);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}