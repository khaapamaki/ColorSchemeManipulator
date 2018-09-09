using System;

namespace ColorSchemeInverter.Common
{
    public static class GenericExtensions
    {
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if(val.CompareTo(max) > 0) return max;
            else return val;
        }
        
        public static T Min<T>(this T val, T min) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else return val;
        }

        public static T Max<T>(this T val, T max) where T : IComparable<T>
        {
            if(val.CompareTo(max) > 0) return max;
            else return val;
        }
      
        
    }
}