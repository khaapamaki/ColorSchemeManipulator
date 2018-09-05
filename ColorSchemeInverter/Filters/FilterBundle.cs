using System.Linq;
using ColorSchemeInverter.Colors;

namespace ColorSchemeInverter.Filters
{
    public class FilterBundle
    {
        public static HSL LightnessInvert(HSL hsl, object[] _)
        {
            var result = new HSL(hsl);
            // Todo: remove clamping when values over 1.0 and less 0.0 are safely converted to 8 bit RGB
            result.Lightness = (1.0 - result.Lightness).Clamp(0.0, 1.0);
            return result;
        }

        public static HSL SaturationGain(HSL hsl, object[] args)
        {
            var result = new HSL(hsl);
            if (args.Any() && args[0] is double multiplier) {
                // Todo: remove clamping when values over 1.0 and less 0.0 are safely converted to 8 bit RGB
                result.Saturation = (result.Saturation * multiplier).Clamp(0.0, 1.0);
            }
            return result;
        }
        
        public static RGB DoNothing(RGB rgb, object[] args)
        {
            var result = new RGB(rgb);
            // do something here
            return result;
        }
        public static RGB Invert(RGB rgb, object[] args)
        {
            return new RGB(rgb) {Red = 1.0 - rgb.Red, Green = 1.0 - rgb.Green, Blue = 1.0 - rgb.Blue};
        }
    }
}