using JetBrains.Annotations;
using PatchKit.UnityEditorExtension.Core;
using UnityEditor;
using UnityEngine;

namespace PatchKit.UnityEditorExtension.Views
{
public class NotLinkedAppView : View
{
    [NotNull]
    private readonly NotLinkedAppMediator _mediator;

    public AppPlatform Platform { get; private set; }

    public NotLinkedAppView(AppPlatform platform, [NotNull] Window window)
        : base(window)
    {
        Platform = platform;
        _mediator = new NotLinkedAppMediator(this);
    }

    public override void Initialize()
    {
    }

    public override void Draw()
    {
        GUILayout.Label(
            "You haven't linked any PatchKit app for " +
            Platform.ToDisplayString(),
            EditorStyles.boldLabel);

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