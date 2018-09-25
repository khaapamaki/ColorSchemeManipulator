using System.Collections.Generic;
using ColorSchemeManipulator.Colors;
using ColorSchemeManipulator.Filters;

namespace ColorSchemeManipulator.SchemeFileSupport
{
    public interface IColorDataHandler<T>
    {
        T ReadFile(string sourceFile);
        void WriteFile(T data, string targetFile);
        
        // test before converting to IEnumerable!
        List<Color> GetColors(T source);
        // List<ColorMatch> GetMatches(T source, List<Color> colors);
        T ReplaceColors(T source, List<Color> colors);
    }
}