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

            EditorGUILayout.LabelField("Cached apps:", EditorStyles.boldLabel);

            foreach (var entry in config.Cache.AppsByPlatform())
            {
                EditorGUILayout.LabelField(entry.Key.ToPatchKitString());
                EditorGUILayout.SelectableLabel(entry.Value, EditorStyles.helpBox);
            }

            if (GUILayout.Button("Reset build settings"))
            {
                EditorUserBuildSettings.SetBuildLocation(EditorUserBuildSettings.activeBuildTarget, "");
            }

            if (GUILayout.Button("Clear cached apps"))
            {
                config.Cache.Clear();
            }
        }
    }
}