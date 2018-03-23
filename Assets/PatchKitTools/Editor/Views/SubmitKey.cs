using System;
using UnityEditor;
using UnityEngine;

namespace PatchKit.Tools.Integration.Views
{
    public class SubmitKey : IView
    {
        private string _apiKeyString = "";
        private bool _invalidSubmit = false;

        public void Show()
        {            
            GUILayout.Label ("Enter your API key", EditorStyles.boldLabel);
            _apiKeyString = EditorGUILayout.TextField ("API Key:", _apiKeyString);
            
            if (!string.IsNullOrEmpty(_apiKeyString))
            {
                var apiKey = new ApiKey(_apiKeyString);

                if (!apiKey.IsValid())
                {
                    EditorGUILayout.HelpBox("Invalid key...", MessageType.Error);
                }
                else
                {
                    if (GUILayout.Button("Submit"))
                    {
                        if (OnKeyResolve != null) OnKeyResolve(apiKey);
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Please enter your API key", MessageType.Info);
            }
        }

        public event Action<ApiKey> OnKeyResolve;
    }
}