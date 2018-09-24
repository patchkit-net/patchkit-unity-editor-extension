using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace PatchKit.UnityEditorExtension.Views
{
public class UploadingInProgressScreen : Screen
{
    public UploadingInProgressScreen([NotNull] Window window)
        : base(window)
    {
    }

    public override void Initialize()
    {
    }

    public override void Draw()
    {
        GUILayout.Label(
            "Your app is being uploaded right now. " +
            "You can track the progress on terminal window.",
            EditorStyles.boldLabel);
    }

    public override string Title
    {
        get { return "Uploading"; }
    }

    public override Vector2 Size
    {
        get { return new Vector2(200f, 100f); }
    }

    public override bool ShouldBePopped()
    {
        return false;
    }
}
}