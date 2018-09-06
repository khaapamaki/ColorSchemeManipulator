using System;
using System.Text;
using ColorSchemeInverter.Colors;

namespace ColorSchemeInverter.Filters
{
    public class HSLFilter : ColorFilter
    {
        private Func<HSL, object[], HSL> FilterDelegate { get; }
        private object[] Arguments { get; }
        
        public HSLFilter(Func<HSL, object[], HSL> filterDelegate, object arg = null)
        {
            FilterDelegate = filterDelegate;
            if (arg != null)
                Arguments = new [] {arg};
        }
        
        public HSLFilter(Func<HSL, object[], HSL> filterDelegate, params object[] args)
        {
            FilterDelegate = filterDelegate;
            Arguments = args;
        }

        public HSLFilter(Func<Color, object[], Color> filterDelegate, object arg = null)
        {
            FilterDelegate = (Func<HSL, object[], HSL>) filterDelegate;
            if (arg != null)
                Arguments = new [] {arg};
        }
        
        public HSLFilter(Func<Color, object[], Color> filterDelegate, params object[] args)
        {
            FilterDelegate = (Func<HSL, object[], HSL>) filterDelegate;
            Arguments = args;
        }
        
       
        public override Color ApplyTo(Color color)
        {
            if (color is RGB) {
                return FilterDelegate(((RGB) color).ToHSL(), Arguments);
            } else if (color is HSL) {
                return FilterDelegate((HSL) color, Arguments);
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