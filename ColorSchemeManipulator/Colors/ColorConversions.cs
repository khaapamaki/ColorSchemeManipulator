using System;
using System.CodeDom;
using System.Drawing;
using System.Runtime.Remoting.Messaging;
using ColorSchemeManipulator.Common;

namespace ColorSchemeManipulator.Colors
{
    public static class ColorConversions
    {
        public static Hsl RgbtoHsl(Rgb rgb)
        {
            var hsl = new Hsl();
            hsl.Alpha = rgb.Alpha;
            double r = rgb.Red;
            double g = rgb.Green;
            double b = rgb.Blue;
            double min = Math.Min(Math.Min(r, g), b);
            double max = Math.Max(Math.Max(r, g), b);
            double delta = max - min;
            hsl.Lightness = (max + min) / 2.0;
            if (delta <= 0.0001) {
                hsl.Hue = 0.0;
                hsl.Saturation = 0.0;
            } else {
                hsl.Saturation = hsl.Lightness <= 0.5 ? delta / (max + min) : delta / (2.0 - max - min);

                double hue;

                if (r >= max) {
                    hue = (g - b) / 6.0 / delta;
                } else if (g >= max) {
                    hue = 1.0 / 3.0 + (b - r) / 6.0 / delta;
                } else {
                    hue = 2.0 / 3.0 + (r - g) / 6.0 / delta;
                }

                if (hue < 0.0)
                    hue += 1.0;
                if (hue > 1.0)
                    hue -= 1.0;

                hsl.Hue = hue * 360.0;
            }

            return hsl;
        }

        public static Rgb HsltoRgb(Hsl hsl)
        {
            double r = 0;
            double g = 0;
            double b = 0;

            if (hsl.Saturation <= 0.001) {
                r = g = b = hsl.Lightness;
            } else {
                double v1, v2;
                double hue = hsl.Hue / 360.0;

                v2 = hsl.Lightness < 0.5
                    ? hsl.Lightness * (1 + hsl.Saturation)
                    : hsl.Lightness + hsl.Saturation - hsl.Lightness * hsl.Saturation;
                v1 = 2 * hsl.Lightness - v2;

                r = HueToRgb(v1, v2, hue + 1.0 / 3);
                g = HueToRgb(v1, v2, hue);
                b = HueToRgb(v1, v2, hue - 1.0 / 3);
            }

            return new Rgb(r, g, b, hsl.Alpha);
        }

        private static double HueToRgb(double v1, double v2, double vH)
        {
            if (vH < 0)
                vH += 1;

            if (vH > 1)
                vH -= 1;

            if (6 * vH < 1)
                return v1 + (v2 - v1) * 6 * vH;

            if (2 * vH < 1)
                return v2;

            if (3 * vH < 2)
                return v1 + (v2 - v1) * (2.0f / 3 - vH) * 6;

            return v1;
        }

        public static Hsv RgbtoHsv(Rgb rgb)
        {
            double delta, min;
            double h = 0.0, s, v;
            min = Math.Min(Math.Min(rgb.Red, rgb.Green), rgb.Blue);
            v = Math.Max(Math.Max(rgb.Red, rgb.Green), rgb.Blue);
            delta = v - min;
            if (v <= 0.001)
                s = 0.0;
            else

                s = delta / v;
            if (s <= 0.001)
                h = 0.0;
            else {
                if (rgb.Red.AboutEqual(v))
                    h = (rgb.Green - rgb.Blue) / delta;
                else if (rgb.Green.AboutEqual(v))
                    h = 2.0 + (rgb.Blue - rgb.Red) / delta;
                else if (rgb.Blue.AboutEqual(v))
                    h = 4.0 + (rgb.Red - rgb.Green) / delta;

                h *= 60.0;

                if (h < 0.0)
                    h = h + 360.0;
            }

            return new Hsv(h, s, v, rgb.Alpha);
        }

        public static Rgb HsvtoRgb(Hsv hsv)
        {
            double r = 0, g = 0, b = 0;

            if (hsv.Saturation <= 0.001) {
                r = hsv.Value;
                g = hsv.Value;
                b = hsv.Value;
            } else {
                int i;
                double f, p, q, t;

                if (hsv.Hue >= 360)
                    hsv.Hue = hsv.Hue - 360;
                else
                    hsv.Hue = hsv.Hue / 60;

                i = (int) Math.Truncate(hsv.Hue);
                f = hsv.Hue - i;

                p = hsv.Value * (1.0 - hsv.Saturation);
                q = hsv.Value * (1.0 - hsv.Saturation * f);
                t = hsv.Value * (1.0 - hsv.Saturation * (1.0 - f));

                switch (i) {
                    case 0:
                        r = hsv.Value;
                        g = t;
                        b = p;
                        break;

                    case 1:
                        r = q;
                        g = hsv.Value;
                        b = p;
                        break;

                    case 2:
                        r = p;
                        g = hsv.Value;
                        b = t;
                        break;

                    case 3:
                        r = p;
                        g = q;
                        b = hsv.Value;
                        break;

                    case 4:
                        r = t;
                        g = p;
                        b = hsv.Value;
                        break;

                    default:
                        r = hsv.Value;
                        g = p;
                        b = q;
                        break;
                }
            }

            return new Rgb(r, g, b, hsv.Alpha);
        }

        // Todo algorithm that directly converts from hsl to hsv
        public static Hsv HsltoHsv(Hsl hsl)
        {
            Rgb rgb = new Rgb(hsl);
            return rgb.ToHsv();
        }

        // Todo algorithm that directly converts from hsv to hsl
        public static Hsl HsvtoHsl(Hsv hsv)
        {
            Rgb rgb = new Rgb(hsv);
            return rgb.ToHsl();
        }

        public static Rgb SystemColorToRgb(Color color)
        {
            var rgb8 = new Rgb8Bit(color.R, color.G, color.B, color.A);
            return new Rgb(rgb8);
        }

        public static Color RgbToSystemColor(Rgb rgb)
        {
            var rgb8 = new Rgb8Bit(rgb);
            return Color.FromArgb(rgb8.Alpha, rgb8.Red, rgb8.Green, rgb8.Blue);
        }
    }
}