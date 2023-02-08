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
            return "Application version changelog cannot be null.";
        }

        if (!value.All(
            c => char.IsLetterOrDigit(c) ||
                char.IsWhiteSpace(c) ||
                c == ':' ||
                c == '_' ||
                c == '-'))
        {
            return
                "The label text can include only letters,\n" +
                "numbers, and ':', '_', or '-' characters.";
        }

        return null;
    }
}
}