using System;

namespace ColorSchemeInverter.Colors
{
    [Obsolete]
    public class HSV
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

        public static HSV FromRGB(RGB rgb)
        {
            return new HSV(rgb);
        }

        public RGB ToRGB()
        {
            double r = 0, g = 0, b = 0;

            if (Saturation <= 0.001)
            {
                r = Value;
                g = Value;
                b = Value;
            }
            else
            {
                int i;
                double f, p, q, t;

                if (Hue >= 360)
                    Hue = Hue - 360;
                else
                    Hue = Hue / 60;

                i = (int)Math.Truncate(Hue);
                f = Hue - i;

                p = Value * (1.0 - Saturation);
                q = Value * (1.0 - (Saturation * f));
                t = Value * (1.0 - (Saturation * (1.0 - f)));

                switch (i)
                {
                    case 0:
                        r = Value;
                        g = t;
                        b = p;
                        break;

                    case 1:
                        r = q;
                        g = Value;
                        b = p;
                        break;

                    case 2:
                        r = p;
                        g = Value;
                        b = t;
                        break;

                    case 3:
                        r = p;
                        g = q;
                        b = Value;
                        break;

                    case 4:
                        r = t;
                        g = p;
                        b = Value;
                        break;

                    default:
                        r = Value;
                        g = p;
                        b = q;
                        break;
                }

            }

            return new RGB((byte)(r * 255), (byte)(g * 255), (byte)(b * 255), Alpha);
        }

        public HSV InvertValue()
        {
            Value = 1.0 - Value;
            return this;
        }
        
        public override string ToString()
        {
            return string.Format($"Hue: {Hue}, Saturation: {Saturation}, Value: {Value} ");
        }
        
        public string ToString(string format)
        {
            if (format.ToUpper() == "X2") {
                return string.Format($"Hue: 0x{Hue*255:X2}, Saturation: 0x{Saturation*255:X2}, Value 0x{Value*255:X2} ");
            } else {
                return ToString();
            }
        }
        
        public bool Equals(HSV c)
        {
            bool value = Hue.AboutEqual(c.Hue) && Saturation.AboutEqual(c.Saturation) && Value.AboutEqual(c.Value) && Alpha.AboutEqual(c.Alpha);
            return value;
        }
    }
}