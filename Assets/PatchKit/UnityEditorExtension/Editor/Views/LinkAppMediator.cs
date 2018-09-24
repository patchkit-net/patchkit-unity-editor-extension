using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using PatchKit.UnityEditorExtension.Core;
using UnityEngine.Assertions;

namespace PatchKit.UnityEditorExtension.Views
{
public class LinkAppMediator
{
    [NotNull]
    private readonly LinkAppScreen _screen;

    [NotNull]
    private Api.Models.Main.App[] _apps;

    public LinkAppMediator([NotNull] LinkAppScreen screen)
    {
        if (screen == null)
        {
            throw new ArgumentNullException("screen");
        }

        _screen = screen;
        _apps = new Api.Models.Main.App[]
        {
        };
    }

    public bool ShouldBePopped()
    {
        return !Config.GetLinkedAccountApiKey().HasValue;
    }

    public void Initialize()
    {
        AppSecret? appSecret = Config.GetLinkedAppSecret(_screen.Platform);

        if (appSecret.HasValue)
        {
            LinkedAppSecret = appSecret.Value.Value;
        }
        else
        {
            LinkedAppSecret = null;
        }

        _apps = Core.Api.GetApps()
            .Where(x => x.Platform == _screen.Platform.ToApiString())
            .ToArray();
    }

    public string LinkedAppSecret { get; private set; }

    [NotNull]
    public IEnumerable<Api.Models.Main.App> Apps
    {
        get { return _apps; }
    }

    public void Link(Api.Models.Main.App app)
    {
        Assert.AreEqual(_screen.Platform.ToApiString(), app.Platform);
        Assert.IsTrue(_apps.Contains(app));

        Config.LinkApp(new AppSecret(app.Secret), _screen.Platform);
        LinkedAppSecret = app.Secret;

        _screen.OnLinked(_screen, app);
    }

    public void CreateNew()
    {
        _screen.Window.Push(
            new CreateAppScreen(
                _screen.Platform,
                (v, app) =>
                {
                    Assert.IsNotNull(v);
                    _screen.Window.Pop(v);

                    Config.LinkApp(new AppSecret(app.Secret), _screen.Platform);
                    LinkedAppSecret = app.Secret;

                    _screen.OnLinked(_screen, app);
                },
                v =>
                {
                    Assert.IsNotNull(v);
                    _screen.Window.Pop(v);
                },
                _screen.Window));
    }

    public void Cancel()
    {
        _screen.OnCanceled(_screen);
    }
}
}