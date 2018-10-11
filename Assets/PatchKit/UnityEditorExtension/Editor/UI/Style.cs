using System;
using UnityEngine;

namespace PatchKit.UnityEditorExtension.UI
{
public static class Style
{
    public static readonly Color greenPastel = new Color(0.502f, 0.839f, 0.839f);
    public static readonly Color greenOlive = new Color(0.502f, 0.839f, 0.031f);
    public static readonly Color redPastel = new Color(0.839f, 0.502f, 0.502f); 
    
    private class Disposable : IDisposable
    {
        private Action _action;

        public Disposable(Action action)
        {
            _action = action;
        }

        public void Dispose()
        {
            if (_action == null)
            {
                return;
            }

            _action();
            _action = null;
        }
    }

    public static IDisposable Colorify(Color color)
    {
        Color previousColor = GUI.color;

        GUI.color = color;

        return new Disposable(() => GUI.color = previousColor);
    }

    public static IDisposable ColorifyBackground(Color color)
    {
        Color previousColor = GUI.backgroundColor;

        GUI.backgroundColor = color;

        return new Disposable(() => GUI.backgroundColor = previousColor);
    }
}
}