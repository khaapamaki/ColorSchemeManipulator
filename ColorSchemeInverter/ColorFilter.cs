using System;

namespace ColorSchemeInverter
{
    public abstract class ColorFilter
    {
        public abstract Color ApplyTo(Color color);
    }
}