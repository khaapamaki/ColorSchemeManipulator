using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using ColorSchemeManipulator.Colors;
using ColorSchemeManipulator.Filters;
using ColorSchemeManipulator.SchemeFileSupport;

namespace ColorSchemeManipulator
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
            } catch (Exception ex) {
                Console.WriteLine(GetType().FullName + " : " + ex.Message);
                throw;
            }

            File.WriteAllText(targetFile, convertedText, Encoding.Default);
        }
        
        // filters need to be stored for MatchEvaluator since it doesn't take parameters
        private FilterSet _filters;  
        
        private string ApplyFilters(string text, FilterSet filters)
        {
            _filters = filters;
            string regExPattern = SchemeFormatUtil.GetRegEx(_schemeFormat);
            text = Regex.Replace(text, regExPattern, new MatchEvaluator(MatchReplace));
            return text;
        }

        private string MatchReplace(Match m)
        {
            string rgbHexFormat = SchemeFormatUtil.GetRgbHexFormat(_schemeFormat);
            if (m.Groups.Count == 4) {
                
                // the second capture group of the regex pattern must be the one that contains color data
                string rgbString = m.Groups[2].ToString();
                
                if (Utils.IsValidHexString(rgbString) && rgbString.Length <= rgbHexFormat.Length) {

                    string filteredRgbString =
                        Rgb.FromRgbString(rgbString, rgbHexFormat)
                            .ApplyFilterSet(_filters)
                            .ToRgbString(rgbHexFormat);

                    // Console.WriteLine(rgbString + " -> " + filteredRGBString);

                    return m.Groups[1]
                           + filteredRgbString
                           + m.Groups[3];
                } else {
                    Console.WriteLine("Invalid RGB string: " + rgbString);
                }
            }

            throw new Exception("Regular Expression Mismatch");
            // return m.Groups[0].ToString();  // alternative for throwing
        }

    }
}