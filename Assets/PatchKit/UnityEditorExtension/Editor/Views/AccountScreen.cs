using JetBrains.Annotations;
using PatchKit.UnityEditorExtension.Core;
using UnityEditor;
using UnityEngine;

namespace PatchKit.UnityEditorExtension.Views
{
public class AccountScreen : Screen
{
    [NotNull]
    private readonly AccountMediator _mediator;

    [NotNull]
    private readonly NotLinkedAccountView _notLinkedAccountView;

    public AccountScreen([NotNull] Window window)
        : base(window)
    {
        _mediator = new AccountMediator(this);
        _notLinkedAccountView = new NotLinkedAccountView(window);
    }

    public override string Title
    {
        get { return "Account"; }
    }

    public override Vector2 Size
    {
        get { return new Vector2(400, 145); }
    }

    public override void Initialize()
    {
        _mediator.Initialize();

        _notLinkedAccountView.Initialize();
    }

    public override bool ShouldBePopped()
    {
        return false;
    }

    public override void Draw()
    {
        if (!_mediator.IsAccountLinked)
        {
            _notLinkedAccountView.Draw();

            return;
        }

        GUILayout.Label(
            "You have successfully linked your PatchKit account.",
            EditorStyles.boldLabel);

        GUILayout.Label("API key:");
        EditorGUILayout.SelectableLabel(_mediator.ApiKey, EditorStyles.helpBox);

        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Change", GUILayout.Width(100)))
            {
                Dispatch(() => _mediator.OpenLinkDialog());
            }

            GUILayout.FlexibleSpace();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();

        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Unlink", GUILayout.Width(100)))
            {
                Dispatch(() => _mediator.Unlink());
            }

            GUILayout.FlexibleSpace();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();
    }
}
}