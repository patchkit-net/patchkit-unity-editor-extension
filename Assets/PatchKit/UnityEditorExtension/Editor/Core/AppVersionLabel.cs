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
            return "The application version label cannot be null.";
        }

        if (string.IsNullOrEmpty(value))
        {
            return "The application version label cannot be empty.";
        }

        if (!value.All(
            c => char.IsLetterOrDigit(c) ||
                char.IsWhiteSpace(c) ||
                c == ':' ||
                c == '_' ||
                c == '-'))
        {
            return "The label text can include only letters,\n" +
                "numbers, and ':', '_', or '-' characters.";
        }

        return null;
    }
}
}