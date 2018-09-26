using System;
using ColorSchemeManipulator.Common;

namespace ColorSchemeManipulator.Colors
{
    public static class ColorConversions
    {
        public static (double, double, double) RgbToHsl(double r, double g, double b)
        {
            double h, s, l;
            double min = Math.Min(Math.Min(r, g), b);
            double max = Math.Max(Math.Max(r, g), b);
            double delta = max - min;
            l = (max + min) / 2.0;
            if (delta <= 0.01) {
                h = 0.0;
                // reduce saturation for low lightness colors
                s = ColorMath.LinearInterpolation(delta, 0.005, 0.01, 0, delta / (max + min));
            } else {
                s = l <= 0.5 ? delta / (max + min) : delta / (2.0 - max - min);

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

                h = hue * 360.0;
            }

            return (h, s, l);
        }

        public static (double, double, double) HslToRgb(double h, double s, double l)
        {
            double r, g, b;

            if (s <= 0.01) {
                r = g = b = l;
            } else {
                double v1, v2;
                double hue = h / 360.0;

                v2 = l < 0.5
                    ? l * (1 + s)
                    : l + s - l * s;
                v1 = 2 * l - v2;

                r = HueToRgb(v1, v2, hue + 1.0 / 3);
                g = HueToRgb(v1, v2, hue);
                b = HueToRgb(v1, v2, hue - 1.0 / 3);
            }

            return (r, g, b);
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

        public static (double, double, double) RgbToHsv(double r, double g, double b)
        {
            double delta, min;
            double h = 0.0, s, v;
            min = Math.Min(Math.Min(r, g), b);
            v = Math.Max(Math.Max(r, g), b);
            delta = v - min;
            if (v <= 0.12)
                // reduce saturation for low value colors
                s = ColorMath.LinearInterpolation(v, 0.005, 0.12, 0, delta / v);
            else
                s = delta / v;
            
            if (s <= 0.001)
                h = 0.0;
            else {
                if (r.AboutEqual(v))
                    h = (g - b) / delta;
                else if (g.AboutEqual(v))
                    h = 2.0 + (b - r) / delta;
                else if (b.AboutEqual(v))
                    h = 4.0 + (r - g) / delta;

                h *= 60.0;

                if (h < 0.0)
                    h = h + 360.0;
            }

            return (h, s, v);
        }

        public static (double, double, double) HsvToRgb(double h, double s, double v)
        {
            double r = 0, g = 0, b = 0;

            if (s <= 0.01) {
                r = v;
                g = v;
                b = v;
            } else {
                int i;
                double f, p, q, t;

                if (h >= 360)
                    h = h - 360;
                else
                    h = h / 60;

                i = (int) Math.Truncate(h);
                f = h - i;

                p = v * (1.0 - s);
                q = v * (1.0 - s * f);
                t = v * (1.0 - s * (1.0 - f));

                switch (i) {
                    case 0:
                        r = v;
                        g = t;
                        b = p;
                        break;

                    case 1:
                        r = q;
                        g = v;
                        b = p;
                        break;

                    case 2:
                        r = p;
                        g = v;
                        b = t;
                        break;

                    case 3:
                        r = p;
                        g = q;
                        b = v;
                        break;

                    case 4:
                        r = t;
                        g = p;
                        b = v;
                        break;

                    default:
                        r = v;
                        g = p;
                        b = q;
                        break;
                }
            }

            return (r, g, b);
        }

        public static Color SystemColorToColor(System.Drawing.Color sysColor)
        {
            return Color.FromRgb(sysColor.R, sysColor.G, sysColor.B, sysColor.A);
        }

        public static System.Drawing.Color ColorToSystemColor(Color color)
        {
            return System.Drawing.Color.FromArgb(
                (byte) (color.Alpha * 255),
                (byte) (color.Red * 255),
                (byte) (color.Green * 255),
                (byte) (color.Blue * 255));
        }

        // Todo algorithm that directly converts from hsv to hsl
        public static (double, double, double) HsvToHsl(double h, double s, double v)
        {
            (double r, double g, double b) = HsvToRgb(h, s, v);
            return RgbToHsl(r, g, b);
        }

        // Todo algorithm that directly converts from hsl to hsv
        public static (double, double, double) HslToHsv(double h, double s, double l)
        {
            (double r, double g, double b) = HslToRgb(h, s, l);
            return RgbToHsv(r, g, b);
        }

    }
}