using System;

namespace ColorSchemeManipulator.Colors
{
    public static class HexRgb
    {
        public static Color FromRgbString(string rgbString, string rgbHexFormat)
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

        public static Color FromRgbString(string rgbString)
        {
            return Color.FromRgb(
                byte.Parse(rgbString.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                byte.Parse(rgbString.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                byte.Parse(rgbString.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));

        }

        public static Color FromArgbString(string rgbString)
        {
            return Color.FromRgb(
                byte.Parse(rgbString.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                byte.Parse(rgbString.Substring(4, 2), System.Globalization.NumberStyles.HexNumber),
                byte.Parse(rgbString.Substring(6, 2), System.Globalization.NumberStyles.HexNumber),
                byte.Parse(rgbString.Substring(0, 2), System.Globalization.NumberStyles.HexNumber));
            
        }

        public static Color FromRgbaString(string rgbString)
        {
            return Color.FromRgb(
                byte.Parse(rgbString.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                byte.Parse(rgbString.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                byte.Parse(rgbString.Substring(4, 2), System.Globalization.NumberStyles.HexNumber),
                byte.Parse(rgbString.Substring(6, 2), System.Globalization.NumberStyles.HexNumber));
        }

        public static string ToRgbString(Color color, string hexFormat)
        {
            return ToRgbString(color.Red, color.Green, color.Blue, color.Alpha, hexFormat);
        }

        public static string ToRgbString(double r, double g, double b, double a, string rgbHexFormat)
        {
            return ToRgbString((byte) (r * 255), (byte) (g * 255),(byte) (b * 255), (byte) (a * 255), rgbHexFormat);
        }
        
        
        public static string ToRgbString(byte r, byte g, byte b, byte a, string rgbHexFormat)
        {
            string result;
            switch (rgbHexFormat.ToUpper()) {
                case "RRGGBB":
                    result = ToRgbString(r, g, b);
                    break;
                case "AARRGGBB":
                    result = ToArgbString(r, g, b, a);
                    break;
                case "RRGGBBAA":
                    result = ToRgbaString(r, g, b, a);
                    break;
                default:
                    result = ToRgbString(r, g, b, a);
                    break;
            }

            bool isUpperCase = rgbHexFormat.ToUpper() == rgbHexFormat;
            return isUpperCase
                ? result.ToUpper()
                : result.ToLower();
        }

        private static string ToRgbString(byte r, byte g, byte b, byte a = 0xff)
        {
            return r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
        }

        private static string ToArgbString(byte r, byte g, byte b, byte a)
        {
            return a.ToString("X2")
                   + r.ToString("X2")
                   + g.ToString("X2")
                   + b.ToString("X2");
        }

        private static string ToRgbaString(byte r, byte g, byte b, byte a)
        {
            return r.ToString("X2")
                   + g.ToString("X2")
                   + b.ToString("X2")
                   + a.ToString("X2");
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