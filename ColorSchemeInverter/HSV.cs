using System;
using System.Xml.Schema;

namespace ColorSchemeInverter
{
    public class HSV
    {
        private byte Hue { get; set; } = 0x00;
        private byte Saturation { get; set; } = 0x00;
        private byte Value { get; set; } = 0x00;
        private byte Alpha { get; set; } = 0xFF;

        public HSV(byte hue, byte saturation, byte value, byte alpha = 0xFF)
        {
            Hue = hue;
            Saturation = saturation;
            Value = value;
            Alpha = alpha;
        }

        public void SetHSV(HSV hsv)
        {
            Hue = hsv.Hue;
            Saturation = hsv.Saturation;
            Value = hsv.Value;
            Alpha = hsv.Alpha;
        }

        public HSV(RGB rgb)
        {
            SetHSV(rgb.ToHSV());
        }

        public static HSV FromRGB(RGB rgb)
        {
            return new HSV(rgb);
        }

        public RGB ToRGB()
        {
            throw new NotImplementedException();
        }

        public HSV InvertValue()
        {
            Value = (byte)(0xFF - Value);
            // todo add some color adjustments if needed, by gamma maybe?
            return this;
        }
        
        public override string ToString()
        {
            return string.Format($"Hue: {Hue}, Saturation: {Saturation}, Value: {Value} ");
        }
        
        public string ToString(string format)
        {
            if (format.ToUpper() == "X2") {
                return string.Format($"Hue: 0x{Hue:X2}, GreSaturationen: 0x{Saturation:X2}, Value 0x{Value:X2} ");
            } else {
                return ToString();
            }
        }
    }
}