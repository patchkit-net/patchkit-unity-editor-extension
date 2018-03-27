using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using PatchKit.Network;

using AppData = PatchKit.Api.Models.Main.App;

namespace PatchKit.Tools.Integration.Views
{
    public class SelectApp : IView
    {
        private readonly ApiUtils _api;

        private List<Views.App> _appViews;

        private string newAppName = "NewApp";

        public SelectApp(ApiUtils api)
        {
            _api = api;
        }

        private Vector2 _scrollViewVector = Vector2.zero;

        public void Show()
        {
            GUILayout.Label("Your apps: ", EditorStyles.boldLabel);

            bool shouldFilterByPlatform = Config.instance().filterAppsByPlatform;
            var buildTargetName = EditorUserBuildSettings.activeBuildTarget.ToPatchKitString();

            if (_api == null)
            {
                EditorGUILayout.HelpBox("Cannot resolve connection to API", MessageType.Error);
                return;
            }

            if (_appViews == null)
            {
                var apps = _api.GetAppsCached(); 

                if (apps == null)
                {
                    return;
                }

                _appViews = apps
                    .Where(app => shouldFilterByPlatform ? (app.Platform == buildTargetName) : true)
                    .Select(app => new Views.App(app))
                    .ToList();
            }

            _scrollViewVector = EditorGUILayout.BeginScrollView(_scrollViewVector, GUILayout.Height(Config.instance().appsScrollViewHeight));

            _appViews.ForEach(app => {
                app.Show();
                if (GUILayout.Button("Select"))
                {
                    OnAppSelected(app.Data);
                }
            });

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("New app: ", EditorStyles.boldLabel);

            newAppName = EditorGUILayout.TextField("Name: ", newAppName);

            if (string.IsNullOrEmpty(newAppName))
            {
                EditorGUILayout.HelpBox("Application name cannot be empty.", MessageType.Error);
            }
            else
            {
                if (GUILayout.Button("Add"))
                {
                    var newApp = _api.CreateNewApp(newAppName, EditorUserBuildSettings.activeBuildTarget.ToPatchKitString());
                    OnAppSelected(newApp);
                }
            }
        }

        public event Action<AppData> OnAppSelected;
    }
}