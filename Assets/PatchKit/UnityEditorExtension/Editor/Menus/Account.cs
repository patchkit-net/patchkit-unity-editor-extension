using PatchKit.UnityEditorExtension.Core;
using PatchKit.UnityEditorExtension.Logic;
using UnityEditor;
using UnityEngine;

namespace PatchKit.UnityEditorExtension.Menus
{
    public class Account : EditorWindow
    {
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
            if (Config.FindOrCreateInstance().GetSavedApiKey() == null)
            {
                var submitKey = new Views.SubmitKey();
                _currentView = submitKey;

                submitKey.OnKeyResolve += () => {
                    _currentView = null;
                };
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
                if (Config.FindOrCreateInstance().GetSavedApiKey() != null)
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

            ApiKey? apiKey = Config.FindOrCreateInstance().GetSavedApiKey();

            GUILayout.Label("Api key:");
            EditorGUILayout.SelectableLabel(apiKey.HasValue ? apiKey.Value.Value : "",
                                            EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("RESET", GUILayout.Width(100)))
                {
                    Config.FindOrCreateInstance().ClearSavedApiKey();

                    Init();
                }
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

        }
    }
}