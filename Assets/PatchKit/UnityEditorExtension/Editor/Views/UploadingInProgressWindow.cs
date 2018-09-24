using JetBrains.Annotations;

namespace PatchKit.UnityEditorExtension.Views
{
public class UploadingInProgressWindow : Window
{
    public static void ShowWindow()
    {
        GetWindow(typeof(UploadingInProgressWindow), true, "Uploading");
    }

    [UsedImplicitly]
    private void Awake()
    {
        ClearAndPush(new UploadingInProgressScreen(this));
    }
}
}