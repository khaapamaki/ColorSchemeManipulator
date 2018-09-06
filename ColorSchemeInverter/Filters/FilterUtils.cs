using System;
using System.Diagnostics;
using System.Text;

namespace ColorSchemeInverter.Filters
{
    public class FilterUtils
    {
        public static double GetDouble(object obj)
        {
            if (IsNumber(obj))
                return (double) obj;
            if (obj is string s)
                return double.Parse(s);
            return 0.0;
        }


        public static bool IsNumber(object o)
        {
            return o is double || o is decimal || o is int || o is byte || o is float;
        }
        
        public static bool IsNumberOrString(object o)
        {
            return o is double || o is decimal || o is int || o is byte || o is float || o is string;
        }
    }
    
   
}