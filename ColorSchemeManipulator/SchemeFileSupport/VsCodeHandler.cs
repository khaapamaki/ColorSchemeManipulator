using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ColorSchemeManipulator.Colors;
using ColorSchemeManipulator.Filters;

namespace ColorSchemeManipulator.SchemeFileSupport
{
    public class VsCodeHandler : IColorFileHandler<string>
    {
        private RgbHexFormatSpecs _specs;

        private const string RegExPattern = "ground\":\\s*\"#(?<hex>[0-9abcdefABCDEF]{8}|[0-9abcdefABCDEF]{6})\"";
        private const SchemeFormat SchemeFormat = SchemeFileSupport.SchemeFormat.VSCode;
        private const string HexFormat = "rrggbbaa";
        
        private MatchCollection _matches;
        
        public VsCodeHandler()
        {
            _specs =
                new RgbHexFormatSpecs() {
                    RgbHexFormat = "rrggbbaa",
                    Padding = "000000ff",
                    PaddingDirection = PaddingDirection.Right
                };
        }

        public string ReadFile(string sourceFile)
        {
            return File.ReadAllText(sourceFile);;
        }

        public void WriteFile(string text, string targetFile)
        {
            File.WriteAllText(targetFile, text, Encoding.Default);
        }
        
        public IEnumerable<Color> GetColors(string source)
        {            
            //List<Color> colors = new List<Color>();
            var matches = Regex.Matches(source, RegExPattern);
            _matches = matches;
            foreach (Match match in matches) {
                string rgbString = match.Groups["hex"].ToString();
                yield return SchemeFormatUtils.FromHexString(rgbString, SchemeFormat);
                // colors.Add(SchemeFormatUtils.FromHexString(rgbString, SchemeFormat.VSCode));
            }

            // return colors;
        }   

        public string ReplaceColors(string source, IEnumerable<Color> colors)
        {
            List<RegexMatch> colorMatches = GetMatches(source, colors.ToList());
            return SchemeFormatUtils.BatchReplace(source, colorMatches);
        }
        
        private List<RegexMatch> GetMatches(string text, IReadOnlyList<Color> colors)
        {
            // Encapsulate filtered colors and regex matches within list of RegexMatch'es
            var matches = _matches ?? Regex.Matches(text, RegExPattern);
            int i = 0;
            List<RegexMatch> colorMatches = new List<RegexMatch>();
            foreach (Match match in matches) {
                string rgbString = match.Groups["hex"].ToString(); 
                string filteredRgbString = HexRgb.ToRgbString(colors[i++], HexFormat);

                colorMatches.Add(new RegexMatch()
                {
                    Index = match.Groups["hex"].Index,
                    Length = match.Groups["hex"].Length,
                    MatchingString = rgbString,
                    ReplacementString = filteredRgbString
                });
            }
            
            return colorMatches;
        }

    }
}