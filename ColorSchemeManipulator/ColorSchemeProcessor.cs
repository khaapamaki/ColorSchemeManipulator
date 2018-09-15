using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using ColorSchemeManipulator.Colors;
using ColorSchemeManipulator.Common;
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
        
        public void ProcessFile(string sourceFile, string targetFile, BatchFilterSet filters)
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
        private BatchFilterSet _filters;

        private string ApplyFilters(string text, BatchFilterSet filters)
        {
            _filters = filters;
            string regExPattern = SchemeFormatUtil.GetRegEx(_schemeFormat);
            text = Regex.Replace(text, regExPattern, new MatchEvaluator(MatchReplace));
            return text;
        }

        // todo THIS CURRENTLY DOES NOTHING
        private string MatchReplace(Match m)
        {
            string rgbHexFormat = SchemeFormatUtil.GetRgbHexFormat(_schemeFormat);
            if (m.Groups.Count == 4) {
                // the second capture group of the regex pattern must be the one that contains color data
                string rgbString = m.Groups[2].ToString();

                if (Utils.IsValidHexString(rgbString) && rgbString.Length <= rgbHexFormat.Length) {
                    string filteredRgbString =
                        Rgb.FromRgbString(rgbString, rgbHexFormat)
                            // .ApplyFilterSet(_filters)
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

        private IEnumerable<Rgb> GetAllColors(string text)
        {
            var colorList = new List<Rgb>();
            string rgbHexFormat = SchemeFormatUtil.GetRgbHexFormat(_schemeFormat);
            string regExPattern = SchemeFormatUtil.GetRegEx(_schemeFormat);
            MatchCollection matches = Regex.Matches(text, regExPattern);
            foreach (var obj in matches) {
                if (obj is Match m) {
                    string rgbString = m.Groups[2].ToString();
                    if (Utils.IsValidHexString(rgbString) && rgbString.Length <= rgbHexFormat.Length) {
                        var color = Rgb.FromRgbString(rgbString, rgbHexFormat);
                        colorList.Add(color);
                    }
                }
            }

            return colorList;
        }

        private (double, double) GetMaxAndMinLightness(IEnumerable<Rgb> colors)
        {
            double? max = null, min = null;
            foreach (var rgb in colors) {
                double br = rgb.ToHsl().Lightness;
                if (min == null || br < min) min = br;
                if (max == null || br > max) max = br; 
            }

            return (min ?? 0, max ?? 1);
        }
        
        private (double, double) GetMaxAndMinBrightness(IEnumerable<Rgb> colors)
        {
            double? max = null, min = null;
            foreach (var rgb in colors) {
                double br = ColorMath.RgbPerceivedBrightness(rgb);
                if (min == null || br < min) min = br;
                if (max == null || br > max) max = br; 
            }

            return (min ?? 0, max ?? 1);
        }

    }
}