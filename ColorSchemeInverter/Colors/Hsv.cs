using System;
using ColorSchemeInverter.Common;
using ColorSchemeInverter.Filters;

namespace ColorSchemeInverter.Colors
{
    public class Hsv : ColorBase
    {
        public double Hue { get; set; }
        public double Saturation { get; set; }
        public double Value { get; set; }
        public double Alpha { get; set; } = 1.0;

        public Hsv() { }

        public Hsv(double hue, double saturation, double value, double alpha = 1.0)
        {
            Hue = hue;
            Saturation = saturation;
            Value = value;
            Alpha = alpha;
        }

        public Hsv(Hsv hsv)
        {
            Hue = hsv.Hue;
            Saturation = hsv.Saturation;
            Value = hsv.Value;
            Alpha = hsv.Alpha;
        }

        public void CopyFrom(Hsv hsv)
        {
            Hue = hsv.Hue;
            Saturation = hsv.Saturation;
            Value = hsv.Value;
            Alpha = hsv.Alpha;
        }

        public Hsv(Rgb rgb)
        {
            CopyFrom(rgb.ToHsv());
        }

        public Hsv(Hsl hsl)
        {
            CopyFrom(hsl.ToHsv());
        }

        public Hsv Interpolate(Hsv hsv, double factor)
        {
            factor = factor.Clamp(0, 1);
            Hsv result = new Hsv();
            result.Hue = ColorMath.Linear01(factor, Hue, hsv.Hue);
            result.Saturation = ColorMath.Linear01(factor, Saturation, hsv.Saturation);
            result.Value = ColorMath.Linear01(factor, Value, hsv.Value);
            result.Alpha = ColorMath.Linear01(factor, Alpha, hsv.Alpha);
            return result;
        }

        public override string ToString()
        {
            return string.Format($"Hue: {Hue}, Saturation: {Saturation}, Value: {Value} ");
        }

        public string ToString(string format)
        {
            if (format.ToUpper() == "X2") {
                return string.Format($"Hue: 0x{Hue * 255:X2}, " +
                                     $"Saturation: 0x{Saturation * 255:X2}, " +
                                     $"Value 0x{Value * 255:X2} ");
            } else {
                return ToString();
            }
        }

        public bool Equals(Hsv c)
        {
            bool value = Hue.AboutEqual(c.Hue)
                         && Saturation.AboutEqual(c.Saturation)
                         && Value.AboutEqual(c.Value)
                         && Alpha.AboutEqual(c.Alpha);
            return value;
        }
    }
}