using System;
using UnityEditor;
using UnityEngine;

namespace PatchKit.Tools.Integration.Views
{
    public class Message : IView
    {
        private readonly string _message;
        private readonly MessageType _messageType;

        public Message(string message, MessageType messageType)
        {
            _message = message;
            _messageType = messageType;
        }

        public void Show()
        {
            EditorGUILayout.HelpBox(_message, _messageType);
        }
    }
}