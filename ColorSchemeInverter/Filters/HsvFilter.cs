using System;
using System.Text;
using ColorSchemeInverter.Colors;

namespace ColorSchemeInverter.Filters
{
    public class HsvFilter : ColorFilter
    {
        private Func<Hsv, object[], Hsv> FilterDelegate { get; }
        private object[] Arguments { get; }

        public HsvFilter(Func<Hsv, object[], Hsv> filterDelegate, params object[] args)
        {
            FilterDelegate = filterDelegate;
            Arguments = args;
        }
        
        public override ColorBase ApplyTo(ColorBase color)
        {
            if (color is Rgb) {
                return FilterDelegate(((Rgb) color).ToHsv(), Arguments);
            } else if (color is Hsv) {
                return FilterDelegate((Hsv) color, Arguments);
            }else if (color is Hsl) {
                return FilterDelegate(((Hsl) color).ToHsv(), Arguments);
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