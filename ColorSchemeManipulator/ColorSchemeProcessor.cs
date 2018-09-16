using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
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
        private List<Color> _filteredColors;

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
            _filteredColors = new List<Color>();
            string rgbHexFormat = SchemeFormatUtil.GetRgbHexFormat(_schemeFormat);
            List<Color> colorSet = new List<Color>();
            string regExPattern = SchemeFormatUtil.GetRegEx(_schemeFormat);
            MatchCollection matches = Regex.Matches(text, regExPattern);
            foreach (Match match in matches) {
                string rgbString = match.Groups[2].ToString();
                colorSet.Add(HexRgb.FromRgbString(rgbString, rgbHexFormat));
            }

            _filteredColors = _filters.ApplyTo(colorSet).ToList();
            _matchReplaceLoopIndex = 0;
            text = Regex.Replace(text, regExPattern, new MatchEvaluator(MatchReplace));
            return text;
        }

        // todo THIS CURRENTLY DOES NOTHING
        private int _matchReplaceLoopIndex = 0;
        private string MatchReplace(Match m)
        {
            string rgbHexFormat = SchemeFormatUtil.GetRgbHexFormat(_schemeFormat);
            if (m.Groups.Count == 4) {
                // the second capture group of the regex pattern must be the one that contains color data
                string rgbString = m.Groups[2].ToString();

                if (Utils.IsValidHexString(rgbString) && rgbString.Length <= rgbHexFormat.Length) {
                    string filteredRgbString =  HexRgb.ToRgbString(_filteredColors[_matchReplaceLoopIndex++], rgbHexFormat);

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

        private IEnumerable<Color> GetAllColors(string text)
        {
            //var colorList = new List<Color>();
            string rgbHexFormat = SchemeFormatUtil.GetRgbHexFormat(_schemeFormat);
            string regExPattern = SchemeFormatUtil.GetRegEx(_schemeFormat);
            MatchCollection matches = Regex.Matches(text, regExPattern);
            foreach (var obj in matches) {
                if (obj is Match m) {
                    string rgbString = m.Groups[2].ToString();
                    if (Utils.IsValidHexString(rgbString) && rgbString.Length <= rgbHexFormat.Length) {
                        var rgb8 = Rgb8Bit.FromRgbString(rgbString, rgbHexFormat);
                        var color = Color.FromRgb8(rgb8.Red8, rgb8.Green8, rgb8.Blue8, rgb8.Alpha8);
                        yield return color;
                    }
                }
            }
        }

        private (double, double) GetMaxAndMinLightness(IEnumerable<Color> colors)
        {
            double? max = null, min = null;
            foreach (var color in colors) {
                double br = color.Lightness;
                if (min == null || br < min) min = br;
                if (max == null || br > max) max = br;
            }

            return (min ?? 0, max ?? 1);
        }

        private (double, double) GetMaxAndMinBrightness(IEnumerable<Color> colors)
        {
            double? max = null, min = null;
            foreach (var color in colors) {
                double br = color.GetBrightness();
                if (min == null || br < min) min = br;
                if (max == null || br > max) max = br;
            }

            return (min ?? 0, max ?? 1);
        }
    }
}