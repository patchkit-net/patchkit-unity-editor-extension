using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PatchKit.Tools.Integration
{
    public static class Hashing
    {
        public static string HashMD5(string plainText)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(plainText);
                return string.Join("", md5.ComputeHash(bytes).Select(b => b.ToString("x2")).ToArray());
            }
        }
    }
}