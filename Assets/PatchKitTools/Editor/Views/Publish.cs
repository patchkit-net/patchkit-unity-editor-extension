using System;
using UnityEditor;
using UnityEngine;

namespace PatchKit.Tools.Integration.Views
{
    public class Publish : IView
    {
        private Api.Models.Main.App _selectedApp;

        private readonly ApiKey _apiKey;

        private readonly string _buildDir;

        private string _label = "";
        private string _changelog = "";
        private bool _autoPublishAfterUpload = true;
        private bool _forceOverrideDraft = true;

        private bool _publishHasBeenExecuted;

        public Publish(ApiKey apiKey, string appSecret, string buildDir, Api.Models.Main.App selectedApp)
        {
            _apiKey = apiKey;
            _buildDir = buildDir;
            _selectedApp = selectedApp;
           
        }

        public void Show()
        {
            GUILayout.Label(_selectedApp.Name, EditorStyles.centeredGreyMiniLabel);
            if (GUILayout.Button(new GUIContent("←", "Change application"), GUILayout.Width(40)))
            {
                if (OnChangeApp != null)
                {
                    OnChangeApp();
                }
            }

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("\n                                 * Publishing *\n" +
                    " The version will be sent with the following information. \n", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();

            
            GUILayout.Label("*Version label: ");
            _label = GUILayout.TextField(_label);

            GUILayout.Label("Changelog: ");
            _changelog = GUILayout.TextArea(_changelog, GUILayout.MinHeight(200));

            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Automatically publish after upload");
                _autoPublishAfterUpload = EditorGUILayout.Toggle(_autoPublishAfterUpload);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Override draft version if it exists");
                _forceOverrideDraft = EditorGUILayout.Toggle(_forceOverrideDraft);
            }
            EditorGUILayout.EndHorizontal();

            if (!_forceOverrideDraft)
            {
                EditorGUILayout.HelpBox("If a draft version exists, interaction with the console will be necessary.", MessageType.Warning);
            }

            if (CanBuild())
            {
                EditorGUILayout.Separator();
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Send version", GUILayout.Width(150)))
                    {
                        if (!TextValidation.DoesStringContainOnlyAllowedCharacters(_changelog) || !TextValidation.DoesStringContainOnlyAllowedCharacters(_label))
                        {
                            if(EditorUtility.DisplayDialog("Warning", "Use only English characters and ':', '_' or '-' in label and changelog input.\n \n" +
                                "Unfortunately PatchKit plugin does not support other languages encoding. If you need to write correct information, please login to your PatchKit Panel and set Version Properties for your application.",
                                "Set using Panel", "Ok"))
                            {
                            
                                //Application.OpenURL("https://panel.patchkit.net/apps/" + _selectedApp.Value.Id);
                                Application.OpenURL("http://staging.patchkit.waw.pl/apps/" + _selectedApp.Id+ "/versions");
                            }
                        }
                        else
                        {
                            if (OnPublishStart != null)
                            {
                                OnPublishStart();
                            }

                            MakeVersionHeadless();
                        }
                    }
                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.HelpBox("* Version label cannot be empty.", MessageType.Error);
            }
        }

        private void MakeVersionHeadless()
        {
            var platform = Application.platform;
            string toolsSource = Utils.PlatformToToolsSource(platform);
            string toolsTarget = Utils.ToolsExtractLocation();
            
            using (var tools = new Tools(toolsSource, toolsTarget, platform))
            {
                BuildAndPublish.messagesView.AddMessage("Making version...", UnityEditor.MessageType.Info);
                UnityEngine.Debug.Log("Making version...");
                tools.MakeVersion(_apiKey.Key, _selectedApp.Secret, _label, _changelog, _buildDir, _autoPublishAfterUpload, _forceOverrideDraft);

                if (OnPublish != null)
                {
                    OnPublish();
                }
            }
        }

        private bool CanBuild()
        {
            return !string.IsNullOrEmpty(_label);
        }

        public event Action OnPublish;
        public event Action OnPublishStart;
        public event Action OnChangeApp;
    }

}