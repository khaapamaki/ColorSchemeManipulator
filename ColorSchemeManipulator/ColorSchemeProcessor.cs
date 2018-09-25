using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using ColorSchemeManipulator.Colors;
using ColorSchemeManipulator.Common;
using ColorSchemeManipulator.Filters;
using ColorSchemeManipulator.SchemeFileSupport;

namespace ColorSchemeManipulator
{

    /// <summary>
    /// Processes text based color scheme files
    /// </summary>
    public class ColorSchemeProcessor
    {
        private readonly string _hexFormat;
        private readonly string _regExPattern;
        private readonly SchemeFormat _schemeFormat;

        /// <summary>
        /// A constructor that sets rgb hex format and regex pattern by the scheme format
        /// </summary>
        /// <param name="schemeFormat"></param>
        public ColorSchemeProcessor(SchemeFormat schemeFormat)
        {
            _schemeFormat = schemeFormat;
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

        /// <summary>
        /// Finds all color strings in a string using regex and applies filters to all of them
        /// </summary>
        /// <param name="text"></param>
        /// <param name="filters"></param>
        /// <returns>A string representing new scheme file with replaced colors</returns>
        private string ApplyFilters(string text, FilterSet filters)
        {      
            // Collect all colors from file to a list of colors
            List<Color> colorSet = new List<Color>();
            MatchCollection matches = Regex.Matches(text, _regExPattern);
            
            foreach (Match match in matches) {
                string rgbString = match.Groups["hex"].ToString();
                
                // todo better way to handle shorter of longer hex codes
                // this is needed for VS Code
                if (rgbString.Length > _hexFormat.Length)
                    rgbString = rgbString.Substring(0, _hexFormat.Length);
                
                colorSet.Add(HexRgb.FromRgbString(rgbString, _schemeFormat));
            }
            
            // Apply filters to the list of colors
            List<Color> filteredColors = filters.ApplyTo(colorSet).ToList();
            
            // Encapsulate filtered colors and regex matches within list of ColorMatch'es
            int i = 0;
            List<ColorMatch> colorMatches = new List<ColorMatch>();
            foreach (Match match in matches) {
                string rgbString = match.Groups["hex"].ToString(); 
                string filteredRgbString = HexRgb.ToRgbString(filteredColors[i++], _hexFormat);

                colorMatches.Add(new ColorMatch()
                {
                    Index = match.Groups["hex"].Index,
                    Length = match.Groups["hex"].Length,
                    MatchingString = rgbString,
                    ReplacementString = filteredRgbString
                });
            }

            // Make replacements
            text = BatchReplace(text, colorMatches);

            return text;
        }

        /// <summary>
        /// Performs manual replacement for matches with processed color replacement strings
        /// </summary>
        /// <param name="text"></param>
        /// <param name="colorMatches"></param>
        /// <returns></returns>
        private static string BatchReplace(string text, List<ColorMatch> colorMatches)
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
        
        /// <summary>
        /// Holds information of regex match to make replacements later
        /// </summary>
        private class ColorMatch
        {
            public int Index;
            public int Length;
            public string MatchingString;
            public string ReplacementString;
        }
    }
}