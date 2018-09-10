namespace ColorSchemeInverter.Common
{
    public static class DoubleExtensions
    {
        public static bool Equals(this double val, double anotherVal, double tolerance)
        {
            return val < anotherVal + tolerance / 2.0 && val > anotherVal - tolerance / 2.0;
        }
        
        public static bool AboutEqual(this double val, double anotherVal)
        {
            const double tolerance = 0.00001;
            return val < anotherVal + tolerance  && val > anotherVal - tolerance;
        }

        public static double NormalizeLoopingValue(this double val, double range)
        {
            if (val == range)
                return val;
            double temp = val % range;
            return temp >= 0 ? temp : range + temp;
        }
    }
}