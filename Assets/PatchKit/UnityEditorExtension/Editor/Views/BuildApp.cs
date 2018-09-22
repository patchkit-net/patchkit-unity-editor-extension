using System;
using System.IO;
using System.Linq;
using PatchKit.UnityEditorExtension.Core;
using UnityEditor;
using UnityEngine;

namespace PatchKit.UnityEditorExtension.Views
{
public class BuildApp : IView
{
    private readonly Api.Models.Main.App _selectedApp;

    public BuildApp(Api.Models.Main.App selectedApp)
    {
        _selectedApp = selectedApp;
    }

    private bool _buildExecuted;

    public void Show()
    {
        string[] scenes = AppBuild.Scenes.ToArray();

        if (!AppBuild.Platform.HasValue)
        {
            OnFailure("Unsupported build target.");
            return;
        }

        if (string.IsNullOrEmpty(AppBuild.Location))
        {
            AppBuild.OpenLocationDialog();
            return;
        }

        GUILayout.Label(_selectedApp.Name, EditorStyles.centeredGreyMiniLabel);

        if (GUILayout.Button(
            new GUIContent("←", "Change application"),
            GUILayout.Width(40)))
        {
            if (OnChangeApp != null)
            {
                OnChangeApp();
            }
        }

        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();
            GUILayout.Label(
                "\n                               * Building *\n" +
                " The project will be built with the following settings. \n",
                EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Label("Target: " + AppBuild.Platform.Value.ToDisplayString());

        GUILayout.Label(
            new GUIContent("Location: " + AppBuild.Location, AppBuild.Location),
            GUILayout.ExpandWidth(true));
        if (GUILayout.Button(
            new GUIContent("Change location", "Change build location"),
            GUILayout.ExpandWidth(true)))
        {
            AppBuild.OpenLocationDialog();
        }

        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("Scenes: ", EditorStyles.boldLabel);
            if (GUILayout.Button(
                new GUIContent(
                    "Edit Scenes",
                    "Button open Build Settings window."),
                GUILayout.Width(120)))
            {
                EditorWindow.GetWindow(
                    Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
            }
        }
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < scenes.Length; i++)
        {
            GUILayout.Label(i + ". " + scenes[i]);
        }

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();
            if (!_buildExecuted &&
                GUILayout.Button(
                    new GUIContent("Build", "Build a new version"),
                    GUILayout.Width(150)))
            {
                Debug.Log("Building the player");
                string errorMessage = AppBuild.Create();

                _buildExecuted = true;

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    if (OnFailure != null)
                    {
                        OnFailure(errorMessage);
                    }
                }
                else
                {
                    if (OnSuccess != null)
                    {
                        OnSuccess();
                    }
                }
            }

            GUILayout.FlexibleSpace();
        }
        EditorGUILayout.EndHorizontal();
    }

    public event Action OnSuccess;
    public event Action<string> OnFailure;
    public event Action OnChangeApp;
}
}