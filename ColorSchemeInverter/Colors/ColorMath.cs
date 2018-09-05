using System;

namespace ColorSchemeInverter.Colors
{
    public static class ColorMath
    {
        public static double Invert(double d)
        {
            return 1.0 - d;
        }
        
        public static double Gamma(double d, double gamma)
        {
            throw new NotImplementedException();
            return d;
        }
        
    }
}