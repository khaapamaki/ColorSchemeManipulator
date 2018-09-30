using System;
using System.Collections.Generic;
using System.Linq;
using ColorSchemeManipulator.Colors;
using ColorSchemeManipulator.Common;

namespace ColorSchemeManipulator.Filters
{
    public static class FilterUtils
    {
        public static double? TryParseDouble(object obj)
        {
            if (obj is double d)
                return d;

            if (obj is int)
                return Convert.ToDouble(obj);

            if (IsNumber(obj)) {
                try {
                    return Convert.ToDouble(obj);
                } catch (Exception) {
                    throw new Exception("Cannot convert " + obj + " to double");
                }
            }

            if (obj is string s) {
                try {
                    return double.Parse(s);
                } catch (Exception) {
                    return null;
                }
            }

            return null;
        }

        // This is quite funny. There must be a better way...
        public static bool IsNumber(object o)
        {
            return o is double || o is decimal || o is int || o is byte || o is float
                   || o is long || o is short || o is uint || o is ulong || o is ushort;
        }

        public static bool IsNumberOrString(object o)
        {
            return IsNumber(o) || o is string;
        }

        [Obsolete]
        public static (ColorRange, object[]) GetRangeAndRemainingParams(double[] args)
        {
            throw new NotSupportedException();
            // if (args != null && args.Length > 0) {
            //     int lastIndex = args.Length - 1;
            //     var lastArg = args[lastIndex];
            //     if (lastArg is ColorRange) {
            //         List<object> temp = args.ToList();
            //         temp.Remove(lastArg);
            //         return ((ColorRange) lastArg, temp.ToArray());
            //     }
            // }
            //
            // return (null, args);
        }

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

        public static (double, double) GetLowestAndHighestRgb(IEnumerable<Color> colors)
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
        }
    }