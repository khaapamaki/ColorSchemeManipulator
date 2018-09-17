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
            if (obj is double)
                return (double) obj;

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

        public static (ColorRange, object[]) GetRangeAndRemainingParams(object[] args)
        {
            if (args != null && args.Length > 0) {
                int lastIndex = args.Length - 1;
                var lastArg = args[lastIndex];
                if (lastArg is ColorRange) {
                    List<object> temp = args.ToList();
                    temp.Remove(lastArg);
                    return ((ColorRange) lastArg, temp.ToArray());
                }
            }

            return (null, args);
        }

        public static double GetRangeFactor(ColorRange range, Color color)
        {
            return range?.InRangeFactor(color) ?? 1.0;
        }

        public static (double, double, double, double, double) ParseLevelsParameters(object[] args)
        {
            if (args.Length >= 5) {
                double inputBlack = TryParseDouble(args[0]) ?? 0.0;
                double inputWhite = TryParseDouble(args[1]) ?? 1.0;
                double gamma = TryParseDouble(args[2]) ?? 1.0;
                double outputBlack = TryParseDouble(args[3]) ?? 0.0;
                double outputWhite = TryParseDouble(args[4]) ?? 1.0;

                return (inputBlack, inputWhite, gamma, outputBlack, outputWhite);
            }

            return (0, 1, 1, 0, 1);
        }

        public static (double, double, double) ParseAutoLevelParameters(object[] args)
        {
            if (args.Length >= 2) {
                double outputBlack = TryParseDouble(args[0]) ?? 0.0;
                double outputWhite = TryParseDouble(args[1]) ?? 1.0;
                double gamma = 1;
                if (args.Length >= 3) {
                    gamma = TryParseDouble(args[2]) ?? 1.0;
                }

                return (outputBlack, outputWhite, gamma);
            }

            return (0, 1, 1);
        }

        public static double CalcLevels(double value, double rangeFactor, object[] args)
        {
            var result = value;
            if (args.Length >= 5) {
                if (rangeFactor > 0.0) {
                    double inputBlack = TryParseDouble(args[0]) ?? 0.0;
                    double inputWhite = TryParseDouble(args[1]) ?? 1.0;
                    double gamma = TryParseDouble(args[2]) ?? 1.0;
                    double outputWhite = TryParseDouble(args[3]) ?? 0.0;
                    double outputBlack = TryParseDouble(args[4]) ?? 1.0;
                    double newValue =
                        ColorMath.Levels(value, inputBlack, inputWhite, gamma, outputWhite, outputBlack);
                    result = ColorMath.LinearInterpolation(rangeFactor, value, newValue);
                }
            }

            return result;
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

        public static double GetHighestLightness(IEnumerable<Color> colors)
        {
            throw new NotImplementedException();
        }
    }
}