using UnityEditor;
using UnityEngine;

namespace PatchKit.Tools.Integration
{
    public class Applications : EditorWindow
    {
        private ApiKey _apiKey;

        [MenuItem("Window/PatchKit/Applications")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(Applications), false, "Applications");
        }

        void OnGUI()
        {
            if (_apiKey == null)
            {
                _apiKey = ApiKey.LoadCached();

                if (_apiKey == null)
                {
                    GUILayout.Label("Please resolve the API key.", EditorStyles.boldLabel);
                    SubmitKeyMenu.OpenWindow(key => {
                        ApiKey.Cache(key);
                        _apiKey = key;
                        this.Focus();
                    });
                    return;
                }
            }
        }
    }
}