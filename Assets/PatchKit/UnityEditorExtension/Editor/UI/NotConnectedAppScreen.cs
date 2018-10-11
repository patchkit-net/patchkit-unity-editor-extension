using PatchKit.UnityEditorExtension.Core;
using UnityEditor;
using UnityEngine;

namespace PatchKit.UnityEditorExtension.UI
{
public class NotConnectedAppScreen : Screen
{
    #region GUI

    public override string Title
    {
        get { return null; }
    }

    public override Vector2? Size
    {
        get { return null; }
    }

    public override void UpdateIfActive()
    {
        if (!Config.GetLinkedAccountApiKey().HasValue)
        {
            Push<NotLinkedAccountScreen>().Initialize();
            return;
        }

        if (Config.GetLinkedAppSecret(_platform).HasValue)
        {
            Pop(null);
        }
    }

    public override void Draw()
    {
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();
            GUILayout.Label(
                "You haven't linked any PatchKit app for " +
                _platform.ToDisplayString(),
                EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Fix it!", GUILayout.Width(100)))
            {
                Dispatch(() => OpenLinkScreen());
            }

            GUILayout.FlexibleSpace();
        }
        EditorGUILayout.EndHorizontal();
    }

    #endregion

    #region Data

    [SerializeField]
    private AppPlatform _platform;

    #endregion

    #region Logic

    public void Initialize(AppPlatform platform)
    {
        _platform = platform;
    }

    public override void OnActivatedFromTop(object result)
    {
    }

    private void OpenLinkScreen()
    {
        Push<ConnectAppScreen>().Initialize(_platform);
    }

    #endregion
}
}