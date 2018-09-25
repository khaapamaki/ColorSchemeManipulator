using System.Collections.Generic;
using ColorSchemeManipulator.Colors;
using ColorSchemeManipulator.Filters;

namespace ColorSchemeManipulator.SchemeFileSupport
{
    public interface IColorFileHandler<T>
    {
        T ReadFile(string sourceFile);
        void WriteFile(T data, string targetFile);   
        IEnumerable<Color> GetColors(T source);
        T ReplaceColors(T source, IEnumerable<Color> colors);
    }
}