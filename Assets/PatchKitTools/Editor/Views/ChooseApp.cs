using System;
using System.IO;
using PatchKit.Api.Models.Main;
using UnityEditor;
using UnityEngine;

namespace PatchKit.Tools.Integration.Views
{
    public class ChooseApp : IView
    {
        public void Show()
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("\nSelect target PatchKit application.\n", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            { 
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(new GUIContent("Choose Existing App", 
                    "Select a PatchKit application from the list of existing applications."), GUILayout.Width(150)))
                {
                    if (OnSelectApp != null)
                    {
                        OnSelectApp();
                    }
                }

                if (GUILayout.Button(new GUIContent("Create New App", "Create new PatchKit application."), GUILayout.Width(150)))
                {
                    if (OnCreateApp != null)
                    {
                        OnCreateApp();
                    }
                }
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();
        }

        public event Action OnCreateApp;
        public event Action OnSelectApp;
    }
}
