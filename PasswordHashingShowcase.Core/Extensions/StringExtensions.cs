using System.Text;

namespace PasswordHashingShowcase.Core.Extensions
{
    internal static class StringExtensions
    {
        public static byte[] GetBytes(this string s) => Encoding.UTF8.GetBytes(s);

        public static string GetBase64String(this byte[] b) => Convert.ToBase64String(b, Base64FormattingOptions.None);
    }
}
