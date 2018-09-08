using System;
using ColorSchemeInverter.Colors;

namespace ColorSchemeInverter.Filters
{
    public abstract class ColorFilter
    {
        public abstract ColorBase ApplyTo(ColorBase colorBase);
        public abstract override string ToString();
    }
}