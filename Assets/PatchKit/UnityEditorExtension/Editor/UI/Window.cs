using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace PatchKit.UnityEditorExtension.UI
{
public class Window : EditorWindow
{
    [SerializeField]
    [NotNull]
    private List<Screen> _screens = new List<Screen>();

    public Screen CurrentScreen
    {
        get
        {
            if (_screens.Count > 0)
            {
                Screen screen = _screens.Last();
                Assert.IsNotNull(screen);
                return screen;
            }

            return null;
        }
    }

    private Vector2? GetCurrentSize()
    {
        for (int i = _screens.Count - 1; i >= 0; i--)
        {
            Assert.IsNotNull(_screens[i]);
            if (_screens[i].Size.HasValue)
            {
                return _screens[i].Size.Value;
            }
        }

        return null;
    }

    private string GetCurrentTitle()
    {
        for (int i = _screens.Count - 1; i >= 0; i--)
        {
            Assert.IsNotNull(_screens[i]);
            if (_screens[i].Title != null)
            {
                return _screens[i].Title;
            }
        }

        return null;
    }

    [NotNull]
    public T Push<T>()
        where T : Screen
    {
        var screen = Screen.CreateInstance<T>(this);

        _screens.Add(screen);

        AdjustToCurrentScreen();
        Repaint();

        return screen;
    }

    public void Pop([NotNull] Screen screen, object result)
    {
        if (!_screens.Contains(screen))
        {
            throw new InvalidOperationException();
        }

        while (_screens.Contains(screen))
        {
            _screens.RemoveAt(_screens.Count - 1);
        }

        if (CurrentScreen != null)
        {
            CurrentScreen.OnActivatedFromTop(result);
        }

        AdjustToCurrentScreen();
        Repaint();
    }

    [NotNull]
    public T ClearAndPush<T>()
        where T : Screen
    {
        _screens.Clear();
        return Push<T>();
    }

    private void AdjustToCurrentScreen()
    {
        if (CurrentScreen == null)
        {
            return;
        }

        Vector2? size = GetCurrentSize();

        if (size.HasValue)
        {
            minSize = size.Value;
            maxSize = size.Value;
        }

        EditorGUIUtility.editingTextField = false;
    }

    private void OnGUI()
    {
        Screen currentScreen = CurrentScreen;
        if (currentScreen != null)
        {
            string titleValue = GetCurrentTitle();

            if (titleValue != null)
            {
                titleContent = new GUIContent(titleValue);
            }

            currentScreen.UpdateIfActive();

            // TODO: More elegant solution
            try
            {
                currentScreen.Draw();
            }
            catch (ArgumentException)
            { 
            }
        }
    }
}
}