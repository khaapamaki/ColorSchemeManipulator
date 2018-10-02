using System.Collections.Generic;
using ColorSchemeManipulator.Colors;

namespace ColorSchemeManipulator.SchemeFormats.Handlers
{
    public interface IColorFileHandler<TData>
    {
        // Checks a file if can processed with the handler
        bool Accepts(string sourceFile);
        
        // Read the contents of the file
        TData ReadFile(string sourceFile);
        
        // Write the contents to a file
        void WriteFile(TData data, string targetFile);
        
        // Parses and enumerates all the color from the file
        IEnumerable<Color> GetColors(TData source);
        
        // Replaces original color values from the data with new colors (must be in the same order)
        // Return altered data
        TData ReplaceColors(TData data, IEnumerable<Color> colors);
    }
}
