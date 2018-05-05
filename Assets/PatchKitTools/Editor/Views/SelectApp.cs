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

        private bool _shouldFilterByPlatform = true;
        public bool ShouldFilterByPlatform 
        {
            get {
                return _shouldFilterByPlatform;
            }

            private set {
                if (value != _shouldFilterByPlatform)
                {
                    _shouldFilterByPlatform = value;
                    Reload();
                }
            }
        }

        public SelectApp(ApiUtils api)
        {
            _api = api;

            Reload();
        }

        private Vector2 _scrollViewVector = Vector2.zero;

        private void Reload()
        {
            var apps = _api.GetAppsCached(); 

            if (apps == null)
            {
                return;
            }

            var buildTargetName = EditorUserBuildSettings.activeBuildTarget.ToPatchKitString();

            _appViews = apps
                    .Where(app => !ShouldFilterByPlatform || (app.Platform == buildTargetName))
                    .Select(app => new Views.App(app))
                    .ToList();
        }

        public void Show()
        {
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
                    if (OnAppSelected != null) OnAppSelected(newApp);
                }
            }
            
            EditorGUILayout.Separator();
            
            GUILayout.Label("Your apps: ", EditorStyles.boldLabel);

            ShouldFilterByPlatform = EditorGUILayout.Toggle("Filter apps by platform", ShouldFilterByPlatform);

            if (_api == null)
            {
                EditorGUILayout.HelpBox("Cannot resolve connection to API", MessageType.Error);
                return;
            }

            _scrollViewVector = EditorGUILayout.BeginScrollView(_scrollViewVector);

            _appViews.ForEach(app => {
                app.Show();
                if (GUILayout.Button("Select"))
                {
                    if (OnAppSelected != null) OnAppSelected(app.Data);
                }
            });

            EditorGUILayout.EndScrollView();
        }

        public event Action<AppData> OnAppSelected;
    }
}