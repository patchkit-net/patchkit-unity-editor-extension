using System;
using System.IO;
using System.Linq;
using PatchKit.Api.Models.Main;
using UnityEditor;
using UnityEngine;

namespace PatchKit.Tools.Integration.Views
{
    public class BuildApp : IView
    {
        
        private Api.Models.Main.App? _selectedApp; 
        public BuildApp(Api.Models.Main.App? selectedApp1)
        {
            _selectedApp = selectedApp1;
        }

        private bool _buildExecuted = false;
        private Api.Models.Main.App? _selectedApp1;

        public void Show()
        {
            string errorMessage = null;

            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            var scenes = EditorBuildSettings.scenes.Select(s => s.path).ToArray();
            var buildLocation = EditorUserBuildSettings.GetBuildLocation(buildTarget);

            if (buildTarget == BuildTarget.StandaloneLinuxUniversal || buildTarget == BuildTarget.StandaloneOSXIntel64)
            {
                OnFailure("Unsupported build target.");
                return;
            }

            if (string.IsNullOrEmpty(buildLocation))
            {
                buildLocation = EditorUtility.SaveFilePanel("Select build location:", "", "", ""); //to odpaliæ
                EditorUserBuildSettings.SetBuildLocation(buildTarget, buildLocation);
                return;
            }

            bool buildDirectoryExists = Directory.Exists(Path.GetDirectoryName(buildLocation));
            bool buildExists = File.Exists(buildLocation);

            GUILayout.Label(_selectedApp.Value.Name, EditorStyles.centeredGreyMiniLabel);

            GUILayout.Label("The project will be built with the following settings.", EditorStyles.boldLabel);
            GUILayout.Label("Target: " + buildTarget.ToString());
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label(new GUIContent("Location: " + buildLocation, buildLocation) , GUILayout.MaxWidth(300));
                if(GUILayout.Button(new GUIContent("Change", "Change build location")))
                {
                    buildLocation = EditorUtility.SaveFilePanel("Select build location:", "", "", "");
                    EditorUserBuildSettings.SetBuildLocation(buildTarget, buildLocation);
                }
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Label("Scenes: ", EditorStyles.boldLabel);
            
            for (int i = 0; i < scenes.Length; i++)
            {
                GUILayout.Label(i + ". " + scenes[i]);
            }
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            {
                if (!_buildExecuted && GUILayout.Button(new GUIContent("Build", "Build new version")))
                {
                    UnityEngine.Debug.Log("Bulding the player");
                    errorMessage = BuildPipeline.BuildPlayer(scenes, buildLocation, buildTarget, BuildOptions.None);

                    _buildExecuted = true;

                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        if (OnFailure != null) OnFailure(errorMessage);
                    }
                    else
                    {
                        if (OnSuccess != null) OnSuccess();
                    }
                }

                if (buildDirectoryExists && buildExists && GUILayout.Button(new GUIContent("Skip", "Use last build")))
                {
                    if (OnSuccess != null) OnSuccess();
                }
            }
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Change application"))
            {
                if (OnChangeApp != null) OnChangeApp();
            }
        }

        public event Action OnSuccess;
        public event Action<string> OnFailure;
        public event Action OnChangeApp;
    }
}