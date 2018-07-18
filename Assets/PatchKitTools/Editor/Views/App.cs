﻿using UnityEngine;
using UnityEditor;
using AppData = PatchKit.Api.Models.Main.App;

namespace PatchKit.Tools.Integration.Views
{
    public class App : IView
    {
        private readonly AppData _data;

        public AppData Data
        {
            get
            {
                return _data;
            }
        }

        public App(AppData data)
        {
            _data = data;
        }

        private bool _isCollapsed = true;

        public void Show()
        {
            var displayName = _data.Name;
            EditorGUILayout.BeginVertical();
                GUILayout.Label(displayName, EditorStyles.boldLabel);
                DisplayValueIfPresent("Platform: ", _data.Platform);
            EditorGUILayout.EndVertical();
            //if (_isCollapsed)
            //{
            //    if (GUILayout.Button("Details ↓", GUILayout.MinWidth(200), GUILayout.MaxWidth(400)))
            //    {
            //        _isCollapsed = false;
            //    }
            //}
            //else
            //{
            //    if (GUILayout.Button("Details ↑", GUILayout.MinWidth(200), GUILayout.MaxWidth(400)))
            //    {
            //        _isCollapsed = true;
            //    }

            //    DisplayValueIfPresent("Display name:", _data.DisplayName);
            //    DisplayValueIfPresent("Author:", _data.Author);

            //    GUILayout.Label("Secret:");
            //    EditorGUILayout.SelectableLabel(_data.Secret, EditorStyles.helpBox);
            //}
        }

        private void DisplayValueIfPresent(string label, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                GUILayout.Label(label + " " + value);
            }
        }
    }
}