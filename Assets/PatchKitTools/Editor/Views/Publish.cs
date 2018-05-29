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
        private bool _autoPublishAfterUpload = true;
        private bool _forceOverrideDraft = true;

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
            _changelog = GUILayout.TextArea(_changelog);

            EditorGUILayout.LabelField("Automatically publish after upload");
            _autoPublishAfterUpload = EditorGUILayout.Toggle(_autoPublishAfterUpload);

            EditorGUILayout.LabelField("Override draft version if exists");
            _forceOverrideDraft = EditorGUILayout.Toggle(_forceOverrideDraft);

            if (!_forceOverrideDraft)
            {
                EditorGUILayout.HelpBox("If a draft version exists, interaction with the console will be necessary.", MessageType.Warning);
            }

            if (CanBuild())
            {
                if (GUILayout.Button("Ok"))
                {
                    if (OnPublishStart != null) OnPublishStart();

                    MakeVersionHeadless();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Version label must not be empty.", MessageType.Error);
            }
        }

        private void MakeVersionHeadless()
        {
            var platform = Application.platform;
            string toolsSource = Utils.PlatformToToolsSource(platform);
            string toolsTarget = Utils.ToolsExtractLocation();
            
//            var thread = new Thread(
//                () => {
            using (var tools = new Tools(toolsSource, toolsTarget, platform))
            {
                UnityEngine.Debug.Log("Making version...");
                tools.MakeVersion(_apiKey.Key, _appSecret, _label, _changelog, _buildDir, _autoPublishAfterUpload, _forceOverrideDraft);

                if (OnPublish != null) OnPublish();
            }
//                }
//            );

//            thread.Start();
        }

        private bool CanBuild()
        {
            return !string.IsNullOrEmpty(_label);
        }

        public event Action OnPublish;
        public event Action OnPublishStart;
    }
}