using System;

namespace ColorSchemeInverter
{
    public class RGB
    {
        private byte Red { get; set; }
        private byte Green { get; set; }
        private byte Blue { get; set; }
        private byte Alpha { get; set; }

        public RGB(byte red, byte green, byte blue, byte alpha = 0xFF)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        public RGB(HSL hsl)
        {
            throw new NotImplementedException();
        }

        public static RGB FromHSL(HSL hsl)
        {
            return new RGB(hsl);
        }
        
        public static RGB FromRGB(string color)
        {
            throw new NotImplementedException();
        }

        public static RGB FromARGB(string color)
        {
            throw new NotImplementedException();
        }

        public static RGB FromRGBA(string color)
        {
            throw new NotImplementedException();
        }

        public string ToRGBString()
        {
            throw new NotImplementedException();
        }

        public string ToARGBString()
        {
            throw new NotImplementedException();
        }

        public string ToRGBAString()
        {
            throw new NotImplementedException();
        }
    }
}