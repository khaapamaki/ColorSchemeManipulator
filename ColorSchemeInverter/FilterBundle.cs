using System;
using System.Linq;

namespace ColorSchemeInverter
{
    public class FilterBundle
    {
        public static HSL InvertLightness(HSL hsl, object[] _)
        {
            var result = new HSL(hsl);
            // Todo: remove clamping when values over 1.0 and less 0.0 are safely converted to 8 bit RGB
            result.Lightness = (1.0 - result.Lightness).Clamp(0.0, 1.0);
            return result;
        }

        public static HSL MultiplySaturation(HSL hsl, object[] args)
        {
            var result = new HSL(hsl);
            if (args.Any() && args[0] is double multiplier) {
                // Todo: remove clamping when values over 1.0 and less 0.0 are safely converted to 8 bit RGB
                result.Saturation = (result.Saturation * multiplier).Clamp(0.0, 1.0);
            }
            return result;
        }
        
        public static HSL DoNothing(HSL hsl, object[] args)
        {
            var result = new HSL(hsl);
            // do something here
            return result;
        }
    }
}