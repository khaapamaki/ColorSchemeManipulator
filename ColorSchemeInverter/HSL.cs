using System;
using System.Xml.Schema;

namespace ColorSchemeInverter
{
    public class HSL : Color
    {
        public double Hue { get; set; }
        public double Saturation { get; set; }
        public double Lightness { get; set; }
        public double Alpha { get; set; } = 1.0;

        public HSL() { }

        public HSL(double hue, double saturation, double lightness, double alpha = 1.0)
        {
            Hue = hue;
            Saturation = saturation;
            Lightness = lightness;
            Alpha = alpha;
        }

        public HSL(HSL hsl)
        {
            Hue = hsl.Hue;
            Saturation = hsl.Saturation;
            Lightness = hsl.Lightness;
            Alpha = hsl.Alpha;
        }
        
        public HSL(RGB rgb)
        {
            CopyFrom(rgb.ToHSL());
        }

        public void CopyFrom(HSL hsl)
        {
            Hue = hsl.Hue;
            Saturation = hsl.Saturation;
            Lightness = hsl.Lightness;
            Alpha = hsl.Alpha;
        }

        public static HSL FromRGB(RGB rgb)
        {
            return new HSL(rgb);
        }

        public RGB ToRGB()
        {
            double r = 0;
            double g = 0;
            double b = 0;

            if (Saturation <= 0.001) {
                r = g = b = Lightness;
            } else {
                double v1, v2;
                double hue = Hue / 360.0;

                v2 = (Lightness < 0.5)
                    ? (Lightness * (1 + Saturation))
                    : ((Lightness + Saturation) - (Lightness * Saturation));
                v1 = 2 * Lightness - v2;

                r = HueToRGB(v1, v2, hue + (1.0 / 3));
                g = HueToRGB(v1, v2, hue);
                b = HueToRGB(v1, v2, hue - (1.0 / 3));
            }

            return new RGB(r, g, b, Alpha);
        }

        private static double HueToRGB(double v1, double v2, double vH)
        {
            if (vH < 0)
                vH += 1;

            if (vH > 1)
                vH -= 1;

            if ((6 * vH) < 1)
                return (v1 + (v2 - v1) * 6 * vH);

            if ((2 * vH) < 1)
                return v2;

            if ((3 * vH) < 2)
                return (v1 + (v2 - v1) * ((2.0f / 3) - vH) * 6);

            return v1;
        }

        public HSL InvertLightness()
        {
            Lightness = 1 - Lightness;
            return this;
        }

        public override string ToString()
        {
            return string.Format($"Hue: {Hue}, Saturation: {Saturation}, Lightness: {Lightness} ");
        }

        public string ToString(string format)
        {
            if (format.ToUpper() == "X2") {
                return string.Format(
                    $"Hue: 0x{Hue * 255:X2}, Saturation: 0x{Saturation * 255:X2}, Lightness 0x{Lightness * 255:X2} ");
            } else {
                return ToString();
            }
        }

        public HSL ApplyFilterSet(FilterSet filters)
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
        
        public bool Equals(HSL c)
        {
            bool value = Hue.AboutEqual(c.Hue) && Saturation.AboutEqual(c.Saturation) && Lightness.AboutEqual(c.Lightness) && Alpha.AboutEqual(c.Alpha);
            return value;
        }
    }
}