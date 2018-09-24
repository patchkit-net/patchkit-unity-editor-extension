using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace PatchKit.UnityEditorExtension.Views
{
public class NotLinkedAccountView : View
{
    [NotNull]
    private readonly NotLinkedAccountMediator _mediator;

    public NotLinkedAccountView([NotNull] Window window)
        : base(window)
    {
        _mediator = new NotLinkedAccountMediator(this);
    }

    public override void Initialize()
    {
    }

    public override void Draw()
    {
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();
            GUILayout.Label(
                "You haven't linked your PatchKit account yet.",
                EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Fix it!", GUILayout.Width(100)))
            {
                Dispatch(() => _mediator.OpenLinkDialog());
            }

            GUILayout.FlexibleSpace();
        }
        EditorGUILayout.EndHorizontal();
    }
}
}