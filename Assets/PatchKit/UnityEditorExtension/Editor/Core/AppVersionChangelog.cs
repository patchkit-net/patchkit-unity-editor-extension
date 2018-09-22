using System.Linq;
using JetBrains.Annotations;

namespace PatchKit.UnityEditorExtension.Core
{
public struct AppVersionChangelog
{
    [NotNull]
    public readonly string Value;

    public readonly bool IsValid;

    public AppVersionChangelog(string value)
    {
        if (value == null)
        {
            throw new ValidationException("Value cannot be null.");
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