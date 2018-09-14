using System;
using ColorSchemeManipulator.Common;
using ColorSchemeManipulator.Filters;

namespace ColorSchemeManipulator.Colors
{
    public class Rgb : ColorBase
    {
        public double Red { get; set; }
        public double Green { get; set; }
        public double Blue { get; set; }
        public double Alpha { get; set; } = 1.0;

        private Rgb() { }

        public Rgb(double red, double green, double blue, double alpha = 1.0)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        public Rgb(Rgb8Bit rgb8Bit)
        {
            Red = rgb8Bit.Red / 255.0;
            Green = rgb8Bit.Green / 255.0;
            Blue = rgb8Bit.Blue / 255.0;
            Alpha = rgb8Bit.Alpha / 255.0;
        }

        public Rgb(Rgb rgb)
        {
            Red = rgb.Red;
            Green = rgb.Green;
            Blue = rgb.Blue;
            Alpha = rgb.Alpha;
        }
        
        public Rgb(Hsl hsl)
        {
            CopyFrom(hsl.ToRgb());
        }

        public Rgb(Hsv hsv)
        {
            CopyFrom(hsv.ToRgb());
        }
      
        
        public void CopyFrom(Rgb rgb)
        {
            Red = rgb.Red;
            Green = rgb.Green;
            Blue = rgb.Blue;
            Alpha = rgb.Alpha;
        }

        public static Rgb FromRgbString(string rgbString, string rgbHexFormat)
        {
            return Rgb8Bit.FromRgbString(rgbString, rgbHexFormat).ToRgb();
        }

        public static Rgb FromRgbString(string rgbString)
        {
            return Rgb8Bit.FromRgbString(rgbString).ToRgb();
        }

        public static Rgb FromArgbString(string rgbString)
        {
            return Rgb8Bit.FromArgbString(rgbString).ToRgb();
        }

        public static Rgb FromRgbaString(string rgbString)
        {
            return Rgb8Bit.FromRgbaString(rgbString).ToRgb();
        }

        public string ToRgbString()
        {
            return ToRgb8Bit().ToRgbString();
        }

        public string ToArgbString()
        {
            return ToRgb8Bit().ToArgbString();
        }

        public string ToRgbaString()
        {
            return ToRgb8Bit().ToRgbaString();
        }

        public Rgb8Bit ToRgb8Bit()
        {
            return new Rgb8Bit(
                Red.Clamp(0.0, 1.0),
                Green.Clamp(0.0, 1.0),
                Blue.Clamp(0.0, 1.0),
                Alpha.Clamp(0.0, 1.0));
        }

        public Rgb Interpolate(Rgb rgb, double factor)
        {
            factor = factor.Clamp(0, 1);
            Rgb result = new Rgb();
            result.Red = ColorMath.LinearInterpolation(factor, Red, rgb.Red);
            result.Green = ColorMath.LinearInterpolation(factor,Green, rgb.Green);
            result.Blue = ColorMath.LinearInterpolation(factor,Blue, rgb.Blue);
            result.Alpha = ColorMath.LinearInterpolation(factor,Alpha, rgb.Alpha);
            return result;
        }

        public string ToRgbString(string rgbHexFormat)
        {
            return ToRgb8Bit().ToRgbString(rgbHexFormat);
        }

        public override string ToString()
        {
            return string.Format($"Red: {Red}, Green: {Green}, Blue: {Blue}, Alpha: {Alpha}");
        }

        public string ToString(string format)
        {
            if (format.ToUpper() == "X2") {
                return string.Format($"Red: 0x{Red * 255:X2}, " +
                                     $"Green: 0x{Green * 255:X2}, " +
                                     $"Blue: 0x{Blue * 255:X2} " +
                                     $"Alpha: 0x{Alpha * 255:X2}");
            } else {
                throw new FormatException("Invalid Format String: " + format);
                //return ToString();
            }
        }

        public bool Equals(Rgb c)
        {
            bool value = Red.AboutEqual(c.Red) && Green.AboutEqual(c.Green) && Blue.AboutEqual(c.Blue) &&
                         Alpha.AboutEqual(c.Alpha);
            return value;
        }

        public Rgb ApplyFilterSet(FilterSet filters)
        {
            return filters.ApplyTo(this);
        }

        public Hsl ApplyFilter(HslFilter filter)
        {
            return filter.ApplyTo(this).ToHsl();
        }

        public Rgb ApplyFilter(RgbFilter filter)
        {
            return filter.ApplyTo(this).ToRgb();
        }
    }
}