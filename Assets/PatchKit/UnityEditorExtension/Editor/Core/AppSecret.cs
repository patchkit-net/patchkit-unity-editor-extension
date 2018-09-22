using JetBrains.Annotations;

namespace PatchKit.UnityEditorExtension.Core
{
public struct AppSecret
{
    [NotNull]
    public readonly string Value;

    public readonly bool IsValid;

    public AppSecret(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ValidationException("Value cannot be null or empty.");
        }

        Value = value;

        IsValid = true;
    }
}
}