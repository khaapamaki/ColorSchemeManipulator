using System;
using ColorSchemeInverter.Common;

namespace ColorSchemeInverter.Colors
{

    public class HSV : ColorBase
    {
        public double Hue { get; set; }
        public double Saturation { get; set; }
        public double Value { get; set; }
        public double Alpha { get; set; } = 1.0;

        public HSV() { }

        public HSV(double hue, double saturation, double value, double alpha = 1.0)
        {
            Hue = hue;
            Saturation = saturation;
            Value = value;
            Alpha = alpha;
        }

        public HSV(HSV hsv)
        {
            Hue = hsv.Hue;
            Saturation = hsv.Saturation;
            Value = hsv.Value;
            Alpha = hsv.Alpha;
        }

        public void CopyFrom(HSV hsv)
        {
            Hue = hsv.Hue;
            Saturation = hsv.Saturation;
            Value = hsv.Value;
            Alpha = hsv.Alpha;
        }

        public HSV(RGB rgb)
        {
            CopyFrom(rgb.ToHSV());
        }

        public HSV(HSL hsl)
        {
            CopyFrom(hsl.ToHSV());
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

        public bool Equals(HSV c)
        {
            bool value = Hue.AboutEqual(c.Hue)
                         && Saturation.AboutEqual(c.Saturation)
                         && Value.AboutEqual(c.Value)
                         && Alpha.AboutEqual(c.Alpha);
            return value;
        }
    }
}