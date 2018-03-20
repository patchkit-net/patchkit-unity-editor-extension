using UnityEditor;
using UnityEngine;

namespace PatchKit.Tools.Integration
{
    public class Account : EditorWindow
    {
        private ApiKey _apiKey;

        [MenuItem("Window/PatchKit/Account")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(Account), false, "Account");
        }

        void OnGUI()
        {
            if (_apiKey == null)
            {
                _apiKey = ApiKey.LoadCached();

                if (_apiKey == null)
                {
                    GUILayout.Label("Please resolve the API key.", EditorStyles.boldLabel);
                    UpdateKey();
                    return;
                }
            }

            GUILayout.Label("Account", EditorStyles.boldLabel);

            GUILayout.Label("Api key:");
            EditorGUILayout.SelectableLabel(_apiKey.Key, EditorStyles.helpBox);
            
            if (GUILayout.Button("Update key"))
            {
                UpdateKey();
            }

            if (GUILayout.Button("RESET"))
            {
                ApiKey.ClearCached();
                _apiKey = null;
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Contact"))
            {
                Application.OpenURL("http://docs.patchkit.net/contact.html");
            }

            if (GUILayout.Button("Documentation"))
            {
                Application.OpenURL("http://docs.patchkit.net/");
            }
        }

        private void UpdateKey()
        {
            SubmitKeyMenu.OpenWindow(key => {
                ApiKey.Cache(key);
                _apiKey = key;
                this.Focus();
            });
        }
    }
}