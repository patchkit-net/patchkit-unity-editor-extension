using System;
using JetBrains.Annotations;
using NUnit.Framework;

namespace PatchKit.UnityEditorExtension.Views
{
public class NotLinkedAppMediator
{
    [NotNull]
    private readonly NotLinkedAppView _view;

    public NotLinkedAppMediator([NotNull] NotLinkedAppView view)
    {
        if (view == null)
        {
            throw new ArgumentNullException("view");
        }

        _view = view;
    }

    public void OpenLinkDialog()
    {
        _view.Window.Push(
            new LinkAppScreen(
                _view.Platform,
                (v, app) =>
                {
                    Assert.IsNotNull(v);
                    _view.Window.Pop(v);
                },
                v =>
                {
                    Assert.IsNotNull(v);
                    _view.Window.Pop(v);
                },
                _view.Window));
    }
}
}