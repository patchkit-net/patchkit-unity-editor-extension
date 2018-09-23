using System;
using PatchKit.UnityEditorExtension.Core;
using UnityEditor;
using UnityEngine;

namespace PatchKit.UnityEditorExtension.Views
{
    public class EditableApp : IView
    {
        private AppSecret _secret;
        private AppSecret _lastValidSecret;

        private readonly AppPlatform _appPlatform;

        private Api.Models.Main.App? _appData = null;

        public EditableApp(AppPlatform appPlatform, AppSecret secret)
        {
            _secret = secret;
            _lastValidSecret = _secret;

            _appPlatform = appPlatform;

            _appData = Core.Api.GetAppInfo(_secret);
        }

        public void Show()
        {
            var newSecret = EditorGUILayout.TextField(_secret.Value);
            if (newSecret != _secret.Value)
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
                    OnSecretChange(_lastValidSecret.Value);
                }
            }
        }

        private void OnSecretChange(string newSecret)
        {
            _appData = TryGetAppInfo(newSecret);
            _secret = new AppSecret(newSecret);

            if (_appData.HasValue)
            {
                Config.SetLinkedAppSecret(new AppSecret(_appData.Value.Secret), _appPlatform);
                _lastValidSecret = _secret;
            }
        }

        private Api.Models.Main.App? TryGetAppInfo(string secret)
        {
            try
            {
                return Core.Api.GetAppInfo(new AppSecret(secret));
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}