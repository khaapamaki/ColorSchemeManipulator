using System;
using System.Collections.Generic;

namespace ColorSchemeManipulator.Common
{
    public static class GenericExtensions
    {
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            var realMin = min.Min(max);
            var realMax = max.Max(min);

            if (val.CompareTo(realMin) < 0) return realMin;
            else if (val.CompareTo(realMax) > 0) return realMax;
            else return val;
        }

        public static T LimitHigh<T>(this T val, T min) where T : IComparable<T>
        {
            return val.CompareTo(min) > 0 ? min : val;
        }

        public static T LimitLow<T>(this T val, T max) where T : IComparable<T>
        {
            return val.CompareTo(max) < 0 ? max : val;
        }
        
        public static T Min<T>(this T val, T min) where T : IComparable<T>
        {
            return val.CompareTo(min) > 0 ? min : val;
        }

        public static T Max<T>(this T val, T max) where T : IComparable<T>
        {
            return val.CompareTo(max) < 0 ? max : val;
        }
        
        public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action)
        {
            // argument null checking omitted
            foreach (var item in sequence)
                action(item);
        }
        
    }
}