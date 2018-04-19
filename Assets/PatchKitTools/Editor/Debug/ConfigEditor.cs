using System.IO;
using UnityEditor;
using UnityEngine;

namespace PatchKit.Tools.Integration.Debug
{
    [CustomEditor(typeof(Config))]
    public class ConfigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var config = target as Config;

            DrawDefaultInspector();
            
            EditorGUILayout.Separator();
            GUILayout.Label("Debug options:");

            if (GUILayout.Button("Reset build settings"))
            {
                EditorUserBuildSettings.SetBuildLocation(EditorUserBuildSettings.activeBuildTarget, "");
            }

            if (GUILayout.Button("Delete appcache"))
            {
                File.Delete(config.LocalCachePath);
            }
        }
    }
}