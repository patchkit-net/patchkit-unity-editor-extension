using System.Linq;

namespace PatchKit.UnityEditorExtension.Logic
{
public static class TextValidation
{
    public static bool DoesStringContainOnlyAllowedCharacters(string s)
    {
        return s.ToLower()
            .All(
                c => (c >= 'a' && c <= 'z') ||
                    char.IsWhiteSpace(c) ||
                    char.IsPunctuation(c) ||
                    char.IsDigit(c));
    }
}
}