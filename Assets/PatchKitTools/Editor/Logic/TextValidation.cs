using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PatchKit.Tools.Integration.Views
{
    public static class TextValidation
    {

        public static bool DoesStringContainOnlyAllowedCharacters(string s)
        {
            return s.ToLower().All(c => (c >= 'a' && c <= 'z') || char.IsWhiteSpace(c) || char.IsPunctuation(c) || char.IsDigit(c));
        }


    }
}
