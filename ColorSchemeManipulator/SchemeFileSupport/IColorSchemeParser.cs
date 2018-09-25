using System.Collections.Generic;
using ColorSchemeManipulator.Colors;
using ColorSchemeManipulator.Filters;

namespace ColorSchemeManipulator.SchemeFileSupport
{
    public interface IColorSchemeParser<T>
    {
        // test before converting to IEnumerable!
        List<Color> GetColors(T source);
        List<ColorMatch> GetMatches(T source, List<Color> colors);
        T ReplaceColors(T source, List<ColorMatch> colorMatches);
    }
}