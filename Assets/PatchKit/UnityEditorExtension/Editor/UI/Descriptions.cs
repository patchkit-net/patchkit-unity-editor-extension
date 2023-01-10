namespace PatchKit.UnityEditorExtension.UI
{
public static class Descriptions
{
    public static readonly string PlatformChangeInfo =
        "While uploading applications from Unity, the PatchKit target platform is determined by the currently active project build platform.";

    public static readonly string NeedToPlatformChange = PlatformChangeInfo +
        "\n\n" +
        "To change it, you have to switch the project platform in the Build Settings window.";
}
}