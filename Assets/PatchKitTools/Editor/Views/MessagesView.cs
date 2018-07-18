
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace PatchKit.Tools.Integration.Views
{

    public class MessagesView : IView
    {
        public List<Message> messages = new List<Message>();
        public void AddMessage(string message, MessageType messageType)
        {
            messages.Add(new Message(message, messageType));
        }
        public void Show()
        {
            for(int i=0; i< messages.Count; i++)
            {
                messages[i].Show();
            }
        }
    }
}

