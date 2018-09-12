using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using ColorSchemeInverter.Colors;
using ColorSchemeInverter.Common;

namespace ColorSchemeInverter.Filters
{
    public static class FilterUtils
    {
        public static double? TryParseDouble(object obj)
        {
            if (obj is double)
                return (double) obj;
            
            if (IsNumber(obj)) {
                try {
                    return (double) obj;
                } catch (Exception) {
                    return null;
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

        // public static bool GetBoolean(object obj)
        // {
        //     if (obj is bool) {
        //         return (bool) obj;
        //     }
        //
        //     if (IsNumber(obj)) {
        //         try {
        //             double d = TryParseDouble(obj);
        //             return d.AboutEqual(1.0);
        //         } catch (Exception e) {
        //             Console.WriteLine("Could not convert object to boolean");
        //         }
        //     }
        //     
        //     // Todo this is very flawed, fix asap
        //     if (obj is string s) {
        //         try {
        //
        //             return s == "1" || s.ToLower() == "true";
        //         } catch (Exception e) {
        //             Console.WriteLine("Could not parse string to boolean");
        //         }
        //     }
        //
        //     Console.WriteLine("Object is not a boolean, boolean expected");
        //     return false;
        // }


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

        public static (double, object[]) GetRangeFactorAndRemainingParams(Rgb rgb, object[] filterParams)
        {
            ColorRange range;
            (range, filterParams) = GetRangeAndRemainingParams(filterParams);

            return (range?.InRangeFactor(rgb) ?? 1.0, filterParams);
        }

        public static (double, object[]) GetRangeFactorAndRemainingParams(Hsl hsl, object[] filterParams)
        {
            ColorRange range;
            (range, filterParams) = GetRangeAndRemainingParams(filterParams);

            return (range?.InRangeFactor(hsl) ?? 1.0, filterParams);
        }

        public static (double, object[]) GetRangeFactorAndRemainingParams(Hsv hsv, object[] filterParams)
        {
            ColorRange range;
            (range, filterParams) = GetRangeAndRemainingParams(filterParams);

            return (range?.InRangeFactor(hsv) ?? 1.0, filterParams);
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
    }
}