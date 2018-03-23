using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using AppData = PatchKit.Api.Models.Main.App;

namespace PatchKit.Tools.Integration.Views
{
    public class SelectApp : IView
    {
        private readonly ApiUtils _api;

        private List<Views.App> _appViews;

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
            GUILayout.Button("Add");
        }

        public event Action<AppData> OnAppSelected;
    }
}