using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ColorSchemeManipulator.Colors;
using ColorSchemeManipulator.Filters;

namespace ColorSchemeManipulator.SchemeFileSupport
{
    public class VSCodeParser : IColorSchemeParser<string>
    {
        private RgbHexFormatSpecs _specs;
        private readonly string _hexFormat;
        private readonly string _regExPattern = "ground\":\\s*\"#(?<hex>[0-9abcdefABCDEF]{8}|[0-9abcdefABCDEF]{6})\"";
        private readonly SchemeFormat _schemeFormat;

        private MatchCollection _matches;
        
        public VSCodeParser()
        {
            _specs =
                new RgbHexFormatSpecs() {
                    RgbHexFormat = "rrggbbaa",
                    Padding = "000000ff",
                    PaddingDirection = PaddingDirection.Right
                };
        }

        public List<Color> GetColors(string source)
        {            
            List<Color> colors = new List<Color>();
            MatchCollection matches = Regex.Matches(source, _regExPattern);
            _matches = matches;
            foreach (Match match in matches) {
                string rgbString = match.Groups["hex"].ToString();
                colors.Add(SchemeFormatUtils.FromHexString(rgbString, _schemeFormat));
            }

            return colors;
        }
        
        public List<ColorMatch> GetMatches(string text, List<Color> colors)
        {
            // Encapsulate filtered colors and regex matches within list of ColorMatch'es
            MatchCollection matches = _matches ?? Regex.Matches(text, _regExPattern);
            int i = 0;
            List<ColorMatch> colorMatches = new List<ColorMatch>();
            foreach (Match match in matches) {
                string rgbString = match.Groups["hex"].ToString(); 
                string filteredRgbString = HexRgb.ToRgbString(colors[i++], _hexFormat);

                colorMatches.Add(new ColorMatch()
                {
                    Index = match.Groups["hex"].Index,
                    Length = match.Groups["hex"].Length,
                    MatchingString = rgbString,
                    ReplacementString = filteredRgbString
                });
            }
            
            return colorMatches;
        }

        public string ReplaceColors(string source, List<ColorMatch> colorMatches)
        {
            return SchemeFormatUtils.BatchReplace(source, colorMatches);
        }
        
    }
}