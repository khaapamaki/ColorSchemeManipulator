using System;

namespace ColorSchemeInverter.Filters
{
    public static class ColorMath
    {
        public static double Invert(double d)
        {
            return 1.0 - d;
        }
        
        public static double Gamma(double x, double gamma)
        {
            double y = x;
            if (x > 0 && x < 1) {
                y = Math.Pow(x, 1.0 / gamma);
            }
            return y;
        }

        public static double SSpline(double x, double s, double a = 0.5)
        {
            // a=point of inflection 0..1
            // s=strength of curve -1..1
            
            double y = x;
            
            if (x >= 0 && x < a) { 
                y = (1 - s) * x + s * (a * Math.Pow(x / a, 2.0));
            } else if (x >= a && x <= 1) {
                y = (1.0 - s) * x + s * (-(1.0 - a) * Math.Pow((1.0 - x)*(1.0 - a), 2.0) + 1.0);
            }

            return y;
            
            // Perfect solution Emil, simple and elegant. Just needs an extra powering to
            // shift the turning point from (a,a) to (a,b):
            // y = [ (1-s)*x + s*a*(x/a)^E ] ^ ( log(b)/log(a) )
            // https://forum.luminous-landscape.com/index.php?topic=52364.0

        }

        public static double Gain(double x, double gain)
        {
            return x * gain;
        }

        public static double Levels(double x, double black, double gamma, double white)
        {
            throw new NotImplementedException();
        }
        
    }
}