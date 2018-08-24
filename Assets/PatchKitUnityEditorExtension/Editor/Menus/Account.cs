using UnityEditor;
using UnityEngine;

namespace PatchKit.Tools.Integration
{
    public class Account : EditorWindow
    {
        private ApiKey _apiKey;

        [MenuItem("Tools/PatchKit/Account", false, 2)]
        public static void ShowWindow()
        {
            EditorWindow window = EditorWindow.GetWindow(typeof(Account), false, "Account");
            window.maxSize = new UnityEngine.Vector2(400, 125);
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

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("RESET", GUILayout.Width(100)))
                {
                    ApiKey.ClearCached();
                    _apiKey = null;

                    Init();
                }
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();
            
        }
    }
}