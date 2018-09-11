using ColorSchemeInverter.Common;
using ColorSchemeInverter.Filters;

namespace ColorSchemeInverter.Colors
{
    public class Hsl : ColorBase
    {
        private double _hue = 0.0;

        public double Hue
        {
            get => _hue.NormalizeLoopingValue(360.0);
            set => _hue = value.NormalizeLoopingValue(360.0);
        }

        public double Saturation { get; set; }
        public double Lightness { get; set; }
        public double Alpha { get; set; } = 1.0;

        public Hsl() { }

        public Hsl(double hue, double saturation, double lightness, double alpha = 1.0)
        {
            Hue = hue;
            Saturation = saturation;
            Lightness = lightness;
            Alpha = alpha;
        }

        public Hsl(Hsl hsl)
        {
            Hue = hsl.Hue;
            Saturation = hsl.Saturation;
            Lightness = hsl.Lightness;
            Alpha = hsl.Alpha;
        }

        public Hsl(Rgb rgb)
        {
            CopyFrom(rgb.ToHsl());
        }

        public Hsl(Hsv hsv)
        {
            CopyFrom(hsv.ToHsl());
        }
            
        public void CopyFrom(Hsl hsl)
        {
            Hue = hsl.Hue;
            Saturation = hsl.Saturation;
            Lightness = hsl.Lightness;
            Alpha = hsl.Alpha;
        }

        public Hsl Interpolate(Hsl hsl, double factor)
        {
            Rgb rgb1 = ToRgb();
            Rgb rgb2 = hsl.ToRgb();
            return rgb1.Interpolate(rgb2, factor).ToHsl();
        }
        
        public override string ToString()
        {
            return string.Format($"Hue: {Hue}, Saturation: {Saturation}, Lightness: {Lightness}, Alpha: {Alpha}");
        }

        public string ToString(string format)
        {
            if (format.ToUpper() == "X2") {
                return string.Format(
                    $"Hue: 0x{Hue * 255:X2}, Saturation: 0x{Saturation * 255:X2}, Lightness 0x{Lightness * 255:X2}");
            } else {
                return ToString();
            }
        }

        public Hsl ApplyFilterSet(FilterSet filters)
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

        public bool Equals(Hsl c)
        {
            bool value = Hue.AboutEqual(c.Hue) && Saturation.AboutEqual(c.Saturation) &&
                         Lightness.AboutEqual(c.Lightness) && Alpha.AboutEqual(c.Alpha);
            return value;
        }
    }
}