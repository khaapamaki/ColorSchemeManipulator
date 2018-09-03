using System;
using System.Xml.Schema;

namespace ColorSchemeInverter
{
    public class HSL
    {
        private byte Hue { get; set; } = 0x00;
        private byte Saturation { get; set; } = 0x00;
        private byte Lightness { get; set; } = 0x00;
        private byte Alpha { get; set; } = 0xFF;

        public HSL(byte hue, byte saturation, byte lightness, byte alpha = 0xFF)
        {
            Hue = hue;
            Saturation = saturation;
            Lightness = lightness;
            Alpha = alpha;
        }

        public void SetHSL(HSL hsl)
        {
            Hue = hsl.Hue;
            Saturation = hsl.Saturation;
            Lightness = hsl.Lightness;
            Alpha = hsl.Alpha;
        }

        public HSL(RGB rgb)
        {
            SetHSL(rgb.ToHSL());
        }

        public static HSL FromRGB(RGB rgb)
        {
            return new HSL(rgb);
        }

        public RGB ToRGB()
        {
            throw new NotImplementedException();
        }

        public HSL InvertLightness()
        {
            Lightness = (byte)(0xFF - Lightness);
            // todo add some color adjustments if needed, by gamma maybe?
            return this;
        }
        
        public override string ToString()
        {
            return string.Format($"Hue: {Hue}, Saturation: {Saturation}, Lightness: {Lightness} ");
        }
        
        public string ToString(string format)
        {
            if (format.ToUpper() == "X2") {
                return string.Format($"Hue: 0x{Hue:X2}, GreSaturationen: 0x{Saturation:X2}, Lightness 0x{Lightness:X2} ");
            } else {
                return ToString();
            }
        }
    }
}