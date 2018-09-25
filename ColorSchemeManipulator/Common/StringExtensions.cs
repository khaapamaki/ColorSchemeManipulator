using System.Text;

namespace ColorSchemeManipulator.Common
{
    public static class StringExtensions
    {
        public static string ReplaceWithin(this string s, int index, int length, string replacement)
        {
            var builder = new StringBuilder();
            builder.Append(s.Substring(0,index));
            builder.Append(replacement);
            builder.Append(s.Substring(index + length));
            return builder.ToString();
        }

        public static string PadLeft(this string str, string padding)
        {
            if (str.Length >= padding.Length)
                return str;

            return padding.Substring(0, padding.Length - str.Length) + str;
        }
        
        public static string PadRight(this string str, string padding)
        {
            if (str.Length >= padding.Length)
                return str;

            return str + padding.Substring(str.Length);
        }
    }
}