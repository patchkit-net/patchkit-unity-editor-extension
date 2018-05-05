using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PatchKit.Tools.Integration.Views
{
    public class BuildApp : IView
    {
        private bool _buildExecuted = false;

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
                buildLocation = EditorUtility.SaveFilePanel("Select build location:", "", "", "");
                EditorUserBuildSettings.SetBuildLocation(buildTarget, buildLocation);
                return;
            }

            bool buildDirectoryExists = Directory.Exists(Path.GetDirectoryName(buildLocation));
            bool buildExists = File.Exists(buildLocation);

            GUILayout.Label("The project will be built with the following settings.", EditorStyles.boldLabel);

            GUILayout.Label("Target: " + buildTarget.ToString());
            GUILayout.Label("Location: " + buildLocation);

            GUILayout.Label("Scenes: ", EditorStyles.boldLabel);
            
            for (int i = 0; i < scenes.Length; i++)
            {
                GUILayout.Label(i + ". " + scenes[i]);
            }

            if (!_buildExecuted && GUILayout.Button("Ok"))
            {
                GUILayout.Label("Building...");

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

            if (buildDirectoryExists && buildExists && GUILayout.Button("Skip"))
            {
                if (OnSuccess != null) OnSuccess();
            }
        }

        public event Action OnSuccess;
        public event Action<string> OnFailure;
    }
}