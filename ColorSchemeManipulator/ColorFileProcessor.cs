using System;
using System.Collections.Generic;
using ColorSchemeManipulator.Colors;
using ColorSchemeManipulator.Filters;
using ColorSchemeManipulator.SchemeFormats.Handlers;

namespace ColorSchemeManipulator
{

    /// <summary>
    /// Generic processor for any supported file type. Uses IColorFileHandler class to perform all needed actions.
    /// </summary>
    public class ColorFileProcessor<T>
    {
        private readonly IColorFileHandler<T> _handler;

        public ColorFileProcessor(IColorFileHandler<T> handler)
        {
            _handler = handler;
        }

        private ColorFileProcessor() { }
        
        public void ProcessFile(string sourceFile, string targetFile, FilterSet filters)
        {
            var data = _handler.ReadFile(sourceFile);
            T filteredData;

            try {
                filteredData = ApplyFilters(data, filters);
            } catch (Exception ex) {
                Console.WriteLine(GetType().FullName + " : " + ex.Message);
                throw;
            }

            _handler.WriteFile(filteredData,targetFile);
        }

        private T ApplyFilters(T source, FilterSet filters)
        {      
            // test IEnumerable!
            IEnumerable<Color> colors = _handler.GetColors(source);
            
            // Apply filters to the list of colors
            IEnumerable<Color> filteredColors = filters.ApplyTo(colors);
  
            return _handler.ReplaceColors(source, filteredColors);
        }
   
    }
    
}