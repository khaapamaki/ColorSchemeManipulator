using System;
using System.Text;
using ColorSchemeInverter.Colors;

namespace ColorSchemeInverter.Filters
{
    public class RGBFilter : ColorFilter
    {
        private Func<RGB, object[], RGB> FilterDelegate { get; }
        private object[] Arguments { get; }

        public RGBFilter(Func<RGB, object[], RGB> filterDelegate, object arg = null)
        {
            FilterDelegate = filterDelegate;
            if (arg != null)
                Arguments = new[] {arg};
        }

        public RGBFilter(Func<RGB, object[], RGB> filterDelegate, params object[] args)
        {
            FilterDelegate = filterDelegate;
            Arguments = args;
        }
        
        // todo auto cast back to subclass?
        public override ColorBase ApplyTo(ColorBase colorBase)
        {
            if (colorBase is RGB) {
                return FilterDelegate((RGB) colorBase, Arguments);
            } else if (colorBase is HSL) {
                return FilterDelegate(((HSL) colorBase).ToRGB(), Arguments);
            }

            throw new NotImplementedException("Only HSL and RGB colors are supported");
        }
        
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var argument in Arguments) {
                sb.Append(argument + " ");
            }

            return FilterDelegate.Method.Name + " " + sb;
        }
    }
}