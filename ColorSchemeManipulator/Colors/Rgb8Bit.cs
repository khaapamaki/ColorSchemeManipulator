using System;
using ColorSchemeManipulator.Common;

namespace ColorSchemeManipulator.Colors
{
    [Obsolete]
    public class Rgb8Bit
    {
        public byte Red8 { get; set; }
        public byte Green8 { get; set; }
        public byte Blue8 { get; set; }
        public byte Alpha8 { get; set; } = 0xFF;

        public Rgb8Bit() { }

        public Rgb8Bit(byte red8, byte green8, byte blue8, byte alpha8 = 0xFF)
        {
            Red8 = red8;
            Green8 = green8;
            Blue8 = blue8;
            Alpha8 = alpha8;
        }

        public Rgb8Bit(double red, double green, double blue, double alpha = 1.0)
        {
            Red8 = (byte) (red.Clamp(0.0, 1.0) * 255);
            Green8 = (byte) (green.Clamp(0.0, 1.0) * 255);
            Blue8 = (byte) (blue.Clamp(0.0, 1.0) * 255);
            Alpha8 = (byte) (alpha.Clamp(0.0, 1.0) * 255);
        }

        public Rgb8Bit(Color color)
        {
            Red8 = (byte) (color.Red.Clamp(0.0, 1.0) * 255);
            Green8 = (byte) (color.Green.Clamp(0.0, 1.0) * 255);
            Blue8 = (byte) (color.Blue.Clamp(0.0, 1.0) * 255);
            Alpha8 = (byte) (color.Alpha.Clamp(0.0, 1.0) * 255);
        }
        
        /*[Obsolete]
        public Rgb8Bit(Rgb rgb)
        {
            Red8 = (byte) (rgb.Red.Clamp(0.0, 1.0) * 255);
            Green8 = (byte) (rgb.Green.Clamp(0.0, 1.0) * 255);
            Blue8 = (byte) (rgb.Blue.Clamp(0.0, 1.0) * 255);
            Alpha8 = (byte) (rgb.Alpha.Clamp(0.0, 1.0) * 255);
        }*/

        public Color ToColor()
        {
            return Color.FromRgb8(Red8, Green8, Blue8, Alpha8);
        }

        /*[Obsolete]
        public Rgb ToRgb()
        {
            return new Rgb(Red8 / 255.0, Green8 / 255.0, Blue8 / 255.0, Alpha8 / 255.0);
        }*/

        public override string ToString()
        {
            return string.Format($"Red8: {Red8}, Green8: {Green8}, Blue8: {Blue8}, Alpha8: {Alpha8}");
        }

        public string ToString(string format)
        {
            if (format.ToUpper() == "X2") {
                return string.Format($"Red8: 0x{Red8:X2}, Green8: 0x{Green8:X2}, Blue8: 0x{Blue8:X2}  Alpha8: 0x{Alpha8:X2}");
            } else {
                throw new FormatException("Invalid Format String: " + format);
                //return ToString();
            }
        }

        public static Rgb8Bit FromRgbString(string rgbString, string rgbHexFormat)
        {
            if (IsValidHexString(rgbString) && rgbString.Length <= rgbHexFormat.Length) {
                if (rgbString.Length < rgbHexFormat.Length) {
                    rgbString = rgbString.PadLeft(rgbHexFormat.Length, '0');
                }

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
                Red8 = byte.Parse(rgbString.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                Green8 = byte.Parse(rgbString.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                Blue8 = byte.Parse(rgbString.Substring(4, 2), System.Globalization.NumberStyles.HexNumber),
                Alpha8 = 0xFF
            };
            return newRgb;
        }

        public static Rgb8Bit FromArgbString(string rbgString)
        {
            var newRgb = new Rgb8Bit
            {
                Red8 = byte.Parse(rbgString.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                Green8 = byte.Parse(rbgString.Substring(4, 2), System.Globalization.NumberStyles.HexNumber),
                Blue8 = byte.Parse(rbgString.Substring(6, 2), System.Globalization.NumberStyles.HexNumber),
                Alpha8 = byte.Parse(rbgString.Substring(0, 2), System.Globalization.NumberStyles.HexNumber)
            };
            return newRgb;
        }

        public static Rgb8Bit FromRgbaString(string rgbString)
        {
            var newRgb = new Rgb8Bit
            {
                Red8 = byte.Parse(rgbString.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                Green8 = byte.Parse(rgbString.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                Blue8 = byte.Parse(rgbString.Substring(4, 2), System.Globalization.NumberStyles.HexNumber),
                Alpha8 = byte.Parse(rgbString.Substring(6, 2), System.Globalization.NumberStyles.HexNumber)
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
            return Red8.ToString("X2") + Green8.ToString("X2") + Blue8.ToString("X2");
        }

        public string ToArgbString()
        {
            return Alpha8.ToString("X2")
                   + Red8.ToString("X2")
                   + Green8.ToString("X2")
                   + Blue8.ToString("X2");
        }

        public string ToRgbaString()
        {
            return Red8.ToString("X2")
                   + Green8.ToString("X2")
                   + Blue8.ToString("X2")
                   + Alpha8.ToString("X2");
        }

        public bool AboutEqual(Rgb8Bit c)
        {
            int dr = Math.Abs((int) Red8 - c.Red8);
            int dg = Math.Abs((int) Green8 - c.Green8);
            int db = Math.Abs((int) Blue8 - c.Blue8);
            int da = Math.Abs((int) Alpha8 - c.Alpha8);
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