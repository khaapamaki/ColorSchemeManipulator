using System.Collections.Generic;

namespace ColorSchemeInverter
{
    public static class Utils
    {
        public static bool IsUppercase(string str)
        {
            return str.ToUpper() == str;
        }

        public static bool IsValidHexString(string str)
        {
            const string validHex = "0123456789abcdefABCDEF";
            foreach (var c in str) {
                if (!validHex.Contains(c.ToString()))
                    return false;
            }

            return true;
        }

    }
}