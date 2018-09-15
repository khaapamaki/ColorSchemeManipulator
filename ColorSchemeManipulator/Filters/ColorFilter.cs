using System;
using ColorSchemeManipulator.Colors;

namespace ColorSchemeManipulator.Filters
{
    [Obsolete]
    public abstract class ColorFilter
    {
        public abstract ColorBase ApplyTo(ColorBase color);
        public abstract override string ToString();
    }
}