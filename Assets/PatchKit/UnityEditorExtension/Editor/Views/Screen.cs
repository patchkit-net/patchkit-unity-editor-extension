using JetBrains.Annotations;
using UnityEngine;

namespace PatchKit.UnityEditorExtension.Views
{
public abstract class Screen : View
{
    public Screen([NotNull] Window window)
        : base(window)
    {
    }

    [NotNull]
    public abstract string Title { get; }

    public abstract Vector2 Size { get; }

    public abstract bool ShouldBePopped();
}
}