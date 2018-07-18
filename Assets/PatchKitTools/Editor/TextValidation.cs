using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class TextValidation {


	//public static void ValitadeEnglish (string s) {
 //       string a = "fdsaf6543śąśą";
 //       string b = "asdff6543dsa";
 //       string zolc = "żółć";

 //       string[] strings = new[] { "asdff65FDFSAFDSA3,  . --- dsa", "fdsaf6. 543śąśą", "fdsafdas", "żólć" };

 //       foreach (var str in strings)
 //       {
 //           Debug.Log("Result for " + str + " is " + IsAscii(str));
 //       }
 //      // return 
 //   }

    public static bool IsEnglish(string s)
    {
        return s.ToLower().All(c => (c >= 'a' && c <= 'z') || char.IsWhiteSpace(c) || char.IsPunctuation(c) || char.IsDigit(c));
    }


}
