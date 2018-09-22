using JetBrains.Annotations;
using PatchKit.UnityEditorExtension.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace PatchKit.UnityEditorExtension.Menus
{
public class Account : EditorWindow
{
    [MenuItem("Tools/PatchKit/Account", false, 2)]
    public static void ShowWindow()
    {
        EditorWindow window = GetWindow(typeof(Account), false, "Account");
        Assert.IsNotNull(window);
        window.maxSize = new Vector2(400, 125);
    }

    private Views.IView _currentView;

    [UsedImplicitly]
    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        if (!Config.GetSavedApiKey().HasValue)
        {
            var submitKey = new Views.SubmitKey();
            _currentView = submitKey;

            submitKey.OnKeyResolve += () => { _currentView = null; };
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
            if (Config.GetSavedApiKey().HasValue)
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

        ApiKey? apiKey = Config.GetSavedApiKey();

        GUILayout.Label("Api key:");
        EditorGUILayout.SelectableLabel(
            apiKey.HasValue ? apiKey.Value.Value : "",
            EditorStyles.helpBox);

        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("RESET", GUILayout.Width(100)))
            {
                Config.ClearSavedApiKey();

                Init();
            }

            GUILayout.FlexibleSpace();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();
    }
}
}