using System;
using System.Linq;
using JetBrains.Annotations;
using PatchKit.UnityEditorExtension.Core;

namespace PatchKit.UnityEditorExtension.Views
{
public class CreateAppMediator
{
    [NotNull]
    private readonly CreateAppScreen _screen;

    [NotNull]
    private Api.Models.Main.App[] _apps;

    public CreateAppMediator([NotNull] CreateAppScreen screen)
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

    public void Initialize()
    {
        Name = string.Empty;

        _apps = Core.Api.GetApps();
    }

    public bool ShouldBePopped()
    {
        return !Config.GetLinkedAccountApiKey().HasValue;
    }

    public string Name { get; set; }

    public string NameValidationError
    {
        get
        {
            if (_apps.Any(x => x.Name == Name))
            {
                return "This name is already taken.";
            }

            return AppName.GetValidationError(Name);
        }
    }

    public void Create()
    {
        Api.Models.Main.App app = Core.Api.CreateNewApp(
            new AppName(Name),
            _screen.Platform);

        _screen.OnCreate(_screen, app);
    }

    public void Cancel()
    {
        _screen.OnCancel(_screen);
    }
}
}