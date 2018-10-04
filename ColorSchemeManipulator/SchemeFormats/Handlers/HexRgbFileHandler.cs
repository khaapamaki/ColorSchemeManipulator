using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ColorSchemeManipulator.Colors;
using ColorSchemeManipulator.Common;

namespace ColorSchemeManipulator.SchemeFormats.Handlers
{
    public abstract class HexRgbFileHandler : IColorFileHandler<string>
    {

        public abstract bool Accepts(string sourceFile);

        public virtual string ReadFile(string sourceFile)
        {
            return File.ReadAllText(sourceFile);
        }

        public virtual void WriteFile(string text, string targetFile)
        {
            File.WriteAllText(targetFile, text, Encoding.Default);
        }

        protected abstract string RegexPattern { get; }
        protected abstract PaddableHexFormat[] InputHexFormats { get; }
        protected abstract string MatchGroupName { get; }
        protected abstract string OutputHexFormat { get; }
               
        private MatchCollection _matches;
        
        double sourceMin = double.MaxValue;
        double sourceMax = double.MinValue;

        public virtual IEnumerable<Color> GetColors(string text)
        {
            sourceMin = double.MaxValue;
            sourceMax = double.MinValue;
            
            var matches = Regex.Matches(text, RegexPattern);
            _matches = matches;
            foreach (Match match in matches) {
                string rgbString = match.Groups[MatchGroupName].ToString();
                var color = PaddableHexFormat.PaddableHexStringToColor(rgbString, InputHexFormats);
                sourceMin = color.CompareValue() < sourceMin ? color.CompareValue() : sourceMin;
                sourceMax = color.CompareValue() > sourceMax ? color.CompareValue() : sourceMax;
                yield return color;
            }
        }

        double resultMin = double.MaxValue;
        double resultMax = double.MinValue;

        public virtual string ReplaceColors(string xml, IEnumerable<Color> colors)
        {
            resultMin = double.MaxValue;
            resultMax = double.MinValue;
            
            List<Color> colorList = colors.ToList();
            
            foreach (var color in colorList) {
                resultMin = color.CompareValue() < resultMin ? color.CompareValue() : resultMin;
                resultMax = color.CompareValue() > resultMax ? color.CompareValue() : resultMax;
            }
            
            List<RegexReplacement> colorMatches = GetMatches(xml, colorList);
                      
            return BatchReplace(xml, colorMatches);
        }

        private List<RegexReplacement> GetMatches(string text, IReadOnlyList<Color> colors)
        {
            // Encapsulate filtered colors and regex matches within list of RegexReplacements
            var matches = _matches ?? Regex.Matches(text, RegexPattern);
            int i = 0;
            List<RegexReplacement> colorMatches = new List<RegexReplacement>();
            
            foreach (Match match in matches) {
                string rgbString = match.Groups[MatchGroupName].ToString();
                string filteredRgbString = HexRgbUtil.ColorToHexString(colors[i], OutputHexFormat);
                
                var sourceColor = PaddableHexFormat.PaddableHexStringToColor(rgbString, InputHexFormats);
                var resultColor = colors[i];
                var minmax = new StringBuilder();
                if (sourceColor.CompareValue().AboutEqual(sourceMax))
                    minmax.Append($"<Source Max {sourceColor.CompareValue():F3}> ");
                if (sourceColor.CompareValue().AboutEqual(sourceMin))
                    minmax.Append($"<Source Min {sourceColor.CompareValue():F3}> ");
                if (resultColor.CompareValue().AboutEqual(resultMax))
                    minmax.Append($"<Result Max {resultColor.CompareValue():F3}> ");
                if (resultColor.CompareValue().AboutEqual(resultMin))
                    minmax.Append($"<Result Min {resultColor.CompareValue():F3}> ");
                              
                if (match.Groups["attr"] != null && match.Groups["attr"].Value != "") {
                    Console.WriteLine(match.Groups["attr"].Value);
                }
                
                string subAttr = match.Groups["attr2"]?.Value ?? ""; 
                Console.WriteLine($"  {subAttr,-30} #{rgbString} -> #{filteredRgbString} {minmax}");
                
                colorMatches.Add(new RegexReplacement()
                {
                    Index = match.Groups[MatchGroupName].Index,
                    Length = match.Groups[MatchGroupName].Length,
                    MatchingString = rgbString,
                    ReplacementString = filteredRgbString
                });

                i++;
            }

            Console.WriteLine($"\n{i} colors affected\n");
            Console.WriteLine($"SOURCE: Min: {sourceMin:F3} Max: {sourceMax:F3}");
            Console.WriteLine($"RESULT: Min: {resultMin:F3} Max: {resultMax:F3}");
            return colorMatches;
        }
        
        // todo move to Utils or somewhere else to be used with other classes
        
        /// <summary>
        /// Performs manual replacement for matches with processed color replacement strings
        /// </summary>
        /// <param name="text"></param>
        /// <param name="colorMatches"></param>
        /// <returns></returns>
        private static string BatchReplace(string text, List<RegexReplacement> colorMatches)
        {
            // matches must be in reverse order by indexes, otherwise replacing with strings
            // of which lengths differ from original's will make latter indexes invalid
            colorMatches = colorMatches.OrderByDescending(m => m.Index).ToList();
            
            foreach (var match in colorMatches) {
                text = text.ReplaceWithin(match.Index, match.Length, match.ReplacementString);
                //Console.WriteLine(match.MatchingString + " -> " +  match.ReplacementString);
            }

            return text;
        }
    }
}