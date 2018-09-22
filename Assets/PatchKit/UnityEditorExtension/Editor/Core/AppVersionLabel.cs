using System.Linq;
using JetBrains.Annotations;

namespace PatchKit.UnityEditorExtension.Core
{
public struct AppVersionLabel
{
    [NotNull]
    public readonly string Value;

    public readonly bool IsValid;

    public AppVersionLabel(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ValidationException("Value cannot be null or empty.");
        }

        if (!value.All(
            c => c >= 'a' && c <= 'z' ||
                char.IsWhiteSpace(c) ||
                char.IsPunctuation(c) ||
                char.IsDigit(c)))
        {
            throw new ValidationException(
                "Value contains forbidden characters.");
        }

        Value = value;

        IsValid = true;
    }
}
}