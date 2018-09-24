using System;
using JetBrains.Annotations;
using PatchKit.UnityEditorExtension.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace PatchKit.UnityEditorExtension.Views
{
public class LinkAppScreen : Screen
{
    public AppPlatform Platform { get; private set; }

    [NotNull]
    public Action<LinkAppScreen, Api.Models.Main.App> OnLinked { get; private set; }

    [NotNull]
    public Action<LinkAppScreen> OnCanceled { get; private set; }

    [NotNull]
    private readonly LinkAppMediator _mediator;

    private Vector2 _scrollViewVector;

    public LinkAppScreen(
        AppPlatform platform,
        [NotNull] Action<LinkAppScreen, Api.Models.Main.App> onLinked,
        [NotNull] Action<LinkAppScreen> onCanceled,
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

        Platform = platform;
        OnLinked = onLinked;
        OnCanceled = onCanceled;

        _mediator = new LinkAppMediator(this);
    }

    public override string Title
    {
        get { return "Link App"; }
    }

    public override Vector2 Size
    {
        get { return new Vector2(400f, 400f); }
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
                "Link your PatchKit app for {0}",
                Platform.ToDisplayString()),
            EditorStyles.boldLabel);

        EditorGUILayout.Space();

        _scrollViewVector = EditorGUILayout.BeginScrollView(_scrollViewVector);

        foreach (Api.Models.Main.App app in _mediator.Apps)
        {
            DrawApp(app);
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();

        if (GUILayout.Button("Create new app", GUILayout.ExpandWidth(true)))
        {
            Dispatch(() => _mediator.CreateNew());
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Cancel", GUILayout.ExpandWidth(true)))
        {
            Dispatch(() => _mediator.Cancel());
        }
    }

    private void DrawApp(Api.Models.Main.App app)
    {
        Assert.IsNotNull(app.Name);
        Assert.IsNotNull(app.Platform);

        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

        EditorGUILayout.BeginVertical();
        {
            if (app.Name.Length > 30)
            {
                string shortName = app.Name.Substring(0, 30);
                shortName += "...";
                GUILayout.Label(
                    new GUIContent(shortName, app.Name),
                    EditorStyles.boldLabel);
            }
            else
            {
                GUILayout.Label(app.Name, EditorStyles.largeLabel);
            }

            if (app.Secret == _mediator.LinkedAppSecret)
            {
                GUILayout.Label("Currently linked app", EditorStyles.miniLabel);
            }
            else
            {
                if (GUILayout.Button("Select", GUILayout.ExpandWidth(true)))
                {
                    Dispatch(() => _mediator.Link(app));
                }
            }

            EditorGUILayout.Space();
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        Assert.IsNotNull(GUI.skin);

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }
}
}