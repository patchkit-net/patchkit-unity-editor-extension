using System;
using System.Collections.Generic;
using System.Linq;
using PatchKit.UnityEditorExtension.Core;
using PatchKit.UnityEditorExtension.Tools;
using UnityEditor;
using UnityEngine;
using AppData = PatchKit.Api.Models.Main.App;
using Environment = PatchKit.UnityEditorExtension.Core.Environment;

namespace PatchKit.UnityEditorExtension.Views
{
public class CreateApp : IView
{
    private string _newAppName = "NewApp";

    private bool FindDuplicateApp(string name)
    {
        List<AppData> api = Core.Api.GetApps().ToList();
        return api.Any(app => app.Name == name);
    }

    public void Show()
    {
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
                "\n Create new application as a target.\n",
                EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
        }
        EditorGUILayout.EndHorizontal();

        _newAppName = EditorGUILayout.TextField("Name: ", _newAppName);
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField(
                new GUIContent(
                    "Build target platform:      " +
                    AppBuild.Platform.Value.ToDisplayString(),
                    "PatchKit application target platform is unambiguous with current active project build platform."));
            if (GUILayout.Button(
                new GUIContent(
                    "Change",
                    "Change PatchKit application target platform"),
                GUILayout.Width(60)))
            {
                if (EditorUtility.DisplayDialog(
                    "Warning",
                    "To change application platform, you need to switch the project platform. \n\nRemember, PatchKit support only: Windows, Mac, Linux platforms.",
                    "Change the project platform",
                    "Cancel"))
                {
                    EditorWindow.GetWindow(
                        System.Type.GetType(
                            "UnityEditor.BuildPlayerWindow,UnityEditor"));
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        if (string.IsNullOrEmpty(_newAppName))
        {
            EditorGUILayout.HelpBox(
                "Application name cannot be empty.",
                MessageType.Error);
        }
        else
        {
            if (!CharactersValidator.Validate(_newAppName))
            {
                EditorGUILayout.HelpBox(
                    "Name only allows English characters and ':', '_' or '-'",
                    MessageType.Error);
            }
            else
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Add", GUILayout.Width(100)))
                    {
                        if (FindDuplicateApp(_newAppName))
                        {
                            EditorUtility.DisplayDialog(
                                "Warning",
                                "Application name is already taken.\nChange it and try again.",
                                "Ok");
                        }
                        else
                        {
                            var newApp = Core.Api.CreateNewApp(
                                _newAppName,
                                AppBuild.Platform.Value);
                            if (OnAppSelected != null)
                            {
                                OnAppSelected(newApp);
                            }
                        }
                    }

                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
            }
        }
    }

    public event Action<AppData> OnAppSelected;
    public event Action OnChangeApp;
}
}