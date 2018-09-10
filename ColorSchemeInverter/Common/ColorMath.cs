using System;
using System.Dynamic;
using ColorSchemeInverter.Colors;
using ColorSchemeInverter.Common;

namespace ColorSchemeInverter.Filters
{
    public static class ColorMath
    {
        public static double Invert(double input)
        {
            return (1.0 - input).LimitLow(0.0);
        }

        public static double Gamma(double input, double gamma)
        {
            gamma = gamma.Clamp(0.01, 9.99);
            double output = input;
            if (input > 0 && input < 1) {
                output = Math.Pow(input, 1.0 / gamma);
            }

            return output.LimitLow(0.0);
        }

        public static double SSpline(double input, double s, double a = 0.5)
        {
            // a=point of inflection 0..1
            // s=strength of curve -1..1

            double output = input;
            a = a.Clamp(0, 1);
            input = a.LimitLow(0);
            s = s.Clamp(-1, 1);
            
            if (input >= 0 && input < a) {
                output = (1 - s) * input + s * (a * Math.Pow(input / a, 2.0));
            } else if (input >= a && input <= 1) {
                output = (1.0 - s) * input + s * (-(1.0 - a) * Math.Pow((1.0 - input) * (1.0 - a), 2.0) + 1.0);
            }

            return output.Min(0.0);

            // Perfect solution Emil, simple and elegant. Just needs an extra powering to
            // shift the turning point from (a,a) to (a,b):
            // y = [ (1-s)*x + s*a*(x/a)^E ] ^ ( log(b)/log(a) )
            // https://forum.luminous-landscape.com/index.php?topic=52364.0
        }

        public static double Gain(double input, double gain)
        {
            return input * gain;
        }

        public static double Levels(double input,
            double inBlack, double inWhite, double gamma, double outBlack, double outWhite)
        {
            // gamma = gamma.Clamp(0.0, 1.0);
            gamma = gamma.Clamp(0.01, 9.99);
            inBlack = inBlack.Clamp(0.0, 1.0);
            inWhite = inWhite.Clamp(0.0, 1.0);
            // outBlack = outBlack.Clamp(0.0, 1.0);
            // outWhite = outWhite.Clamp(0.0, 1.0);
            input = input.Min(0.0);
            
            // input values
            double output = ((input - inBlack) / (inWhite - inBlack)).Clamp(0.0, 1.0);

            // mid-tones
            if (!gamma.AboutEqual(0.5)) {
                output = Math.Pow(output,1 / gamma);
            }

            // output values
            output = (output * (outWhite - outBlack) + outBlack).Clamp(0.0, 1.0);

            return output.Min(0.0);
            
            // https://stackoverflow.com/questions/39510072/algorithm-for-adjustment-of-image-levels
        }

        public static double LinearInterpolation(double x, double x0, double x1, double y0, double y1)
        {
            x = x.Clamp(x0, x1);
            
            if (x == x0)
                return y0;
            
            if (x == x1)
                return y1;

            if (x1 - x0 == 0.0) {
                return (y0 + y1) / 2;
            }

            return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
        }

        
        public static double LinearInterpolation(double x, double y0, double y1)
        {
            return LinearInterpolation(x, 0.0, 1.0, y0, y1);
        }
        
        public static double LinearInterpolationForLoopingValues(double x, double x0, double x1, double y0, double y1, double loopMax)
        {
            x = x.Clamp(x0, x1);
            
            if (x == x0)
                return y0;
            
            if (x == x1)
                return y1;
            
            // Todo implement interpolation for looping values
            throw new NotImplementedException();
        }  
        
        public static double LinearInterpolationForLoopingValues(double x, double y0, double y1, double loopMax)
        {
            return LinearInterpolationForLoopingValues(x, 0.0, 1.0, y0, y1, loopMax);
        }
        
        [Obsolete]
        private static double GetGammaFromMidtoneValue(double midtones)
        {
            midtones = midtones.Clamp(0.0, 1.0);
            double gamma = 1.0;
            if (midtones < 0.5) {
                gamma = 1 + 9 * (1 - 2 * midtones);
                gamma = Math.Min(gamma, 9.99);
            } else if (midtones > 0.5) {
                gamma = 1 - (midtones * 2 - 1);
                gamma = Math.Max(gamma, 0.01);
            }

            return gamma.Clamp(0.01, 9.99);
        }
    }
}