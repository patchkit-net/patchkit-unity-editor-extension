using System.Linq;
using JetBrains.Annotations;
using PatchKit.UnityEditorExtension.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace PatchKit.UnityEditorExtension.Views
{
public class SafeBuildAndUploadView : View
{
    [NotNull]
    private readonly SafeBuildAndUploadMediator _mediator;

    private Vector2 _scrollViewVector;

    public SafeBuildAndUploadView([NotNull] Window window)
        : base(window)
    {
        _mediator = new SafeBuildAndUploadMediator(this);
    }

    public override void Initialize()
    {
        _mediator.Initialize();
    }

    public override void Draw()
    {
        _scrollViewVector = EditorGUILayout.BeginScrollView(_scrollViewVector);
        {
            DrawContent();
        }
        EditorGUILayout.EndScrollView();
    }

    private void DrawContent()
    {
        Assert.IsNotNull(GUI.skin);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("App", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("Name:");

                GUILayout.FlexibleSpace();

                GUILayout.Label(
                    _mediator.LinkedAppName,
                    EditorStyles.miniLabel,
                    GUILayout.ExpandWidth(true));
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("Secret:");

                GUILayout.FlexibleSpace();

                GUILayout.Label(
                    _mediator.LinkedAppSecret,
                    EditorStyles.miniLabel,
                    GUILayout.ExpandWidth(true));
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("Platform:");

                GUILayout.FlexibleSpace();

                GUILayout.Label(
                    _mediator.Platform.ToDisplayString(),
                    EditorStyles.miniLabel,
                    GUILayout.ExpandWidth(true));
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            if (GUILayout.Button(
                new GUIContent("Switch app"),
                GUILayout.ExpandWidth(true)))
            {
                Dispatch(() => _mediator.SwitchLinkedApp());
            }

            if (GUILayout.Button(
                new GUIContent("Switch platform"),
                GUILayout.ExpandWidth(true)))
            {
                Dispatch(() => _mediator.SwitchPlatform());
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("Build Settings", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("Location:");

                GUILayout.FlexibleSpace();

                GUILayout.Label(
                    new GUIContent(
                        _mediator.BuildLocation,
                        _mediator.BuildLocation),
                    EditorStyles.miniLabel,
                    GUILayout.ExpandWidth(true));
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("Scenes: ", EditorStyles.boldLabel);
                if (GUILayout.Button(
                    new GUIContent(
                        "Edit Scenes",
                        "Button open Build Settings window."),
                    GUILayout.Width(120)))
                {
                    Dispatch(() => _mediator.SwitchScenes());
                }
            }
            EditorGUILayout.EndHorizontal();

            string[] scenes = AppBuild.Scenes.ToArray();

            for (int i = 0; i < scenes.Length; i++)
            {
                GUILayout.Label(i + ". " + scenes[i]);
            }

            if (GUILayout.Button(
                new GUIContent("Change location"),
                GUILayout.ExpandWidth(true)))
            {
                Dispatch(() => _mediator.ChangeBuildLocation());
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label(
                    "Version Configuration",
                    EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            GUILayout.Label("*Label: ");
            _mediator.VersionLabel =
                GUILayout.TextField(_mediator.VersionLabel);

            EditorGUILayout.Space();

            GUILayout.Label("Changelog: ");
            _mediator.VersionChangelog = GUILayout.TextArea(
                _mediator.VersionChangelog,
                GUILayout.MinHeight(200));
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("Automatically publish after upload");
            _mediator.PublishOnUpload =
                EditorGUILayout.Toggle(_mediator.PublishOnUpload);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("Overwrite draft version if it exists");
            _mediator.OverwriteDraftVersion =
                EditorGUILayout.Toggle(_mediator.OverwriteDraftVersion);
        }
        EditorGUILayout.EndHorizontal();

        if (!_mediator.OverwriteDraftVersion)
        {
            EditorGUILayout.HelpBox(
                "If a draft version exists, interaction with the console will be necessary.",
                MessageType.Warning);
        }

        if (!_mediator.IsBuildLocationSelected)
        {
            EditorGUILayout.HelpBox(
                "You haven't selected build location.",
                MessageType.Error);
        }
        else if (_mediator.VersionLabelValidationError != null)
        {
            EditorGUILayout.HelpBox(
                _mediator.VersionLabelValidationError,
                MessageType.Error);
        }
        else if (_mediator.VersionChangelogValidationError != null)
        {
            EditorGUILayout.HelpBox(
                _mediator.VersionChangelogValidationError,
                MessageType.Error);
        }
        else
        {
            using (Style.Colorify(Color.green))
            {
                if (GUILayout.Button(
                    new GUIContent("Build & Upload", "Build a new version"),
                    GUILayout.ExpandWidth(true)))
                {
                    Dispatch(() => _mediator.BuildAndUpload());
                }
            }
        }

        EditorGUILayout.Space();
    }
}
}