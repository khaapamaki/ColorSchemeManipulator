using System;
using System.Text;
using ColorSchemeInverter.Colors;

namespace ColorSchemeInverter.Filters
{
    public class RgbFilter : ColorFilter
    {
        private Func<Rgb, object[], Rgb> FilterDelegate { get; }
        private object[] Arguments { get; }

        public RgbFilter(Func<Rgb, object[], Rgb> filterDelegate, params object[] args)
        {
            FilterDelegate = filterDelegate;
            Arguments = args;
        }

        public override ColorBase ApplyTo(ColorBase color)
        {
            if (color is Rgb) {
                return FilterDelegate((Rgb) color, Arguments);
            } else if (color is Hsl) {
                return FilterDelegate(((Hsl) color).ToRgb(), Arguments);
            } else if (color is Hsv) {
                return FilterDelegate(((Hsv) color).ToRgb(), Arguments);
            }

            throw new NotImplementedException();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var argument in Arguments) {
                sb.Append(argument + " ");
            }

            return FilterDelegate.Method.Name + " " + sb;
        }
    }
}