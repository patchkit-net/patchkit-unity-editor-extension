using System.Collections.Generic;
using System.Linq;
using PatchKit.UnityEditorExtension.Core;
using PatchKit.UnityEditorExtension.Logic;
using UnityEditor;
using UnityEngine;

namespace PatchKit.UnityEditorExtension.Menus
{
public class Applications : EditorWindow
{
    private Dictionary<AppPlatform, Views.EditableApp> _cachedAppsView;

    [MenuItem("Tools/PatchKit/Applications", false, 1)]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(Applications), false, "Applications");
    }

    private void Init()
    {
        _cachedAppsView = Config.FindOrCreateInstance()
            .GetSavedAppSecrets()
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
        if (Config.FindOrCreateInstance().GetSavedApiKey() == null)
        {
            Init();
        }

        if (Config.FindOrCreateInstance().GetSavedApiKey() == null)
        {
            EditorGUILayout.HelpBox(
                "Please resolve the API key using the Account window.",
                MessageType.Error);
        }
        else
        {
            if (_cachedAppsView != null && _cachedAppsView.Count > 0)
            {
                GUILayout.Label(
                    "Currently cached app:",
                    EditorStyles.boldLabel);

                foreach (var entry in _cachedAppsView)
                {
                    GUILayout.Label(
                        "For " + entry.Key.ToString(),
                        EditorStyles.boldLabel);
                    entry.Value.Show();
                    if (GUILayout.Button("Remove entry"))
                    {
                        Config.FindOrCreateInstance()
                            .ClearSavedAppSecret(entry.Key);
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