using System.Linq;
using JetBrains.Annotations;

namespace PatchKit.UnityEditorExtension.Core
{
public struct ApiKey
{
    [NotNull]
    public readonly string Value;

    public readonly bool IsValid;

    public ApiKey(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ValidationException("Value cannot be null or empty.");
        }

        if (!value.All(char.IsLetterOrDigit))
        {
            throw new ValidationException(
                "Value cannot have other characters than letters and digits.");
        }

        if (value.Length != 32)
        {
            throw new ValidationException(
                "Value must be 32 characters length.");
        }

        Value = value;

        IsValid = true;
    }
}
}