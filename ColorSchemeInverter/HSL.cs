using System;
using System.Xml.Schema;

namespace ColorSchemeInverter
{
    public class HSL
    {
        private byte Hue { get; set; }
        private byte Saturation { get; set; }
        private byte Luminance { get; set; }
        private byte Alpha { get; set; }

        public HSL(byte hue, byte saturation, byte luminance, byte alpha = 0xFF)
        {
            Hue = hue;
            Saturation = saturation;
            Luminance = luminance;
            Alpha = alpha;
        }

        public HSL(RGB rgb)
        {
            throw new NotImplementedException();     
        }

        public static HSL FromRGB(RGB rgb)
        {
            return new HSL(rgb);
        }

        public HSL Invert()
        {
            throw new NotImplementedException();
            return this;
        }
    }
}