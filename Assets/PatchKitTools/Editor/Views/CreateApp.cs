using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using AppData = PatchKit.Api.Models.Main.App;
using UnityEngine.UI;

namespace PatchKit.Tools.Integration.Views
{
    public class CreateApp : IView
    {
        private readonly ApiUtils _api;

        private List<Views.App> _appViews;

        private string newAppName = "NewApp";

        //  private bool _shouldFilterByPlatform = true;
        //public bool ShouldFilterByPlatform
        //{
        //    get
        //    {
        //        return _shouldFilterByPlatform;
        //    }

        //    private set
        //    {
        //        if (value != _shouldFilterByPlatform)
        //        {
        //            _shouldFilterByPlatform = value;
        //           // Reload();
        //        }
        //    }
        //}

        public CreateApp(ApiUtils api)
        {
            _api = api;

            // Reload();
        }

        //   private Vector2 _scrollViewVector = Vector2.zero;

        //private void Reload()
        //{
        //    var apps = _api.GetAppsCached();

        //    if (apps == null)
        //    {
        //        return;
        //    }

        //    var buildTargetName = EditorUserBuildSettings.activeBuildTarget.ToPatchKitString();

        //    _appViews = apps
        //            .Where(app => !ShouldFilterByPlatform || (app.Platform == buildTargetName))
        //            .Select(app => new Views.App(app))
        //            .ToList();
        //}

        private bool FindDuplicateApp(string name)
        {
            var api = _api.GetApps();
            for (int i = 0; i < api.Count; i++)
            {
                if (api[i].Name == name) return true;
            }
            return false;
        }
        public void Show()
        {
            if (GUILayout.Button(new GUIContent("←", "Change application"), GUILayout.Width(40)))
            {
                if (OnChangeApp != null) OnChangeApp();
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("\n Create new application as a target.\n", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            newAppName = EditorGUILayout.TextField("Name: ", newAppName);
            EditorGUILayout.BeginHorizontal();
            {
                var buildTargetName = EditorUserBuildSettings.activeBuildTarget;
                EditorGUILayout.LabelField(new GUIContent("Build target platform:      " + buildTargetName.ToString(), "PatchKit application target platform is unambiguous with current active project build platform."));
                if (GUILayout.Button(new GUIContent("Change", "Change PatchKit application target platform"), GUILayout.Width(60)))
                {
                    if (EditorUtility.DisplayDialog("Warning", "To change application platform, You need to switch project platform. \n\nRemember, PatchKit support only: Windows, Mac, Linux platforms.",
                           "Change project platform", "Cancel"))
                    {
                        EditorWindow.GetWindow(System.Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            if (string.IsNullOrEmpty(newAppName))
            {
                EditorGUILayout.HelpBox("Application name cannot be empty.", MessageType.Error);
            }
            else
            {
                if (!TextValidation.IsEnglish(newAppName))
                {
                    EditorGUILayout.HelpBox("Name only allows english characters and ':', '_' or '-'", MessageType.Error);
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();


                    if (GUILayout.Button("Add", GUILayout.Width(100)))
                    {
                        if (FindDuplicateApp(newAppName))
                        {
                            EditorUtility.DisplayDialog("Warning", "Application name is already taken.\nChange it and try again.", "Ok");
                        }
                        else
                        {
                            var newApp = _api.CreateNewApp(newAppName, EditorUserBuildSettings.activeBuildTarget.ToPatchKitString());
                            if (OnAppSelected != null) OnAppSelected(newApp);
                        }

                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }

            }

            //EditorGUILayout.Separator();

            //GUILayout.Label("Your apps: ", EditorStyles.boldLabel);

            //ShouldFilterByPlatform = EditorGUILayout.Toggle("Filter apps by platform", ShouldFilterByPlatform);
            //if (_api == null)
            //{
            //    EditorGUILayout.HelpBox("Cannot resolve connection to API", MessageType.Error);
            //    return;
            //}

            //EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            //_scrollViewVector = EditorGUILayout.BeginScrollView(_scrollViewVector);

            //if (_appViews != null)
            //{
            //    _appViews.ForEach(app => {

            //        app.Show();
            //        GUILayout.BeginHorizontal();
            //        GUILayout.FlexibleSpace();
            //        if (GUILayout.Button("Select", GUILayout.Width(100)))
            //        {
            //            if (OnAppSelected != null) OnAppSelected(app.Data);
            //        }
            //        GUILayout.FlexibleSpace();
            //        GUILayout.EndHorizontal();
            //    });
            //}
            //else
            //{
            //    EditorGUILayout.HelpBox("Failed to load any apps, maybe your api key is invalid.", MessageType.Warning);
            //}

            //EditorGUILayout.EndScrollView();
        }

        public event Action<AppData> OnAppSelected;
        public event Action OnChangeApp;

    }
}

