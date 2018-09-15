using System.Collections.Generic;
using ColorSchemeManipulator.Colors;

namespace ColorSchemeManipulator.Filters
{
    public interface IFilterable
    {
        IEnumerable<ColorBase> GetColors();
        void Add(ColorBase color);
    }
}