using System;
using System.Collections.Generic;

namespace ColorSchemeInverter
{
    public class HSLFilter
    {
        private Func<HSL, object[], HSL> Filter { get; }
        private object[] Arguments { get; }
        
        public HSLFilter(Func<HSL, object[], HSL> filter, object arg = null)
        {
            Filter = filter;
            if (arg != null)
                Arguments = new [] {arg};
        }
        
        public HSLFilter(Func<HSL, object[], HSL> filter, params object[] args)
        {
            Filter = filter;
            Arguments = args;
        }

        public HSL ApplyTo(HSL hsl)
        {
            return Filter(hsl, Arguments);
        }
    }
}