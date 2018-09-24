using JetBrains.Annotations;
using PatchKit.UnityEditorExtension.Core;
using UnityEditor;
using UnityEngine.Assertions;

namespace PatchKit.UnityEditorExtension.Views
{
public class AccountMediator
{
    [NotNull]
    private readonly AccountScreen _screen;

    public AccountMediator([NotNull] AccountScreen screen)
    {
        _screen = screen;
    }

    public void Initialize()
    {
    }

    public string ApiKey
    {
        get
        {
            ApiKey? value = Config.GetLinkedAccountApiKey();

            return value.HasValue ? value.Value.Value : string.Empty;
        }
    }

    public bool IsAccountLinked
    {
        get { return Config.GetLinkedAccountApiKey().HasValue; }
    }

    public void OpenLinkDialog()
    {
        _screen.Window.Push(
            new LinkAccountScreen(
                v =>
                {
                    Assert.IsNotNull(v);
                    _screen.Window.Pop(v);
                },
                v =>
                {
                    Assert.IsNotNull(v);
                    _screen.Window.Pop(v);
                },
                _screen.Window));
    }

    public void Unlink()
    {
        if (EditorUtility.DisplayDialog(
            "Are you sure?",
            "Are you sure that you want to unlink your PatchKit account from this project?\n\nThis operation cannot be undone.",
            "Yes",
            "No"))
        {
            Config.UnlinkAccount();
        }
    }
}
}