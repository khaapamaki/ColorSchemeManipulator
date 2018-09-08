using System;
using ColorSchemeInverter.Common;
using ColorSchemeInverter.Filters;

namespace ColorSchemeInverter.Colors
{
    public class RGB : ColorBase
    {
        public double Red { get; set; }
        public double Green { get; set; }
        public double Blue { get; set; }
        public double Alpha { get; set; } = 1.0;

        private RGB() { }

        public RGB(double red, double green, double blue, double alpha = 1.0)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        public RGB(RGB8bit rgb8Bit)
        {
            Red = rgb8Bit.Red / 255.0;
            Green = rgb8Bit.Green / 255.0;
            Blue = rgb8Bit.Blue / 255.0;
            Alpha = rgb8Bit.Alpha / 255.0;
        }

        public RGB(RGB rgb)
        {
            Red = rgb.Red;
            Green = rgb.Green;
            Blue = rgb.Blue;
            Alpha = rgb.Alpha;
        }
        
        public RGB(HSL hsl)
        {
            CopyFrom(hsl.ToRGB());
        }

        public RGB(HSV hsv)
        {
            CopyFrom(hsv.ToRGB());
        }
      
        
        public void CopyFrom(RGB rgb)
        {
            Red = rgb.Red;
            Green = rgb.Green;
            Blue = rgb.Blue;
            Alpha = rgb.Alpha;
        }

        public static RGB FromRGBString(string rgbString, string rgbStringFormat)
        {
            return RGB8bit.FromRGBString(rgbString, rgbStringFormat).ToRGB();
        }

        public static RGB FromRGBString(string rgbString)
        {
            return RGB8bit.FromRGBString(rgbString).ToRGB();
        }

        public static RGB FromARGBString(string rgbString)
        {
            return RGB8bit.FromARGBString(rgbString).ToRGB();
        }

        public static RGB FromRGBAString(string rgbString)
        {
            return RGB8bit.FromRGBAString(rgbString).ToRGB();
        }

        public string ToRGBString()
        {
            return ToRGB8Bit().ToRGBString();
        }

        public string ToARGBString()
        {
            return ToRGB8Bit().ToARGBString();
        }

        public string ToRGBAString()
        {
            return ToRGB8Bit().ToRGBAString();
        }

        public RGB8bit ToRGB8Bit()
        {
            return new RGB8bit(
                Red.Clamp(0.0, 1.0),
                Green.Clamp(0.0, 1.0),
                Blue.Clamp(0.0, 1.0),
                Alpha.Clamp(0.0, 1.0));
        }



        public string ToRGBString(string rgbStringFormat)
        {
            return ToRGB8Bit().ToRGBString(rgbStringFormat);
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
                return ToString();
            }
        }

        public bool Equals(RGB c)
        {
            bool value = Red.AboutEqual(c.Red) && Green.AboutEqual(c.Green) && Blue.AboutEqual(c.Blue) &&
                         Alpha.AboutEqual(c.Alpha);
            return value;
        }

        public RGB ApplyFilterSet(FilterSet filters)
        {
            return filters.ApplyTo(this);
        }

        public HSL ApplyFilter(HSLFilter filter)
        {
            return filter.ApplyTo(this).ToHSL();
        }

        public RGB ApplyFilter(RGBFilter filter)
        {
            return filter.ApplyTo(this).ToRGB();
        }
    }
}