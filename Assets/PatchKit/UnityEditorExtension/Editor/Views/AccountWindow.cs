using JetBrains.Annotations;
using UnityEditor;

namespace PatchKit.UnityEditorExtension.Views
{
public class AccountWindow : Window
{
    [MenuItem("Tools/PatchKit/Account", false, 2)]
    public static void ShowWindow()
    {
        GetWindow(typeof(AccountWindow), true, "Account");
    }

    [UsedImplicitly]
    private void Awake()
    {
        ClearAndPush(new AccountScreen(this));
    }
}
}