using System;
using ColorSchemeInverter.Common;

namespace ColorSchemeInverter.Colors
{
    public class RGB8bit
    {
        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }
        public byte Alpha { get; set; } = 0xFF;

        public RGB8bit() { }

        public RGB8bit(byte red, byte green, byte blue, byte alpha = 0xFF)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        public RGB8bit(double red, double green, double blue, double alpha = 1.0)
        {
            Red = (byte) (red.Clamp(0.0, 1.0) * 255);
            Green = (byte) (green.Clamp(0.0, 1.0) * 255);
            Blue = (byte) (blue.Clamp(0.0, 1.0) * 255);
            Alpha = (byte) (alpha.Clamp(0.0, 1.0) * 255);
        }

        public RGB8bit(RGB rgb)
        {
            Red = (byte) (rgb.Red.Clamp(0.0, 1.0) * 255);
            Green = (byte) (rgb.Green.Clamp(0.0, 1.0) * 255);
            Blue = (byte) (rgb.Blue.Clamp(0.0, 1.0) * 255);
            Alpha = (byte) (rgb.Alpha.Clamp(0.0, 1.0) * 255);
        }

        public RGB ToRGB()
        {
            return new RGB(Red / 255.0, Green / 255.0, Blue / 255.0, Alpha / 255.0);
        }

        public override string ToString()
        {
            return string.Format($"Red: {Red}, Green: {Green}, Blue: {Blue}, Alpha: {Alpha}");
        }

        public string ToString(string format)
        {
            if (format.ToUpper() == "X2") {
                return string.Format($"Red: 0x{Red:X2}, Green: 0x{Green:X2}, Blue: 0x{Blue:X2}  Alpha: 0x{Alpha:X2}");
            } else {
                throw new FormatException("Invalid Format String: " + format);
                return ToString();
            }
        }

        public static RGB8bit FromRGBString(string rgbString, string rgbStringFormat)
        {
            if (IsValidHexString(rgbString) && rgbString.Length == rgbStringFormat.Length) {
                switch (rgbStringFormat.ToUpper()) {
                    case "RRGGBB":
                        return FromRGBString(rgbString);
                    case "AARRGGBB":
                        return FromARGBString(rgbString);
                        break;
                    case "RRGGBBAA":
                        return FromRGBAString(rgbString);
                    default:
                        throw new Exception("Incorrect RGB string format: " + rgbStringFormat);
                }
            }

            throw new Exception("Invalid color string: " + rgbString);
        }

        public static RGB8bit FromRGBString(string rgbString)
        {
            var newRGB = new RGB8bit
            {
                Red = byte.Parse(rgbString.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                Green = byte.Parse(rgbString.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                Blue = byte.Parse(rgbString.Substring(4, 2), System.Globalization.NumberStyles.HexNumber),
                Alpha = 0xFF
            };
            return newRGB;
        }

        public static RGB8bit FromARGBString(string rbgString)
        {
            var newRGB = new RGB8bit
            {
                Red = byte.Parse(rbgString.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                Green = byte.Parse(rbgString.Substring(4, 2), System.Globalization.NumberStyles.HexNumber),
                Blue = byte.Parse(rbgString.Substring(6, 2), System.Globalization.NumberStyles.HexNumber),
                Alpha = byte.Parse(rbgString.Substring(0, 2), System.Globalization.NumberStyles.HexNumber)
            };
            return newRGB;
        }

        public static RGB8bit FromRGBAString(string rgbString)
        {
            var newRGB = new RGB8bit
            {
                Red = byte.Parse(rgbString.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                Green = byte.Parse(rgbString.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                Blue = byte.Parse(rgbString.Substring(4, 2), System.Globalization.NumberStyles.HexNumber),
                Alpha = byte.Parse(rgbString.Substring(6, 2), System.Globalization.NumberStyles.HexNumber)
            };
            return newRGB;
        }


        public string ToRGBString(string rgbStringFormat)
        {
            string result;
            switch (rgbStringFormat.ToUpper()) {
                case "RRGGBB":
                    result = ToRGBString();
                    break;
                case "AARRGGBB":
                    result = ToARGBString();
                    break;
                case "RRGGBBAA":
                    result = ToRGBAString();
                    break;
                default:
                    result = ToRGBString();
                    break;
            }

            bool isUpperCase = rgbStringFormat.ToUpper() == rgbStringFormat;
            return isUpperCase
                ? result.ToUpper()
                : result.ToLower();             
        }
        
        public string ToRGBString()
        {
            return Red.ToString("X2") + Green.ToString("X2") + Blue.ToString("X2");
        }

        public string ToARGBString()
        {
            return Alpha.ToString("X2")
                   + Red.ToString("X2")
                   + Green.ToString("X2")
                   + Blue.ToString("X2");
        }

        public string ToRGBAString()
        {
            return Red.ToString("X2")
                   + Green.ToString("X2")
                   + Blue.ToString("X2")
                   + Alpha.ToString("X2");
        }

        public bool AboutEqual(RGB8bit c)
        {
            int dr = Math.Abs((int)Red - c.Red);
            int dg = Math.Abs((int)Green - c.Green);
            int db = Math.Abs((int)Blue - c.Blue);
            int da = Math.Abs((int)Alpha - c.Alpha);
            return (dr <= 1 && dg <= 1 && db <= 1 && da <= 1);
        }
        
        private static bool IsValidHexString(string str)
        {
            const string validHex = "0123456789abcdefABCDEF";
            foreach (var c in str) {
                if (!validHex.Contains(c.ToString()))
                    return false;
            }

            return true;
        }
    }
}