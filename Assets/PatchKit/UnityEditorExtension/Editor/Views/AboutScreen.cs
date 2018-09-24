using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace PatchKit.UnityEditorExtension.Views
{
public class AboutScreen : Screen
{
    public AboutScreen([NotNull] Window window)
        : base(window)
    {
    }

    public override string Title
    {
        get { return "About"; }
    }

    public override Vector2 Size
    {
        get { return new Vector2(400, 400); }
    }

    public override void Initialize()
    {
    }

    public override void Draw()
    {
        GUILayout.Label("\n");
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();
            GUILayout.Label(
                (Texture) EditorGUIUtility.Load(
                    "PatchKit Unity Editor Extension Logo.png"),
                GUILayout.Height(150),
                GUILayout.Width(307));
            GUILayout.FlexibleSpace();
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Label("About", EditorStyles.boldLabel);
        GUILayout.Label(
            "The PatchKit Unity Editor Extension is a Unity tool to automatic\n" +
            "application distribution directly from the Unity Editor.\n\n" +
            "Provide fast, comfortable and safe connection with API PatchKit.\n" +
            "The Extension allows uploading without leaving the editor, all you\n" +
            "have to do is log in and make the build. After that, version is\n" +
            "being sent to PatchKit.",
            GUILayout.Width(380));

        EditorGUILayout.Separator();
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginVertical();
            {
                if (GUILayout.Button("Contact", GUILayout.Width(170)))
                {
                    Application.OpenURL(
                        "http://docs.patchkit.net/contact.html");
                }

                if (GUILayout.Button("Documentation", GUILayout.Width(170)))
                {
                    Application.OpenURL("http://docs.patchkit.net/");
                }
            }
            EditorGUILayout.EndVertical();
            GUILayout.FlexibleSpace();
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Label("\nVersion 1.0.0", EditorStyles.centeredGreyMiniLabel);
    }

    public override bool ShouldBePopped()
    {
        return false;
    }
}
}