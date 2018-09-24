using System;
using JetBrains.Annotations;
using UnityEditor;

namespace PatchKit.UnityEditorExtension.Views
{
[Serializable]
public abstract class View
{
    public View([NotNull] Window window)
    {
        if (window == null)
        {
            throw new ArgumentNullException("window");
        }

        Window = window;
    }

    public abstract void Initialize();

    public abstract void Draw();

    [NotNull]
    public Window Window { get; private set; }

    protected void Dispatch([NotNull] Action action)
    {
        if (action == null)
        {
            throw new ArgumentNullException("action");
        }

        EditorApplication.delayCall += () =>
        {
            action();
            Window.Repaint();
        };
    }
}
}