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
        string validationError = GetValidationError(value);

        if (validationError != null)
        {
            throw new ValidationException(validationError);
        }

        Value = value;

        IsValid = true;
    }

    [ContractAnnotation("null => notNull")]
    public static string GetValidationError(string value)
    {
        if (value == null)
        {
            return "Value cannot be null.";
        }

        if (string.IsNullOrEmpty(value))
        {
            return "Value cannot be empty.";
        }

        if (!value.All(
            c => c >= 'a' && c <= 'z' ||
                c >= 'A' && c <= 'Z' ||
                char.IsWhiteSpace(c) ||
                char.IsPunctuation(c) ||
                char.IsDigit(c)))
        {
            return "Value contains forbidden characters.";
        }

        return null;
    }
}
}