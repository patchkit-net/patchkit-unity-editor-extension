using System;
using PatchKit.UnityEditorExtension.Core;
using UnityEditor;
using UnityEngine;

namespace PatchKit.UnityEditorExtension.Views
{
public class SubmitKey : IView
{
    private string _apiKeyValue = "";

    public void Show()
    {
        GUILayout.Label("Enter your API key", EditorStyles.boldLabel);
        _apiKeyValue = EditorGUILayout.TextField("API Key:", _apiKeyValue);

        if (!string.IsNullOrEmpty(_apiKeyValue))
        {
            ApiKey? apiKey = null;
            string apiKeyValidationError = string.Empty;
            try
            {
                apiKey = new ApiKey(_apiKeyValue);
            }
            catch (ValidationException e)
            {
                apiKeyValidationError = e.Message;
            }

            if (!apiKey.HasValue)
            {
                EditorGUILayout.HelpBox(
                    "Invalid key: " + apiKeyValidationError,
                    MessageType.Error);
            }
            else
            {
                EditorGUILayout.Separator();
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Submit", GUILayout.Width(100)))
                    {
                        Config.SetSavedApiKey(apiKey.Value);

                        if (OnKeyResolve != null)
                        {
                            OnKeyResolve();
                        }
                    }

                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        else
        {
            EditorGUILayout.HelpBox(
                "Please enter your API key",
                MessageType.Info);
        }
    }

    public event Action OnKeyResolve;
}
}