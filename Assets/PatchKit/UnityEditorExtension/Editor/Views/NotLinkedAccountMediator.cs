using System;
using JetBrains.Annotations;
using UnityEngine.Assertions;

namespace PatchKit.UnityEditorExtension.Views
{
public class NotLinkedAccountMediator
{
    [NotNull]
    private readonly NotLinkedAccountView _view;

    public NotLinkedAccountMediator([NotNull] NotLinkedAccountView view)
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
            new LinkAccountScreen(
                v =>
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