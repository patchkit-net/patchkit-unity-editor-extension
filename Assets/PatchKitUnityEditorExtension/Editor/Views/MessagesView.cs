
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace PatchKit.Tools.Integration.Views
{

    public class MessagesView : IView
    {
        private List<Message> _messages = new List<Message>();
        public void ClearList()
        {
            _messages.Clear();
        }

        public void AddMessage(string message, MessageType messageType)
        {
            _messages.Add(new Message(message, messageType));
        }

        public void Show()
        {
            for(int i = 0 ; i < _messages.Count; i++)
            {
                _messages[i].Show();
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Close and restart", "Change application"), GUILayout.Width(150)))
            {
                if (OnChangeApp != null) OnChangeApp();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        public event System.Action OnChangeApp;
    }
}

