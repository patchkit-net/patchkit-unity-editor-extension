using System;
using JetBrains.Annotations;
using PatchKit.UnityEditorExtension.Core;
using UnityEngine;
using UnityEngine.Assertions;

namespace PatchKit.UnityEditorExtension.Views
{
public class LinkAccountMediator
{
    [NotNull]
    private readonly LinkAccountScreen _screen;

    public LinkAccountMediator([NotNull] LinkAccountScreen screen)
    {
        if (screen == null)
        {
            throw new ArgumentNullException("screen");
        }

        _screen = screen;
    }

    public void Initialize()
    {
        ApiKey? savedApiKey = Config.GetLinkedAccountApiKey();

        if (savedApiKey.HasValue)
        {
            NewApiKey = savedApiKey.Value.Value;
        }
        else
        {
            NewApiKey = string.Empty;
        }
    }

    public string NewApiKey { get; set; }

    public string NewApiKeyValidationError
    {
        get { return ApiKey.GetValidationError(NewApiKey); }
    }

    public void Link()
    {
        var apiKey = new ApiKey(NewApiKey);

        Config.LinkAccount(apiKey);

        _screen.OnLinked(_screen);
    }

    public void OpenProfileWebpage()
    {
        Application.OpenURL("https://panel.patchkit.net/users/profile");
    }

    public void Cancel()
    {
        _screen.OnCanceled(_screen);
    }
}
}