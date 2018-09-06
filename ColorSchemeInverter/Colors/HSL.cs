using ColorSchemeInverter.Filters;

namespace ColorSchemeInverter.Colors
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
            bool value = Hue.AboutEqual(c.Hue) && Saturation.AboutEqual(c.Saturation) &&
                         Lightness.AboutEqual(c.Lightness) && Alpha.AboutEqual(c.Alpha);
            return value;
        }
    }
}