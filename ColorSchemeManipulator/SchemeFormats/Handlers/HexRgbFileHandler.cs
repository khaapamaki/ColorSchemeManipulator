using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ColorSchemeManipulator.Colors;

namespace ColorSchemeManipulator.SchemeFormats.Handlers
{
    public abstract class HexRgbFileHandler : IColorFileHandler<string>
    {
        protected abstract string RegexPattern { get; }
        protected abstract PaddableHexFormat[] InputHexFormats { get; }
        protected abstract string MatchGroupName { get; }
        protected abstract string OutputHexFormat { get; }

        private MatchCollection _matches;

        public virtual string ReadFile(string sourceFile)
        {
            return File.ReadAllText(sourceFile);
        }

        public virtual void WriteFile(string text, string targetFile)
        {
            File.WriteAllText(targetFile, text, Encoding.Default);
        }

        public virtual IEnumerable<Color> GetColors(string text)
        {
            var matches = Regex.Matches(text, RegexPattern);
            _matches = matches;
            foreach (Match match in matches) {
                string rgbString = match.Groups[MatchGroupName].ToString();
                yield return SchemeUtils.PaddableHexStringToColor(rgbString, InputHexFormats);
            }
        }

        public virtual string ReplaceColors(string xml, IEnumerable<Color> colors)
        {
            List<RegexReplacement> colorMatches = GetMatches(xml, colors.ToList());
            return SchemeUtils.BatchReplace(xml, colorMatches);
        }

        private List<RegexReplacement> GetMatches(string text, IReadOnlyList<Color> colors)
        {
            // Encapsulate filtered colors and regex matches within list of RegexReplacements
            var matches = _matches ?? Regex.Matches(text, RegexPattern);
            int i = 0;
            List<RegexReplacement> colorMatches = new List<RegexReplacement>();
            foreach (Match match in matches) {
                string rgbString = match.Groups[MatchGroupName].ToString();
                string filteredRgbString = HexRgbUtil.ColorToHexString(colors[i++], OutputHexFormat);

                colorMatches.Add(new RegexReplacement()
                {
                    Index = match.Groups[MatchGroupName].Index,
                    Length = match.Groups[MatchGroupName].Length,
                    MatchingString = rgbString,
                    ReplacementString = filteredRgbString
                });
            }

            return colorMatches;
        }
    }
}