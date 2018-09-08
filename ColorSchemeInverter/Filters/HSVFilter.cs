using System;
using System.Text;
using ColorSchemeInverter.Colors;

namespace ColorSchemeInverter.Filters
{
    public class HSVFilter : ColorFilter
    {
        private Func<HSV, object[], HSV> FilterDelegate { get; }
        private object[] Arguments { get; }

        public HSVFilter(Func<HSV, object[], HSV> filterDelegate, params object[] args)
        {
            FilterDelegate = filterDelegate;
            Arguments = args;
        }
        
        public override ColorBase ApplyTo(ColorBase color)
        {
            if (color is RGB) {
                return FilterDelegate(((RGB) color).ToHSV(), Arguments);
            } else if (color is HSV) {
                return FilterDelegate((HSV) color, Arguments);
            }else if (color is HSL) {
                return FilterDelegate(((HSL) color).ToHSV(), Arguments);
            }

            throw new NotImplementedException();
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