using System;
using UnityEditor;
using UnityEngine;

namespace PatchKit.Tools.Integration
{
    public class SubmitKeyMenu : EditorWindow
    {
        private string _apiKeyString = "...";
        private bool _invalidSubmit = false;
        private bool _submit = false;

        void OnGUI()
        {
            _submit = false;
            
            GUILayout.Label ("Enter your API key", EditorStyles.boldLabel);
            _apiKeyString = EditorGUILayout.TextField ("API Key:", _apiKeyString);
            
            if (GUILayout.Button("Submit"))
            {
                _submit = true;

                var apiKey = new ApiKey(_apiKeyString);

                if (!apiKey.IsValid())
                {
                    _invalidSubmit = true;
                    apiKey = null;
                }
                else
                {
                    _invalidSubmit = false;

                    if (OnKeyResolve != null) OnKeyResolve(apiKey);

                    this.Close();
                }
            }

            if (_invalidSubmit)
            {
                EditorGUILayout.HelpBox("Invalid key...", MessageType.Error);
            }
        }

        public static void OpenWindow(Action<ApiKey> onFinish)
        {
            var submitKeyMenu = EditorWindow.GetWindow(typeof(SubmitKeyMenu)) as SubmitKeyMenu;
            submitKeyMenu.OnKeyResolve += onFinish;
        }

        public event Action<ApiKey> OnKeyResolve;
    }
}