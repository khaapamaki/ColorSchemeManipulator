using System;
using ColorSchemeInverter.Colors;

namespace ColorSchemeInverter.Filters
{
    public class HSLFilter : ColorFilter
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

        // todo auto cast back to subclass?
        public override Color ApplyTo(Color color)
        {
            if (color is RGB) {
                return Filter(((RGB) color).ToHSL(), Arguments);
            } else if (color is HSL) {
                return Filter((HSL) color, Arguments);
            }

            throw new NotImplementedException("Only HSL and RGB colors are supported");
        }
    }
}