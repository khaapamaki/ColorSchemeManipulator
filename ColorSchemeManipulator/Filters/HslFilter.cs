using System;
using System.Text;
using ColorSchemeManipulator.Colors;

namespace ColorSchemeManipulator.Filters
{
    public class HslFilter : ColorFilter
    {
        private Func<Hsl, object[], Hsl> FilterDelegate { get; }
        private object[] Arguments { get; }

        public HslFilter(Func<Hsl, object[], Hsl> filterDelegate, params object[] args)
        {
            FilterDelegate = filterDelegate;
            Arguments = args;
        }

        public override ColorBase ApplyTo(ColorBase color)
        {
            if (color is Rgb) {
                return FilterDelegate(((Rgb) color).ToHsl(), Arguments);
            } else if (color is Hsl) {
                return FilterDelegate((Hsl) color, Arguments);
            } else if (color is Hsv) {
                return FilterDelegate(((Hsv) color).ToHsl(), Arguments);
            }

            throw new NotImplementedException("Only HSL and RGB colors are supported");
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var argument in Arguments) {
                sb.Append(argument.ToString() + " ");
            }

            return FilterDelegate.Method.Name + " " + sb;
        }
    }
}