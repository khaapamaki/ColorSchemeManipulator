using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ColorSchemeManipulator.Colors;
using ColorSchemeManipulator.Filters;
using ColorSchemeManipulator.SchemeFileSupport;

namespace ColorSchemeManipulator
{
    public class ColorSchemeProcessor
    {
        // private readonly SchemeFormat _schemeFormat;
        private readonly string _hexFormat;
        private readonly string _regExPattern;

        public ColorSchemeProcessor(SchemeFormat schemeFormat)
        {
            // _schemeFormat = schemeFormat;
            _hexFormat = SchemeFormatUtil.GetRgbHexFormat(schemeFormat);
            _regExPattern = SchemeFormatUtil.GetRegEx(schemeFormat);
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
        private List<Color> _filteredColors;
        
        private string ApplyFilters(string text, FilterSet filters)
        {
            _filters = filters;
            _filteredColors = new List<Color>();
            List<Color> colorSet = new List<Color>();
            MatchCollection matches = Regex.Matches(text, _regExPattern);
            foreach (Match match in matches) {
                string rgbString = match.Groups[2].ToString();
                colorSet.Add(HexRgb.FromRgbString(rgbString, _hexFormat));
            }

            _filteredColors = _filters.ApplyTo(colorSet).ToList();
            _matchReplaceLoopIndex = 0;
            text = Regex.Replace(text, _regExPattern, new MatchEvaluator(MatchReplace));

            return text;
        }

        // Todo If there's any better way to batch replace by enumerated filtering results, do so. This is ugly...
        private int _matchReplaceLoopIndex = 0;
        private string MatchReplace(Match m)
        {
            if (m.Groups.Count == 4) {
                // the second capture group of the regex pattern must be the one that contains color data
                string rgbString = m.Groups[2].ToString();

                if (HexRgb.IsValidHexString(rgbString) && rgbString.Length <= _hexFormat.Length) {
                    string filteredRgbString =  HexRgb.ToRgbString(_filteredColors[_matchReplaceLoopIndex++], _hexFormat);
                    // Console.WriteLine(rgbString + " -> " + filteredRgbString);
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

        [Obsolete]
        private IEnumerable<Color> GetAllColors(string text)
        {
            MatchCollection matches = Regex.Matches(text, _regExPattern);
            foreach (var obj in matches) {
                if (obj is Match m) {
                    string rgbString = m.Groups[2].ToString();
                    if (HexRgb.IsValidHexString(rgbString) && rgbString.Length <= _hexFormat.Length) {
                        yield return HexRgb.FromRgbString(rgbString, _hexFormat);
                    }
                }
            }
        }
        
        // Todo move to FilterUtils
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
        
        // Todo move to FilterUtils
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