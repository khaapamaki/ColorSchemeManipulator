using System;
using System.Collections.Generic;
using System.Diagnostics;
using ColorSchemeManipulator.Colors;
using ColorSchemeManipulator.Filters;
using ColorSchemeManipulator.SchemeFormats.Handlers;

namespace ColorSchemeManipulator
{
    /// <summary>
    /// Generic processor for any supported file type. Uses external handler class
    /// that that implements IColorFileHandler to perform all needed actions.
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
            Console.WriteLine("Applying filters:");
            
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
            // Fetch all colors
            IEnumerable<Color> colors = _handler.GetColors(source);
            
            // Apply filters
            IEnumerable<Color> filteredColors = filters.ApplyTo(colors);
// #if DEBUG            
            var watch = new Stopwatch();
            watch.Start();
// #endif
            // replace original colors with filtered ones
            var result = _handler.ReplaceColors(source, filteredColors);
// #if DEBUG            
            watch.Stop();
            Console.WriteLine("Color processed in " + watch.ElapsedMilliseconds + "ms");
// #endif
            return result;
        }
   
    }
    
}