using System;
using System.Text;
using ColorSchemeInverter.Colors;

namespace ColorSchemeInverter.Filters
{
    public abstract class ColorFilter
    {
        public abstract ColorBase ApplyTo(ColorBase color);
        public abstract override string ToString();
    }
}