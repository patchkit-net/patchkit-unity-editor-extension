using System;
using System.Collections.Generic;
using System.Linq;
using PatchKit.UnityEditorExtension.Core;
using PatchKit.UnityEditorExtension.Logic;
using UnityEditor;
using UnityEngine;
using AppData = PatchKit.Api.Models.Main.App;
using Environment = PatchKit.UnityEditorExtension.Core.Environment;

namespace PatchKit.UnityEditorExtension.Views
{
public class SelectApp : IView
{
    private List<Views.App> _appViews;
    private bool _shouldFilterByPlatform = true;

    public bool ShouldFilterByPlatform
    {
        get { return _shouldFilterByPlatform; }

        private set
        {
            if (value != _shouldFilterByPlatform)
            {
                _shouldFilterByPlatform = value;
                Reload();
            }
        }
    }

    public SelectApp()
    {
        Reload();
    }

    private Vector2 _scrollViewVector = Vector2.zero;

    private void Reload()
    {
        var apps = Core.Api.GetAppsCached();

        if (apps == null)
        {
            return;
        }

        var buildTargetName =
            Environment.BuildPlatform.Value;

        _appViews = apps
            .Where(
                app => !ShouldFilterByPlatform ||
                    (app.Platform == buildTargetName.ToApiString()))
            .Select(app => new Views.App(app))
            .ToList();
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
                "\n Choose target PatchKit application from the list below. \n",
                EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
        }
        EditorGUILayout.EndHorizontal();

        ShouldFilterByPlatform = EditorGUILayout.ToggleLeft(
            string.Format("Show only PatchKit apps for current build target ({0})", Environment.BuildPlatform.Value.ToDisplayString()),
            ShouldFilterByPlatform, GUILayout.ExpandWidth(true));

        //TODO: Maybe there's a way to detect it
        /*if (_api == null)
        {
            EditorGUILayout.HelpBox(
                "Cannot resolve connection to API",
                MessageType.Error);
            return;
        }*/

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        GUILayout.BeginVertical("box");
        {
            _scrollViewVector =
                EditorGUILayout.BeginScrollView(_scrollViewVector);

            if (_appViews != null)
            {
                _appViews.ForEach(
                    app =>
                    {
                        EditorGUILayout.BeginHorizontal();
                        app.Show();

                        if (GUILayout.Button("Select", GUILayout.Width(100)))
                        {
                            if (OnAppSelected != null)
                            {
                                OnAppSelected(app.Data);
                            }
                        }

                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.LabelField(
                            "",
                            GUI.skin.horizontalSlider);
                    });
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "Failed to load any apps, your api key might be invalid.",
                    MessageType.Warning);
            }

            EditorGUILayout.EndScrollView();
        }
        GUILayout.EndVertical();
    }

    public event Action<AppData> OnAppSelected;
    public event Action OnChangeApp;
}
}