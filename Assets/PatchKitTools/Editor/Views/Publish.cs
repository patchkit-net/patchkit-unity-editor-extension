using System;
using UnityEditor;
using UnityEngine;

namespace PatchKit.Tools.Integration.Views
{
    public class Publish : IView
    {
        private readonly ApiKey _apiKey;

        private readonly string _appSecret;
        private readonly string _buildDir;

        private string _label = "";
        private string _changelog = "";

        private bool _publishHasBeenExecuted;

        public Publish(ApiKey apiKey, string appSecret, string buildDir)
        {
            _apiKey = apiKey;
            _appSecret = appSecret;
            _buildDir = buildDir;
        }

        public void Show()
        {
            GUILayout.Label("Publishing", EditorStyles.boldLabel);
            
            GUILayout.Label("Version label: ");
            _label = GUILayout.TextField(_label);

            GUILayout.Label("Changelog: ");
            _changelog = GUILayout.TextField(_changelog);

            if (CanBuild())
            {
                if (GUILayout.Button("Ok"))
                {
                    Tools.MakeVersionHeadless(_apiKey.Key, _appSecret, _label, _changelog, _buildDir, OnPublish);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Version label must not be empty.", MessageType.Error);
            }
        }

        private bool CanBuild()
        {
            return !string.IsNullOrEmpty(_label);
        }

        public event Action OnPublish;
    }
}