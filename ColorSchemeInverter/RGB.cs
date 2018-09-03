using System;

namespace ColorSchemeInverter
{
    public class RGB
    {
        private byte Red { get; set; } = 0x00;
        private byte Green { get; set; } = 0x00;
        private byte Blue { get; set; } = 0x00;
        private byte Alpha { get; set; } = 0xFF;

        private RGB() { }
        
        public RGB(byte red, byte green, byte blue, byte alpha = 0xFF)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        public RGB(HSL hsl)
        {
            SetRGB(hsl.ToRGB());
        }

        public void SetRGB(RGB rgb)
        {
            Red = rgb.Red;
            Green = rgb.Green;
            Blue = rgb.Blue;
            Alpha = rgb.Alpha;
        }
        
        public static RGB FromHSL(HSL hsl)
        {
            return new RGB(hsl);
        }
        
        public static RGB FromRGB(string color)
        {
            var newRGB = new RGB();
            newRGB.Red = byte.Parse(color.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
            newRGB.Green = byte.Parse(color.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
            newRGB.Blue = byte.Parse(color.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
            newRGB.Alpha = 0xFF;
            return newRGB;
        }

        public static RGB FromARGB(string color)
        {
            var newRGB = new RGB();
            newRGB.Red = byte.Parse(color.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
            newRGB.Green = byte.Parse(color.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
            newRGB.Blue = byte.Parse(color.Substring(6,2), System.Globalization.NumberStyles.HexNumber);
            newRGB.Alpha = byte.Parse(color.Substring(0,2), System.Globalization.NumberStyles.HexNumber);;
            return newRGB;
        }

        public static RGB FromRGBA(string color)
        {
            var newRGB = new RGB();
            newRGB.Red = byte.Parse(color.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
            newRGB.Green = byte.Parse(color.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
            newRGB.Blue = byte.Parse(color.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
            newRGB.Alpha = byte.Parse(color.Substring(6,2), System.Globalization.NumberStyles.HexNumber);;
            return newRGB;
        }

        public string ToRGBString()
        {
            return Red.ToString("X2") + Green.ToString("X2") + Blue.ToString("X2");
        }

        public string ToARGBString()
        {
            return Red.ToString("X2") + Green.ToString("X2") + Blue.ToString("X2");
        }

        public string ToRGBAString()
        {
            return Red.ToString("X2") + Green.ToString("X2") + Blue.ToString("X2") + Alpha.ToString("X2");
        }

        public HSL ToHSL()
        {
            throw new NotImplementedException();
        }
        
        public HSV ToHSV()
        {
            throw new NotImplementedException();
        }
        
        public RGB InvertInHSL() { 
            SetRGB(ToHSL().InvertLightness().ToRGB());
            return this;
        }

        public RGB InvertInHSV() { 
        public RGB InvertInHSV() { 
            SetRGB(ToHSV().InvertValue().ToRGB());
            return this;
        }
        
        public override string ToString()
        {
            return string.Format($"Red: {Red}, Green: {Green}, Blue: {Blue} ");
        }
        
        public string ToString(string format)
        {
            if (format.ToUpper() == "X2") {
                return string.Format($"Red: 0x{Red:X2}, Green: 0x{Green:X2}, Blue: 0x{Blue:X2} ");
            } else {
                return ToString();
            }
        }
    }
}