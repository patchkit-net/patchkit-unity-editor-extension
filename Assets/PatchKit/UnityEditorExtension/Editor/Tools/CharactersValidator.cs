using System;
using System.Linq;
using JetBrains.Annotations;

namespace PatchKit.UnityEditorExtension.Tools
{
public class CharactersValidator
{
    public static bool Validate([NotNull] string text)
    {
        if (text == null)
        {
            throw new ArgumentNullException("text");
        }

        if (!text.All(
            c => c >= 'a' && c <= 'z' ||
                char.IsWhiteSpace(c) ||
                char.IsPunctuation(c) ||
                char.IsDigit(c)))
        {
            return false;
        }

        return true;
    }
}
}