using System.Collections.Generic;
using ColorSchemeManipulator.Colors;

namespace ColorSchemeManipulator.SchemeFormats.Handlers
{
    public interface IColorFileHandler<T>
    {
        bool Accepts(string sourceFile);
        T ReadFile(string sourceFile);
        void WriteFile(T data, string targetFile);   
        IEnumerable<Color> GetColors(T source);
        T ReplaceColors(T xml, IEnumerable<Color> colors);
    }
}