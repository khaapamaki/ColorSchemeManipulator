using System;

namespace ColorSchemeInverter
{
    public class RGB
    {
        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }
        public byte Alpha { get; set; } = 0xFF;

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
            CopyFrom(hsl.ToRGB());
        }

        public RGB(RGB rgb)
        {
            Red = rgb.Red;
            Green = rgb.Green;
            Blue = rgb.Blue;
            Alpha = rgb.Alpha;
        }
        
        public void CopyFrom(RGB rgb)
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

        public static RGB FromRGBString(string rgbString, string rgbStringFormat)
        {
            if (IsValidHexString(rgbString) && rgbString.Length == rgbStringFormat.Length) {
                switch (rgbStringFormat.ToUpper()) {
                    case "RRGGBB":
                        return RGB.FromRGBString(rgbString);
                    case "AARRGGBB":
                        return RGB.FromARGBString(rgbString);
                        break;
                    case "RRGGBBAA":
                        return RGB.FromRGBAString(rgbString);
                    default:
                        throw new Exception("Incorrect RGB string format: " + rgbStringFormat);
                }
            }

            throw new Exception("Invalid color string: " + rgbString);
        }
        
        public static RGB FromRGBString(string color)
        {
            var newRGB = new RGB();
            newRGB.Red = byte.Parse(color.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
            newRGB.Green = byte.Parse(color.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
            newRGB.Blue = byte.Parse(color.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
            newRGB.Alpha = 0xFF;
            return newRGB;
        }

        public static RGB FromARGBString(string color)
        {
            var newRGB = new RGB();
            newRGB.Red = byte.Parse(color.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
            newRGB.Green = byte.Parse(color.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
            newRGB.Blue = byte.Parse(color.Substring(6,2), System.Globalization.NumberStyles.HexNumber);
            newRGB.Alpha = byte.Parse(color.Substring(0,2), System.Globalization.NumberStyles.HexNumber);;
            return newRGB;
        }

        public static RGB FromRGBAString(string color)
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
            return Alpha.ToString("X2") + Red.ToString("X2") + Green.ToString("X2") + Blue.ToString("X2");
        }

        public string ToRGBAString()
        {
            return Red.ToString("X2") + Green.ToString("X2") + Blue.ToString("X2") + Alpha.ToString("X2");
        }

        public HSL ToHSL()
        {
            HSL hsl = new HSL();
            hsl.Alpha = Alpha;
            
            double r = (Red / 255.0);
            double g = (Green/ 255.0);
            double b = (Blue / 255.0);

            double min = Math.Min(Math.Min(r, g), b);
            double max = Math.Max(Math.Max(r, g), b);
            double delta = max - min;

            hsl.Lightness = (max + min) / 2.0;

            if (delta <= 0.0001)
            {
                hsl.Hue = 0.0;
                hsl.Saturation = 0.0;
            }
            else
            {
                hsl.Saturation = (hsl.Lightness <= 0.5) ? (delta / (max + min)) : (delta / (2.0 - max - min));

                double hue;

                if (r >= max)
                {
                    hue = ((g - b) / 6.0) / delta;
                }
                else if (g >= max)
                {
                    hue = (1.0 / 3.0) + ((b - r) / 6.0) / delta;
                }
                else
                {
                    hue = (2.0 / 3.0) + ((r - g) / 6.0) / delta;
                }

                if (hue < 0.0)
                    hue += 1.0;
                if (hue > 1.0)
                    hue -= 1.0;

                hsl.Hue = hue * 360.0;
            }

            return hsl;
        }
        
        public HSV ToHSV()
        {
            double delta, min;
            double h = 0.0, s, v;

            min = Math.Min(Math.Min(Red, Green), Blue);
            v = Math.Max(Math.Max(Red, Green), Blue);
            delta = v - min;

            if (v <= 0.001)
                s = 0.0;
            else
                s = delta / v;

            if (s <= 0.001)
                h = 0.0;

            else
            {
                if (Red == v)
                    h = (Green - Blue) / delta;
                else if (Green == v)
                    h = 2.0 + (Blue - Red) / delta;
                else if (Blue == v)
                    h = 4.0 + (Red - Green) / delta;

                h *= 60.0;

                if (h < 0.0)
                    h = h + 360.0;
            }

            return new HSV(h, s, (v / 255.0), Alpha);
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
            
            return IsUppercase(rgbStringFormat)
                ? result.ToUpper()
                : result.ToLower();  
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
                return ToString();
            }
        }
        
        private static bool IsUppercase(string str)
        {
            return str.ToUpper() == str;
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