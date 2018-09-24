using PatchKit.UnityEditorExtension.Core;
using UnityEngine;
using UnityEngine.Assertions;

namespace PatchKit.UnityEditorExtension.UI
{
public class BuildAndUploadScreen : Screen
{
    #region GUI

    public override string Title
    {
        get { return "Build & Upload"; }
    }

    public override Vector2? Size
    {
        get { return new Vector2(600f, 400f); }
    }

    public override void UpdateIfActive()
    {
        if (_platform != AppBuild.Platform)
        {
            Pop(null);
        }
        else if (!IsAccountLinked)
        {
            Push<NotLinkedAccountScreen>().Initialize();
        }
        else if (!IsAppLinked)
        {
            Push<NotLinkedAppScreen>().Initialize(_platform);
        }
        else
        {
            AppSecret? appSecret = Config.GetLinkedAppSecret(_platform);
            Assert.IsTrue(appSecret.HasValue);
            Push<SafeBuildAndUploadScreen>()
                .Initialize(_platform, appSecret.Value);
        }
    }

    public override void Draw()
    {
    }

    #endregion

    #region Data

    [SerializeField]
    private AppPlatform _platform;

    #endregion

    #region Logic

    public void Initialize(AppPlatform platform)
    {
        _platform = platform;
    }

    public override void OnActivatedFromTop(object result)
    {
    }

    private bool IsAccountLinked
    {
        get { return Config.GetLinkedAccountApiKey().HasValue; }
    }

    private bool IsAppLinked
    {
        get { return Config.GetLinkedAppSecret(_platform).HasValue; }
    }

    #endregion
}
}