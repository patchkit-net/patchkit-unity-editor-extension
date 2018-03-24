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

        public void Show()
        {
            GUILayout.Label("Your apps: ", EditorStyles.boldLabel);

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
                    .Select(app => new Views.App(app))
                    .ToList();
            }

            _appViews.ForEach(app => {
                app.Show();
                if (GUILayout.Button("Select"))
                {
                    OnAppSelected(app.Data);
                }
            });

            GUILayout.Label("New app: ", EditorStyles.boldLabel);

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