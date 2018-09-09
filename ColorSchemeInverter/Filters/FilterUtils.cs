using System;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Text;
using ColorSchemeInverter.Colors;
using ColorSchemeInverter.Common;

namespace ColorSchemeInverter.Filters
{
    public class FilterUtils
    {
        public static double GetDouble(object obj)
        {
            if (IsNumber(obj)) {
                try {
                    return (double) obj;
                } catch (Exception e) {
                    Console.WriteLine("Could not cast object to double");
                }
            }

            if (obj is string s) {
                try {
                    return double.Parse(s);
                } catch (Exception e) {
                    Console.WriteLine("Could not parse string to double");
                }
            }

            Console.WriteLine("Object is not a number, double expected");
            return 0.0;
        }

        public static bool GetBoolean(object obj)
        {
            if (obj is bool) {
                return (bool) obj;
            }

            if (IsNumber(obj)) {
                try {
                    double d = GetDouble(obj);
                    return d.AboutEqual(1.0);
                } catch (Exception e) {
                    Console.WriteLine("Could not convert object to boolean");
                }
            }
            
            // Todo this is very flawed, fix asap
            if (obj is string s) {
                try {

                    return s == "1" || s.ToLower() == "true";
                } catch (Exception e) {
                    Console.WriteLine("Could not parse string to boolean");
                }
            }

            Console.WriteLine("Object is not a boolean, boolean expected");
            return false;
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
        
        public static double Linear(double x, double x0, double x1, double y0, double y1)
        {
            if (x1 - x0 == 0.0)
            {
                return (y0 + y1) / 2;
            }
            return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
        }
        
        public static double Linear01(double x, double y0, double y1)
        {
            const double x1 = 1.0;
            const double x0 = 0.0;
            x = x.Clamp(0, 1);
            return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
        } 
        
                
        // Todo Change behavior this always looks for range at the end, extracts it and returns remaining args + range
        public static double GetRangeFactor(Hsl hsl, object[] args, byte index)
        {
            return args.Length > index && args[index] is ColorRange range ? range.InRangeFactor(hsl) : 1.0;
        }
        
        public static double GetRangeFactor(Rgb rgb, object[] args, byte index)
        {
            return args.Length > index && args[index] is ColorRange range ? range.InRangeFactor(rgb) : 1.0;
        }
   
        public static double GetRangeFactor(Hsv hsv, object[] args, byte index)
        {
            return args.Length > index && args[index] is ColorRange range ? range.InRangeFactor(hsv) : 1.0;
        }
        
        public static double CalcLevels(double value, double rangeFactor, object[] args)
        {
            var result = value;
            if (args.Length >= 5) {                
                if (rangeFactor > 0.0) {
                    double inputBlack = FilterUtils.GetDouble(args[0]);
                    double inputWhite = FilterUtils.GetDouble(args[1]);
                    double midtones = FilterUtils.GetDouble(args[2]);
                    double outputWhite = FilterUtils.GetDouble(args[3]);
                    double outputBlack = FilterUtils.GetDouble(args[4]);
                    double newValue = ColorMath.Levels(value, inputBlack, inputWhite, midtones, outputWhite, outputBlack);
                    result = FilterUtils.Linear01(rangeFactor, value, newValue);
                }
            }
            return result;
        }

    }
}