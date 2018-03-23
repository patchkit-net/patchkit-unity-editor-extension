using System;
using UnityEditor;
using UnityEngine;

using AppData = PatchKit.Api.Models.Main.App;

namespace PatchKit.Tools.Integration.Views
{
    public class SelectApp : IView
    {
        private readonly ApiUtils _api;

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

            var apps = _api.GetAppsCached(); 

            if (apps == null)
            {
                return;
            }

            apps.ForEach(app => {
                GUILayout.Label("Name: " + app.Name);
                if (!string.IsNullOrEmpty(app.DisplayName))
                {
                    GUILayout.Label("Disp. name: " + app.DisplayName);
                }

                if (GUILayout.Button("Select"))
                {
                    OnAppSelected(app);
                }
            });

            GUILayout.Label("New app: ", EditorStyles.boldLabel);
            GUILayout.Button("Add");
        }

        public event Action<AppData> OnAppSelected;
    }
}