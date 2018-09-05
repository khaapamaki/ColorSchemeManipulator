using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using ColorSchemeInverter.Colors;
using ColorSchemeInverter.Filters;
using ColorSchemeInverter.SchemeFileSupport;

namespace ColorSchemeInverter
{
    public class ColorSchemeProcessor
    {
        private readonly SchemeFormat _schemeFormat;

        public ColorSchemeProcessor(SchemeFormat schemeFormat)
        {
            _schemeFormat = schemeFormat;
        }

        public void ProcessFile(string sourceFile, string targetFile, FilterSet filters)
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

        private FilterSet _filters;  // filters need to be stored for MatchEvaluator since it doesn't take parameters
        
        private string ApplyFilters(string text, FilterSet filters)
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
                if (Utils.IsValidHexString(rgbString) && rgbString.Length == rgbStringFormat.Length) {
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
            return m.Groups[0].ToString();  // alternative for throwing
        }

    }
}