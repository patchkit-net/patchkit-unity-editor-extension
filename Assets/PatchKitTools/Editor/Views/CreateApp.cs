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

        public CreateApp(ApiUtils api)
        {
            _api = api;
        }

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
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("\n Create new application as a target.\n", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
            }
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
                if (!TextValidation.DoesStringContainOnlyAllowedCharacters(newAppName))
                {
                    EditorGUILayout.HelpBox("Name only allows english characters and ':', '_' or '-'", MessageType.Error);
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    { 
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
                    }
                    GUILayout.EndHorizontal();
                }

            }
        }

        public event Action<AppData> OnAppSelected;
        public event Action OnChangeApp;

    }
}

