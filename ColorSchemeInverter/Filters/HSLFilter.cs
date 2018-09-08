using System;
using System.Text;
using ColorSchemeInverter.Colors;

namespace ColorSchemeInverter.Filters
{
    public class HSLFilter : ColorFilter
    {
        private Func<HSL, object[], HSL> FilterDelegate { get; }
        private object[] Arguments { get; }

        public HSLFilter(Func<HSL, object[], HSL> filterDelegate, params object[] args)
        {
            FilterDelegate = filterDelegate;
            Arguments = args;
        }

        public override ColorBase ApplyTo(ColorBase color)
        {
            if (color is RGB) {
                return FilterDelegate(((RGB) color).ToHSL(), Arguments);
            } else if (color is HSL) {
                return FilterDelegate((HSL) color, Arguments);
            }else if (color is HSV) {
                return FilterDelegate(((HSV) color).ToHSL(), Arguments);
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