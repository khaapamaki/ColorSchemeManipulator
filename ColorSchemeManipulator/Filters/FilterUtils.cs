using System.Collections.Generic;
using ColorSchemeManipulator.Colors;

namespace ColorSchemeManipulator.Filters
{
    public static class FilterUtils
    {
        public static double GetRangeFactor(ColorRange range, Color color)
        {
            return range?.InRangeFactor(color) ?? 1.0;
        }

        public static (double, double, double, double, double) GetLevelsParameters(double[] args)
        {
            return args.Length >= 5 ? (args[0], args[1],args[2], args[3], args[4]) : (0, 1, 1, 0, 1);
        }

        public static (double, double, double) GetAutoLevelParameters(double[] args)
        {
            return args.Length >= 2 ? (args[0], args[1], args.Length >= 3 ? args[2] : 1) : (0, 1, 1);
        }

        public static double CalcLevels(double value, double rangeFactor, double[] args)
        {
            {
                var result = value;
                if (args.Length >= 5) {
                    if (rangeFactor > 0.0) {
                        double newValue =
                            ColorMath.Levels(value, args[0],  args[1],  args[2],  args[3],  args[4]);
                        result = ColorMath.LinearInterpolation(rangeFactor, value, newValue);
                    }
                }

                return result;
            }
        }

        public static (double, double) GetLowestAndHighestRgb(List<Color> colors)
            {
                bool some = false;
                double hi = 0.0;
                double lo = 1.0;
                foreach (var color in colors) {
                    double val = ColorMath.AverageRgb(color.Red, color.Green, color.Blue);
                    if (val > hi) hi = val;
                    if (val < lo) lo = val;
                    some = true;
                }

                return some ? (lo, hi) : (0, 1);
            }

            public static (double, double) GetLowestAndHighestBrightness(IEnumerable<Color> colors)
            {
                bool some = false;
                double hi = 0.0;
                double lo = 1.0;
                foreach (var color in colors) {
                    double val = ColorMath.RgbPerceivedBrightness(color.Red, color.Green, color.Blue);
                    if (val > hi) hi = val;
                    if (val < lo) lo = val;
                    some = true;
                }

                return some ? (lo, hi) : (0, 1);
            }

            public static (double, double) GetLowestAndHighestLightness(IEnumerable<Color> colors)
            {
                bool some = false;
                double hi = 0.0;
                double lo = 1.0;
                foreach (var color in colors) {
                    if (color.Lightness > hi) hi = color.Lightness;
                    if (color.Lightness < lo) lo = color.Lightness;
                    some = true;
                }

                return some ? (lo, hi) : (0, 1);
            }
        
            public static (double, double) GetLowestAndHighestValue(IEnumerable<Color> colors)
            {
                bool some = false;
                double hi = 0.0;
                double lo = 1.0;
                foreach (var color in colors) {
                    if (color.Value > hi) hi = color.Value;
                    if (color.Value < lo) lo = color.Value;
                    some = true;
                }

                return some ? (lo, hi) : (0, 1);
            }
        }
    }