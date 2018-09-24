using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace PatchKit.UnityEditorExtension.Views
{
public class Window : EditorWindow
{
    [NotNull]
    private readonly Stack<Screen> _views = new Stack<Screen>();

    public void Push([NotNull] Screen screen)
    {
        if (screen == null)
        {
            throw new ArgumentNullException("screen");
        }

        if (screen.ShouldBePopped())
        {
            return;
        }

        _views.Push(screen);

        screen.Initialize();

        AdjustToNewView();
        Repaint();
    }

    public void Pop()
    {
        _views.Pop();

        AdjustToNewView();
        Repaint();
    }

    public void Pop([NotNull] Screen screen)
    {
        while (_views.Contains(screen))
        {
            _views.Pop();
        }

        AdjustToNewView();
        Repaint();
    }

    public void ClearAndPush([NotNull] Screen screen)
    {
        if (screen == null)
        {
            throw new ArgumentNullException("screen");
        }

        _views.Clear();
        Push(screen);
    }

    private void AdjustToNewView()
    {
        Screen screen = _views.Peek();

        if (screen == null)
        {
            return;
        }

        minSize = screen.Size;
        maxSize = screen.Size;

        EditorGUIUtility.editingTextField = false;
    }

    private void OnGUI()
    {
        Screen screen = null;

        while (screen == null && _views.Count > 0)
        {
            screen = _views.Peek();
            Assert.IsNotNull(screen);

            if (screen.ShouldBePopped())
            {
                _views.Pop();
                AdjustToNewView();
                screen = null;
            }
        }

        if (screen == null)
        {
            return;
        }

        titleContent = new GUIContent(screen.Title);

        screen.Draw();
    }
}
}