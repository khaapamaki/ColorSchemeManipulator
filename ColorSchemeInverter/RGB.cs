using System;

namespace ColorSchemeInverter
{
    public class RGB : Color
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

        public RGB(RGB8Bit rgb8)
        {
            Red = rgb8.Red / 255.0;
            Green = rgb8.Green / 255.0;
            Blue = rgb8.Blue / 255.0;
            Alpha = rgb8.Alpha / 255.0;
        }

        public RGB(HSL hsl)
        {
            CopyFrom(hsl.ToRGB());
        }

        public RGB(RGB rgb)
        {
            Red = rgb.Red;
            Green = rgb.Green;
            Blue = rgb.Blue;
            Alpha = rgb.Alpha;
        }

        public void CopyFrom(RGB rgb)
        {
            Red = rgb.Red;
            Green = rgb.Green;
            Blue = rgb.Blue;
            Alpha = rgb.Alpha;
        }

        public static RGB FromHSL(HSL hsl)
        {
            return new RGB(hsl);
        }

        public static RGB FromRGBString(string rgbString, string rgbStringFormat)
        {
            return RGB8Bit.FromRGBString(rgbString, rgbStringFormat).ToRGB();
        }

        public static RGB FromRGBString(string rgbString)
        {
            return RGB8Bit.FromRGBString(rgbString).ToRGB();
        }

        public static RGB FromARGBString(string rgbString)
        {
            return RGB8Bit.FromARGBString(rgbString).ToRGB();
        }

        public static RGB FromRGBAString(string rgbString)
        {
            return RGB8Bit.FromRGBAString(rgbString).ToRGB();
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

        public RGB8Bit ToRGB8Bit()
        {
            return new RGB8Bit(
                Red.Clamp(0.0, 1.0),
                Green.Clamp(0.0, 1.0),
                Blue.Clamp(0.0, 1.0),
                Alpha.Clamp(0.0, 1.0));
        }


        public HSL ToHSL()
        {
            HSL hsl = new HSL();
            hsl.Alpha = Alpha;
            double r = Red;
            double g = Green;
            double b = Blue;
            double min = Math.Min(Math.Min(r, g), b);
            double max = Math.Max(Math.Max(r, g), b);
            double delta = max - min;
            hsl.Lightness = (max + min) / 2.0;
            if (delta <= 0.0001) {
                hsl.Hue = 0.0;
                hsl.Saturation = 0.0;
            } else {
                hsl.Saturation = (hsl.Lightness <= 0.5) ? (delta / (max + min)) : (delta / (2.0 - max - min));

                double hue;

                if (r >= max) {
                    hue = ((g - b) / 6.0) / delta;
                } else if (g >= max) {
                    hue = (1.0 / 3.0) + ((b - r) / 6.0) / delta;
                } else {
                    hue = (2.0 / 3.0) + ((r - g) / 6.0) / delta;
                }

                if (hue < 0.0)
                    hue += 1.0;
                if (hue > 1.0)
                    hue -= 1.0;

                hsl.Hue = hue * 360.0;
            }

            return hsl;
        }

        public HSV ToHSV()
        {
            double delta, min;
            double h = 0.0, s, v;
            min = Math.Min(Math.Min(Red, Green), Blue);
            v = Math.Max(Math.Max(Red, Green), Blue);
            delta = v - min;
            if (v <= 0.001)
                s = 0.0;
            else

                s = delta / v;
            if (s <= 0.001)
                h = 0.0;
            else {
                if (Red.AboutEqual(v))
                    h = (Green - Blue) / delta;
                else if (Green.AboutEqual(v))
                    h = 2.0 + (Blue - Red) / delta;
                else if (Blue.AboutEqual(v))
                    h = 4.0 + (Red - Green) / delta;

                h *= 60.0;

                if (h < 0.0)
                    h = h + 360.0;
            }

            return new HSV(h, s, v, Alpha);
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
                return string.Format(
                    $"Red: 0x{Red * 255:X2}, Green: 0x{Green * 255:X2}, Blue: 0x{Blue * 255:X2}  Alpha: 0x{Alpha * 255:X2}");
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