using JetBrains.Annotations;
using UnityEditor;

namespace PatchKit.UnityEditorExtension.Views
{
public class AboutWindow : Window
{
    [MenuItem("Tools/PatchKit/About", false, -50)]
    public static void ShowWindow()
    {
        GetWindow(typeof(AboutWindow), true, "About");
    }

    [UsedImplicitly]
    private void Awake()
    {
        ClearAndPush(new AboutScreen(this));
    }
}
}