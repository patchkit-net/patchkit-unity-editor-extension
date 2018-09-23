using System.Collections.Generic;
using System.Linq;
using PatchKit.UnityEditorExtension.Core;
using UnityEditor;
using UnityEngine;

namespace PatchKit.UnityEditorExtension.Menus
{
public class Applications : EditorWindow
{
    private Dictionary<AppPlatform, Views.EditableApp> _savedAppsView;

    [MenuItem("Tools/PatchKit/Applications", false, 1)]
    public static void ShowWindow()
    {
        GetWindow(typeof(Applications), false, "Applications");
    }

    private void Init()
    {
        _savedAppsView = Config.GetLinkedAppSecrets()
            .Where(x => x.Value.HasValue)
            .Select(
                entry => new KeyValuePair<AppPlatform, Views.EditableApp>(
                    entry.Key,
                    new Views.EditableApp(entry.Key, entry.Value.Value)))
            .ToDictionary(entry => entry.Key, entry => entry.Value);
    }

    private void Awake()
    {
        Init();
    }

    private void OnFocus()
    {
        Init();
    }

    private void OnGUI()
    {
        if (Config.GetSavedApiKey() == null)
        {
            Init();
        }

        if (Config.GetSavedApiKey() == null)
        {
            EditorGUILayout.HelpBox(
                "Please resolve the API key using the Account window.",
                MessageType.Error);
        }
        else
        {
            if (_savedAppsView != null && _savedAppsView.Count > 0)
            {
                GUILayout.Label(
                    "Currently cached app:",
                    EditorStyles.boldLabel);

                foreach (var entry in _savedAppsView)
                {
                    GUILayout.Label("For " + entry.Key, EditorStyles.boldLabel);
                    entry.Value.Show();
                    if (GUILayout.Button("Remove entry"))
                    {
                        Config.ClearLinkedAppSecret(entry.Key);
                        Init();
                    }

                    EditorGUILayout.Separator();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No cached apps.", MessageType.Info);
            }
        }
    }
}
}