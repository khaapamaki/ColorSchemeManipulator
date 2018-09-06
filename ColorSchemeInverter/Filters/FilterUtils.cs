using System;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Text;

namespace ColorSchemeInverter.Filters
{
    public class FilterUtils
    {
        public static double GetDouble(object obj)
        {
            if (IsNumber(obj))
                try {
                    return (double) obj;
                } catch (Exception e) {
                    Console.WriteLine("Could not cast object to double");
                }

            if (obj is string s)
                try {
                    return double.Parse(s);
                } catch (Exception e) {
                    Console.WriteLine("Could not parse string to double");
                }

            Console.WriteLine("Object is not a number, double expected");
            return 0.0;
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
    }
}