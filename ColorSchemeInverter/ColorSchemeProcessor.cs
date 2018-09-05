using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ColorSchemeInverter
{
    public class ColorSchemeProcessor
    {
        public ColorSchemeProcessor(SchemeFormat schemeFormat)
        {
            _schemeFormat = schemeFormat;
        }

        private SchemeFormat _schemeFormat;
        private HSLFilterSet _filters;

        public void ProcessFile(string sourceFile, string targetFile, HSLFilterSet filters)
        {
            string text = File.ReadAllText(sourceFile);
            string convertedText;
            try {
                convertedText = ApplyFilters(text, filters);
            } catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }

            File.WriteAllText(targetFile, convertedText, Encoding.Default);
        }

        private string ApplyFilters(string text, HSLFilterSet filters)
        {
            _filters = filters;
            string regExPattern = SchemeFormatUtil.GetRegEx(_schemeFormat);
            text = Regex.Replace(text, regExPattern, new MatchEvaluator(MatchReplace));
            return text;
        }

        private string MatchReplace(Match m)
        {
            string rgbStringFormat = SchemeFormatUtil.GetRGBStringFromat(_schemeFormat);
            if (m.Groups.Count == 4) {
                string rgbString = m.Groups[2].ToString();
                if (IsValidHexString(rgbString) && rgbString.Length == rgbStringFormat.Length) {
                    string filteredRGBString =
                        RGB.FromRGBString(rgbString, rgbStringFormat)
                            .ToHSL()
                            .ApplyFilterSet(_filters)
                            .ToRGB()
                            .ToRGBString(rgbStringFormat);

                    Console.WriteLine(rgbString + " -> " + filteredRGBString);

                    return m.Groups[1]
                           + filteredRGBString
                           + m.Groups[3];
                } else {
                    Console.WriteLine("Invalid RGB string: " + rgbString);
                }
            }

            throw new Exception("Regular Expression Mismatch");
            return m.Groups[0].ToString();
        }

        // Duplicate code in RGB class
        private static bool IsValidHexString(string str)
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