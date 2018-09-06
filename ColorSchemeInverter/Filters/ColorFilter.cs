using System;
using ColorSchemeInverter.Colors;

namespace ColorSchemeInverter.Filters
{
    public abstract class ColorFilter
    {
        public abstract Color ApplyTo(Color color);
        public abstract override string ToString();
    }
}