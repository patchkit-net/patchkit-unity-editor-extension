using UnityEditor;
using UnityEngine.Assertions;

namespace PatchKit.UnityEditorExtension.UI
{
public class AccountWindow : Window
{
    [MenuItem("Tools/PatchKit/Account", false, 2)]
    public static void ShowWindow()
    {
        var window = (AccountWindow) GetWindow(
            typeof(AccountWindow),
            true,
            "Account");

        Assert.IsNotNull(window);

        window.Push<AccountScreen>().Initialize();
    }
}
}