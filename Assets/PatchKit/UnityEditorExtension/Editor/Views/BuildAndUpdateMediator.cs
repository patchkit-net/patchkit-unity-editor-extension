using System;
using JetBrains.Annotations;
using Newtonsoft.Json.Bson;
using PatchKit.UnityEditorExtension.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace PatchKit.UnityEditorExtension.Views
{
public class BuildAndUpdateMediator
{
    [NotNull]
    private readonly BuildAndUploadScreen _screen;

    public BuildAndUpdateMediator([NotNull] BuildAndUploadScreen screen)
    {
        if (screen == null)
        {
            throw new ArgumentNullException("screen");
        }

        _screen = screen;
    }

    public void Initialize()
    {
    }

    public bool ShouldBePopped()
    {
        return _screen.Platform != AppBuild.Platform;
    }

    public bool IsAccountLinked
    {
        get { return Config.GetLinkedAccountApiKey().HasValue; }
    }

    public bool IsAppLinked
    {
        get { return Config.GetLinkedAppSecret(_screen.Platform).HasValue; }
    }
}
}