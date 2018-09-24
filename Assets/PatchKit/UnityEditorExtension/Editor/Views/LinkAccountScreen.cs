using System;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace PatchKit.UnityEditorExtension.Views
{
public class LinkAccountScreen : Screen
{
    [NotNull]
    private readonly LinkAccountMediator _mediator;

    [NotNull]
    public Action<LinkAccountScreen> OnLinked { get; private set; }

    [NotNull]
    public Action<LinkAccountScreen> OnCanceled { get; private set; }

    public LinkAccountScreen(
        [NotNull] Action<LinkAccountScreen> onLinked,
        [NotNull] Action<LinkAccountScreen> onCanceled,
        [NotNull] Window window)
        : base(window)
    {
        if (onLinked == null)
        {
            throw new ArgumentNullException("onLinked");
        }

        if (onCanceled == null)
        {
            throw new ArgumentNullException("onCanceled");
        }

        OnLinked = onLinked;
        OnCanceled = onCanceled;

        _mediator = new LinkAccountMediator(this);
    }

    public override string Title
    {
        get { return "Link Account"; }
    }

    public override Vector2 Size
    {
        get { return new Vector2(400f, 145f); }
    }

    public override void Initialize()
    {
        _mediator.Initialize();
    }

    public override bool ShouldBePopped()
    {
        return false;
    }

    public override void Draw()
    {
        GUILayout.Label("Link your PatchKit account", EditorStyles.boldLabel);
        _mediator.NewApiKey = EditorGUILayout.TextField(
            "API Key:",
            _mediator.NewApiKey);

        if (GUILayout.Button("Find your API key", GUILayout.ExpandWidth(true)))
        {
            Dispatch(() => _mediator.OpenProfileWebpage());
        }

        if (!string.IsNullOrEmpty(_mediator.NewApiKey))
        {
            if (_mediator.NewApiKeyValidationError != null)
            {
                EditorGUILayout.HelpBox(
                    "Invalid key: " + _mediator.NewApiKeyValidationError,
                    MessageType.Error);
            }
            else
            {
                EditorGUILayout.Separator();
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Submit", GUILayout.Width(100)))
                    {
                        Dispatch(() => _mediator.Link());
                    }

                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        else
        {
            EditorGUILayout.HelpBox(
                "Please enter your API key to link your PatchKit account with extension.",
                MessageType.Info);
        }

        EditorGUILayout.Separator();
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Cancel", GUILayout.Width(100)))
            {
                Dispatch(() => _mediator.Cancel());
            }

            GUILayout.FlexibleSpace();
        }
        EditorGUILayout.EndHorizontal();
    }
}
}