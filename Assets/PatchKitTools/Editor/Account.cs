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

        private Views.IView _currentView = null;

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            if (_apiKey == null)
            {
                _apiKey = ApiKey.LoadCached();

                if (_apiKey == null)
                {
                    var submitKey = new Views.SubmitKey();
                    _currentView = submitKey;

                    submitKey.OnKeyResolve += (key) => {
                        _apiKey = key;
                        ApiKey.Cache(key);

                        _currentView = null;
                    };
                }
            }
        }

        private void OnGUI()
        {
            if (_currentView != null)
            {
                _currentView.Show();
            }
            else
            {
                if (_apiKey != null)
                {
                    AccountGUI();
                }
                else
                {
                    Init();
                }
            }

            Repaint();
        }

        private void AccountGUI()
        {
            GUILayout.Label("Account", EditorStyles.boldLabel);

            GUILayout.Label("Api key:");
            EditorGUILayout.SelectableLabel(_apiKey.Key, EditorStyles.helpBox);

            if (GUILayout.Button("RESET"))
            {
                ApiKey.ClearCached();
                _apiKey = null;

                Init();
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
    }
}