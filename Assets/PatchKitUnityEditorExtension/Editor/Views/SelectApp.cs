using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using AppData = PatchKit.Api.Models.Main.App;
using UnityEngine.UI;

namespace PatchKit.Tools.Integration.Views
{
    public class SelectApp : IView
    {
        private readonly ApiUtils _api;
        private List<Views.App> _appViews;
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
            
            if (GUILayout.Button(new GUIContent("←", "Change application"), GUILayout.Width(40)))
            {
                if (OnChangeApp != null)
                {
                    OnChangeApp();
                }
            }

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("\n Choose target PatchKit application from the list below. \n", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();

            ShouldFilterByPlatform = EditorGUILayout.Toggle("Filter apps by platform", ShouldFilterByPlatform);
            if (_api == null)
            {
                EditorGUILayout.HelpBox("Cannot resolve connection to API", MessageType.Error);
                return;
            }

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            GUILayout.BeginVertical("box");
            {
                _scrollViewVector = EditorGUILayout.BeginScrollView(_scrollViewVector);

                if (_appViews != null)
                {
                    _appViews.ForEach(app => {
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
                        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    });
                }
                else
                {
                    EditorGUILayout.HelpBox("Failed to load any apps, your api key might be invalid.", MessageType.Warning);
                }

                EditorGUILayout.EndScrollView();
            }
            GUILayout.EndVertical();
        }

        public event Action<AppData> OnAppSelected;
        public event Action OnChangeApp;
    }
}