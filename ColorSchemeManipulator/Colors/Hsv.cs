using System;
using ColorSchemeManipulator.Common;

namespace ColorSchemeManipulator.Colors
{
    public class Hsv : ColorBase
    {
        
        private double _hue = 0.0;

        public double Hue
        {
            get => _hue.NormalizeLoopingValue(360.0);
            set => _hue = value.NormalizeLoopingValue(360.0);
        }
        
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

        public void CopyFrom(Hsv hsv)
        {
            Hue = hsv.Hue;
            Saturation = hsv.Saturation;
            Value = hsv.Value;
            Alpha = hsv.Alpha;
        }

        public Hsv(ColorBase color)
        {
            CopyFrom(color.ToHsv());
        }

        public Hsv Interpolate(Hsv hsv, double factor)
        {
            Rgb rgb1 = ToRgb();
            Rgb rgb2 = hsv.ToRgb();
            return rgb1.Interpolate(rgb2, factor).ToHsv();
        }

        public override string ToString()
        {
            return string.Format($"Hue: {Hue}, Saturation: {Saturation}, Value: {Value}, Alpha8: {Alpha}");
        }

        public string ToString(string format)
        {
            if (format.ToUpper() == "X2") {
                return string.Format($"Hue: 0x{Hue * 255:X2}, " +
                                     $"Saturation: 0x{Saturation * 255:X2}, " +
                                     $"Value 0x{Value * 255:X2}");
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