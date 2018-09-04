using System;

namespace ColorSchemeInverter
{
    public static class ExtensionMethods
    {
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if(val.CompareTo(max) > 0) return max;
            else return val;
        }

        public static bool Equals(this double val, double anotherVal, double tolerance)
        {
            return (val < anotherVal + tolerance / 2.0 && val > anotherVal - tolerance / 2.0);
        }
        
        public static bool AboutEqual(this double val, double anotherVal)
        {
            const double tolerance = 0.00001;
            return (val < anotherVal + tolerance  && val > anotherVal - tolerance);
        }
    }
}