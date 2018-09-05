using System;
using ColorSchemeInverter.Colors;

namespace ColorSchemeInverter.Filters
{
    public class RGBFilter : ColorFilter
    {
        private Func<RGB, object[], RGB> Filter { get; }
        private object[] Arguments { get; }

        public RGBFilter(Func<RGB, object[], RGB> filter, object arg = null)
        {
            Filter = filter;
            if (arg != null)
                Arguments = new[] {arg};
        }

        public RGBFilter(Func<RGB, object[], RGB> filter, params object[] args)
        {
            Filter = filter;
            Arguments = args;
        }
        
        // todo auto cast back to subclass?
        public override Color ApplyTo(Color color)
        {
            if (color is RGB) {
                return Filter((RGB) color, Arguments);
            } else if (color is HSL) {
                return Filter(((HSL) color).ToRGB(), Arguments);
            }

            throw new NotImplementedException("Only HSL and RGB colors are supported");
        }
    }
}