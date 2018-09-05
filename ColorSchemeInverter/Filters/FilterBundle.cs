using System.Linq;
using ColorSchemeInverter.Colors;

namespace ColorSchemeInverter.Filters
{
    public class FilterBundle
    {
        public static HSL LightnessInvert(HSL hsl, object[] _)
        {
            var result = new HSL(hsl);
            result.Lightness = 1.0 - result.Lightness;
            return result;
        }

        public static HSL LightnessGain(HSL hsl, object[] args)
        {
            var result = new HSL(hsl);
            if (args.Any() && args[0] is double gain) {
                result.Lightness = (result.Lightness * gain);
            }
            return result;
        }
        
        public static HSL SaturationGain(HSL hsl, object[] args)
        {
            var result = new HSL(hsl);
            if (args.Any() && args[0] is double gain) {
                result.Saturation = (result.Saturation * gain);
            }
            return result;
        }
        
        public static HSL SaturationGamma(HSL hsl, object[] args)
        {
            var result = new HSL(hsl);
            if (args.Any() && args[0] is double gamma) {
                result.Saturation = ColorMath.Gamma(hsl.Saturation, gamma);
            }
            return result;
        }
        
        public static HSL LightnessGamma(HSL hsl, object[] args)
        {
            var result = new HSL(hsl);
            if (args.Any() && args[0] is double gamma) {
                result.Lightness = ColorMath.Gamma(hsl.Lightness, gamma);
            }
            return result;
        }
        
        public static HSL SaturationContrast(HSL hsl, object[] args)
        {
            var result = new HSL(hsl);
            if (args.Any() && args[0] is double strength) {
                if (args.Length >= 2 && args[1] is double midpoint) {
                    result.Saturation = ColorMath.SSpline(hsl.Saturation, strength, midpoint);
                } else {
                    result.Saturation = ColorMath.SSpline(hsl.Saturation, strength);
                }
            }        
            return result;
        }
       
        public static HSL LightnessContrast(HSL hsl, object[] args)
        {
            var result = new HSL(hsl);
            if (args.Any() && args[0] is double strength) {
                if (args.Length >= 2 && args[1] is double midpoint) {
                    result.Lightness = ColorMath.SSpline(hsl.Lightness, strength, midpoint);
                } else {
                    result.Lightness = ColorMath.SSpline(hsl.Lightness, strength);
                }
            }        
            return result;
        }
        
        public static RGB Contrast(RGB hsl, object[] args)
        {
            var result = new RGB(hsl);
            if (args.Any() && args[0] is double strength) {
                if (args.Length >= 2 && args[1] is double midpoint) {
                    result.Red = ColorMath.SSpline(hsl.Red, strength, midpoint);
                    result.Green = ColorMath.SSpline(hsl.Green, strength, midpoint);
                    result.Blue = ColorMath.SSpline(hsl.Blue, strength, midpoint);
                } else {
                    result.Red = ColorMath.SSpline(hsl.Red, strength);
                    result.Green = ColorMath.SSpline(hsl.Green, strength);
                    result.Blue = ColorMath.SSpline(hsl.Blue, strength);
                }
            }        
            return result;
        }
        
        public static RGB Gamma(RGB rgb, object[] args)
        {
            var result = new RGB(rgb);
            if (args.Any() && args[0] is double strength) {
                    result.Red = ColorMath.Gamma(rgb.Red, strength);
                    result.Green = ColorMath.Gamma(rgb.Green, strength);
                    result.Blue = ColorMath.Gamma(rgb.Blue, strength);
            }        
            return result;
        }
        
        public static RGB Gain(RGB rgb, object[] args)
        {
            var result = new RGB(rgb);
            if (args.Any() && args[0] is double strength) {
                result.Red = ColorMath.Gain(rgb.Red, strength);
                result.Green = ColorMath.Gain(rgb.Green, strength);
                result.Blue = ColorMath.Gain(rgb.Blue, strength);
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
            return new RGB(rgb) { Red = 1.0 - rgb.Red, Green = 1.0 - rgb.Green, Blue = 1.0 - rgb.Blue };
        }
    }
}