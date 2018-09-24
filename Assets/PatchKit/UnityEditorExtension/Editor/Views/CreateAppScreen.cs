using System;
using JetBrains.Annotations;
using PatchKit.UnityEditorExtension.Core;
using UnityEditor;
using UnityEngine;

namespace PatchKit.UnityEditorExtension.Views
{
public class CreateAppScreen : Screen
{
    public AppPlatform Platform { get; private set; }

    [NotNull]
    public Action<CreateAppScreen, Api.Models.Main.App> OnCreate
    {
        get;
        private set;
    }

    [NotNull]
    public Action<CreateAppScreen> OnCancel { get; private set; }

    [NotNull]
    private readonly CreateAppMediator _mediator;

    public CreateAppScreen(
        AppPlatform platform,
        [NotNull] Action<CreateAppScreen, Api.Models.Main.App> onCreate,
        [NotNull] Action<CreateAppScreen> onCancel,
        [NotNull] Window window)
        : base(window)
    {
        if (onCreate == null)
        {
            throw new ArgumentNullException("onCreate");
        }

        if (onCancel == null)
        {
            throw new ArgumentNullException("onCancel");
        }

        Platform = platform;
        OnCreate = onCreate;
        OnCancel = onCancel;

        _mediator = new CreateAppMediator(this);
    }

    public override string Title
    {
        get { return "Create app"; }
    }

    public override Vector2 Size
    {
        get { return new Vector2(400f, 150f); }
    }

    public override void Initialize()
    {
        _mediator.Initialize();
    }

    public override bool ShouldBePopped()
    {
        return _mediator.ShouldBePopped();
    }

    public override void Draw()
    {
        GUILayout.Label(
            string.Format(
                "Create new PatchKit app for {0}",
                Platform.ToDisplayString()),
            EditorStyles.boldLabel);

        _mediator.Name = EditorGUILayout.TextField("Name: ", _mediator.Name);

        EditorGUILayout.Space();

        if (string.IsNullOrEmpty(_mediator.Name))
        {
            EditorGUILayout.HelpBox(
                "Please insert your application name",
                MessageType.Info);
        }
        else
        {
            if (_mediator.NameValidationError != null)
            {
                EditorGUILayout.HelpBox(
                    _mediator.NameValidationError,
                    MessageType.Error);
            }
            else
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();

                    using (Style.Colorify(Color.green))
                    {
                        if (GUILayout.Button("Create", GUILayout.Width(100)))
                        {
                            Dispatch(() => _mediator.Create());
                        }
                    }

                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Cancel", GUILayout.Width(100)))
            {
                Dispatch(() => _mediator.Cancel());
            }

            GUILayout.FlexibleSpace();
        }
        GUILayout.EndHorizontal();
    }
}
}