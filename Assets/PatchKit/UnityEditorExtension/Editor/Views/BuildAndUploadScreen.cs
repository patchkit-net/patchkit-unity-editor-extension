using System;
using JetBrains.Annotations;
using PatchKit.UnityEditorExtension.Core;
using UnityEngine;

namespace PatchKit.UnityEditorExtension.Views
{
public class BuildAndUploadScreen : Screen
{
    [NotNull]
    private readonly BuildAndUpdateMediator _mediator;

    [NotNull]
    private readonly NotLinkedAccountView _notLinkedAccountView;

    [NotNull]
    private readonly NotLinkedAppView _notLinkedAppView;

    [NotNull]
    private readonly SafeBuildAndUploadView _safeBuildAndUploadView;

    private View _currentView;

    public AppPlatform Platform { get; private set; }

    public BuildAndUploadScreen(AppPlatform platform, [NotNull] Window window)
        : base(window)
    {
        Platform = platform;
        _mediator = new BuildAndUpdateMediator(this);
        _notLinkedAccountView = new NotLinkedAccountView(window);
        _notLinkedAppView = new NotLinkedAppView(platform, window);
        _safeBuildAndUploadView = new SafeBuildAndUploadView(window);
    }

    public override string Title
    {
        get { return "Build & Upload"; }
    }

    public override Vector2 Size
    {
        get { return new Vector2(400f, 400f); }
    }

    public override void Initialize()
    {
        _mediator.Initialize();
        _notLinkedAccountView.Initialize();
    }

    public override bool ShouldBePopped()
    {
        return _mediator.ShouldBePopped();
    }

    private void SwitchToView([NotNull] View view)
    {
        if (view == null)
        {
            throw new ArgumentNullException("view");
        }

        Dispatch(
            () =>
            {
                if (_currentView != view)
                {
                    _currentView = view;
                    _currentView.Initialize();
                }
            });
    }

    public override void Draw()
    {
        if (!_mediator.IsAccountLinked)
        {
            SwitchToView(_notLinkedAccountView);
        }
        else if (!_mediator.IsAppLinked)
        {
            SwitchToView(_notLinkedAppView);
        }
        else
        {
            SwitchToView(_safeBuildAndUploadView);
        }

        if (_currentView != null)
        {
            _currentView.Draw();
        }
    }
}
}