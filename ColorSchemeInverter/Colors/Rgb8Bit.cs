using System;
using ColorSchemeInverter.Common;

namespace ColorSchemeInverter.Colors
{
    public class Rgb8Bit
    {
        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }
        public byte Alpha { get; set; } = 0xFF;

        public Rgb8Bit() { }

        public Rgb8Bit(byte red, byte green, byte blue, byte alpha = 0xFF)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        public Rgb8Bit(double red, double green, double blue, double alpha = 1.0)
        {
            Red = (byte) (red.Clamp(0.0, 1.0) * 255);
            Green = (byte) (green.Clamp(0.0, 1.0) * 255);
            Blue = (byte) (blue.Clamp(0.0, 1.0) * 255);
            Alpha = (byte) (alpha.Clamp(0.0, 1.0) * 255);
        }

        public Rgb8Bit(Rgb rgb)
        {
            Red = (byte) (rgb.Red.Clamp(0.0, 1.0) * 255);
            Green = (byte) (rgb.Green.Clamp(0.0, 1.0) * 255);
            Blue = (byte) (rgb.Blue.Clamp(0.0, 1.0) * 255);
            Alpha = (byte) (rgb.Alpha.Clamp(0.0, 1.0) * 255);
        }

        public Rgb ToRgb()
        {
            return new Rgb(Red / 255.0, Green / 255.0, Blue / 255.0, Alpha / 255.0);
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
                //return ToString();
            }
        }

        public static Rgb8Bit FromRgbString(string rgbString, string rgbHexFormat)
        {
            if (IsValidHexString(rgbString) && rgbString.Length == rgbHexFormat.Length) {
                switch (rgbHexFormat.ToUpper()) {
                    case "RRGGBB":
                        return FromRgbString(rgbString);
                    case "AARRGGBB":
                        return FromArgbString(rgbString);
                    case "RRGGBBAA":
                        return FromRgbaString(rgbString);
                    default:
                        throw new Exception("Incorrect RGB string format: " + rgbHexFormat);
                }
            }

            throw new Exception("Invalid color string: " + rgbString);
        }

        public static Rgb8Bit FromRgbString(string rgbString)
        {
            var newRgb = new Rgb8Bit
            {
                Red = byte.Parse(rgbString.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                Green = byte.Parse(rgbString.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                Blue = byte.Parse(rgbString.Substring(4, 2), System.Globalization.NumberStyles.HexNumber),
                Alpha = 0xFF
            };
            return newRgb;
        }

        public static Rgb8Bit FromArgbString(string rbgString)
        {
            var newRgb = new Rgb8Bit
            {
                Red = byte.Parse(rbgString.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                Green = byte.Parse(rbgString.Substring(4, 2), System.Globalization.NumberStyles.HexNumber),
                Blue = byte.Parse(rbgString.Substring(6, 2), System.Globalization.NumberStyles.HexNumber),
                Alpha = byte.Parse(rbgString.Substring(0, 2), System.Globalization.NumberStyles.HexNumber)
            };
            return newRgb;
        }

        public static Rgb8Bit FromRgbaString(string rgbString)
        {
            var newRgb = new Rgb8Bit
            {
                Red = byte.Parse(rgbString.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                Green = byte.Parse(rgbString.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                Blue = byte.Parse(rgbString.Substring(4, 2), System.Globalization.NumberStyles.HexNumber),
                Alpha = byte.Parse(rgbString.Substring(6, 2), System.Globalization.NumberStyles.HexNumber)
            };
            return newRgb;
        }

        public string ToRgbString(string rgbHexFormat)
        {
            string result;
            switch (rgbHexFormat.ToUpper()) {
                case "RRGGBB":
                    result = ToRgbString();
                    break;
                case "AARRGGBB":
                    result = ToArgbString();
                    break;
                case "RRGGBBAA":
                    result = ToRgbaString();
                    break;
                default:
                    result = ToRgbString();
                    break;
            }

            bool isUpperCase = rgbHexFormat.ToUpper() == rgbHexFormat;
            return isUpperCase
                ? result.ToUpper()
                : result.ToLower();             
        }
        
        public string ToRgbString()
        {
            return Red.ToString("X2") + Green.ToString("X2") + Blue.ToString("X2");
        }

        public string ToArgbString()
        {
            return Alpha.ToString("X2")
                   + Red.ToString("X2")
                   + Green.ToString("X2")
                   + Blue.ToString("X2");
        }

        public string ToRgbaString()
        {
            return Red.ToString("X2")
                   + Green.ToString("X2")
                   + Blue.ToString("X2")
                   + Alpha.ToString("X2");
        }

        public bool AboutEqual(Rgb8Bit c)
        {
            int dr = Math.Abs((int)Red - c.Red);
            int dg = Math.Abs((int)Green - c.Green);
            int db = Math.Abs((int)Blue - c.Blue);
            int da = Math.Abs((int)Alpha - c.Alpha);
            return dr <= 1 && dg <= 1 && db <= 1 && da <= 1;
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