using System;
using System.Diagnostics.CodeAnalysis;

namespace ColorSchemeManipulator.Colors
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class HexRgbUtil
    {
        public static Color HexStringToColor(string rgbString, string hexFormat)
            {
                if (IsValidHexString(rgbString) && rgbString.Length <= hexFormat.Length) {
                    if (rgbString.Length < hexFormat.Length) {
                        throw new Exception("Invalid color string: " + rgbString);
                        // rgbString = rgbString.PadLeft(hexFormat.Length, '0');
                    }

                    switch (hexFormat.ToUpper()) {
                        case "RRGGBB":
                            return RGBStringToColor(rgbString);
                        case "RGB":
                            return ShortRGBStringToColor(rgbString);
                        case "AARRGGBB":
                            return ARGBStringToColor(rgbString);
                        case "RRGGBBAA":
                            return RGBAStringToColor(rgbString);
                        default:
                            throw new Exception("Incorrect RGB string format: " + hexFormat);
                    }
                }

                throw new Exception("Invalid color string: " + rgbString);
            }

            public static Color RGBStringToColor(string rgbString)
            {
                return Color.FromRgb(
                    byte.Parse(rgbString.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                    byte.Parse(rgbString.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                    byte.Parse(rgbString.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));
            }
        
            public static Color ShortRGBStringToColor(string rgbString)
            {
                return Color.FromRgb(
                    byte.Parse(rgbString.Substring(0, 1) + "0", System.Globalization.NumberStyles.HexNumber),
                    byte.Parse(rgbString.Substring(1, 2) + "0", System.Globalization.NumberStyles.HexNumber),
                    byte.Parse(rgbString.Substring(2, 3) + "0", System.Globalization.NumberStyles.HexNumber));
            }

            public static Color ARGBStringToColor(string rgbString)
            {
                return Color.FromRgb(
                    byte.Parse(rgbString.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                    byte.Parse(rgbString.Substring(4, 2), System.Globalization.NumberStyles.HexNumber),
                    byte.Parse(rgbString.Substring(6, 2), System.Globalization.NumberStyles.HexNumber),
                    byte.Parse(rgbString.Substring(0, 2), System.Globalization.NumberStyles.HexNumber));
            }

            public static Color RGBAStringToColor(string rgbString)
            {
                return Color.FromRgb(
                    byte.Parse(rgbString.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                    byte.Parse(rgbString.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                    byte.Parse(rgbString.Substring(4, 2), System.Globalization.NumberStyles.HexNumber),
                    byte.Parse(rgbString.Substring(6, 2), System.Globalization.NumberStyles.HexNumber));
            }

            public static string ColorToHexString(Color color, string hexFormat)
            {
                return ComponentsToHexString(color.Red, color.Green, color.Blue, color.Alpha, hexFormat);
            }

            public static string ComponentsToHexString(double r, double g, double b, double a, string hexFormat)
            {
                return ComponentsToHexString((byte) (r * 255), (byte) (g * 255), (byte) (b * 255), (byte) (a * 255),
                    hexFormat);
            }

            public static string ComponentsToHexString(byte r, byte g, byte b, byte a, string hexFormat)
            {
                string result;
                switch (hexFormat.ToUpper()) {
                    case "RRGGBB":
                        result = ComponentsToRGBString(r, g, b);
                        break;
                    case "AARRGGBB":
                        result = ComponentsToARGBString(r, g, b, a);
                        break;
                    case "RRGGBBAA":
                        result = ComponentsToRGBAString(r, g, b, a);
                        break;
                    default:
                        result = ComponentsToRGBString(r, g, b, a);
                        break;
                }

                bool isUpperCase = hexFormat.ToUpper() == hexFormat;
                return isUpperCase
                    ? result.ToUpper()
                    : result.ToLower();
            }

            private static string ComponentsToRGBString(byte r, byte g, byte b, byte a = 0xff)
            {
                return r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
            }

            private static string ComponentsToARGBString(byte r, byte g, byte b, byte a)
            {
                return a.ToString("X2")
                       + r.ToString("X2")
                       + g.ToString("X2")
                       + b.ToString("X2");
            }

            private static string ComponentsToRGBAString(byte r, byte g, byte b, byte a)
            {
                return r.ToString("X2")
                       + g.ToString("X2")
                       + b.ToString("X2")
                       + a.ToString("X2");
            }

            public static bool IsValidHexString(string str)
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