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

        public static (string, string) SplitIntoCommandAndArguments(string option)
        {
            string cmd = option;
            string argString = "";
            int splitPos = option.IndexOf('=');
            if (splitPos > 0) {
                cmd = option.Substring(0, splitPos);
                if (splitPos < option.Length) {
                    argString = option.Substring(splitPos + 1);
                }
            }

            return (cmd, argString);
        }

        public static string[] ExtractArgs(string argString)
        {
            var args = new List<string>();
            foreach (var s in argString.Trim('"').Split(',')) {
                args.Add(s.Trim());
            }

            return args.ToArray();
        }
    }
}