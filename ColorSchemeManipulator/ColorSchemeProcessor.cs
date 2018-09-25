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
        private readonly IColorDataHandler<T> _handler;

        /// <summary>
        /// A constructor that sets rgb hex format and regex pattern by the scheme format
        /// </summary>
        public ColorSchemeProcessor(IColorDataHandler<T> handler)
        {
            _handler = handler;
        }

        public void ProcessFile(string sourceFile, string targetFile, FilterSet filters)
        {
            T data = _handler.ReadFile(sourceFile);
            T filteredData;

            try {
                filteredData = ApplyFilters(data, filters);
            } catch (Exception ex) {
                Console.WriteLine(GetType().FullName + " : " + ex.Message);
                throw;
            }

            _handler.WriteFile(filteredData,targetFile);
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
            List<Color> colors = _handler.GetColors(source);
            
            // Apply filters to the list of colors
            List<Color> filteredColors = filters.ApplyTo(colors).ToList();
  
            return _handler.ReplaceColors(source, filteredColors);
        }
   
    }
    
}