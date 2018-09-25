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
    public class ColorSchemeProcessor<T>
    {
        // private readonly string _hexFormat;
        // private readonly string _regExPattern;
        // private readonly SchemeFormat _schemeFormat;
        private readonly IColorSchemeParser<T> _parser;

        /// <summary>
        /// A constructor that sets rgb hex format and regex pattern by the scheme format
        /// </summary>
        public ColorSchemeProcessor(IColorSchemeParser<T> parser)
        {
            _parser = parser;
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
        /// <param name="source"></param>
        /// <param name="filters"></param>
        /// <returns>A string representing new scheme file with replaced colors</returns>
        private T ApplyFilters(T source, FilterSet filters)
        {      
            // Collect all colors from file to a list of colors
            // test before converting to IEnumerable!
            List<Color> colors = _parser.GetColors(source);
            
            // Apply filters to the list of colors
            List<Color> filteredColors = filters.ApplyTo(colors).ToList();
  
            List<ColorMatch> matches =  _parser.GetMatches(source, filteredColors);
            return _parser.ReplaceColors(source, matches);
        }
   
    }
    
}