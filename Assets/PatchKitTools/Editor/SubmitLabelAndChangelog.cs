using System;
using UnityEditor;
using UnityEngine;

namespace PatchKit.Tools.Integration
{
    public class SubmitLabelAndChangelog : EditorWindow
    {
        private string _label = "Label...";
        private string _changelog = "Changelog...\n...\n...";

        private bool _invalidSubmit = false;
        private bool _submit = false;

        void OnGUI()
        {
            _submit = false;
            
            GUILayout.Label("Enter your version details.");
            _label = EditorGUILayout.TextField("Version label:", _label);

            GUILayout.Label("Changelog: ", EditorStyles.boldLabel);
            _changelog = EditorGUILayout.TextArea(_changelog);
            
            if (GUILayout.Button("Submit"))
            {
                _submit = true;

                if (string.IsNullOrEmpty(_label))
                {
                    _invalidSubmit = true;
                }
                else
                {
                    _invalidSubmit = false;

                    if (OnResolve != null) OnResolve(_label, _changelog);

                    this.Close();
                }
            }

            if (string.IsNullOrEmpty(_label))
            {
                EditorGUILayout.HelpBox("Version label cannot be empty.", MessageType.Error);
            }
        }

        public event Action<string, string> OnResolve;
    }
}