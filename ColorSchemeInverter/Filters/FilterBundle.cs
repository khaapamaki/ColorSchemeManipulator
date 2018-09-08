using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using ColorSchemeInverter.CLI;
using ColorSchemeInverter.Colors;
using ColorSchemeInverter.Common;

namespace ColorSchemeInverter.Filters
{
    public sealed class FilterBundle
    {
        private static FilterBundle _instance;
        private static readonly object Padlock = new object();

        private FilterBundle() { }

        private static FilterBundle GetInstance()
        {
            lock (Padlock) {
                return _instance ?? (_instance = new FilterBundle());
            }
        }

        private bool _isRegistered = false;

        public static void RegisterCliOptions()
        {
            if (GetInstance()._isRegistered)
                return;

            CliArgs.Register(new List<string> {"-b", "--brightness"}, RGBGain, 1);
            CliArgs.Register(new List<string> {"-c", "--contrast"}, Contrast, 1);
            CliArgs.Register(new List<string> {"-g", "--gamma"}, Gamma, 1);
            CliArgs.Register(new List<string> {"-gr", "--gamma-red"}, GammaRed, 1);
            CliArgs.Register(new List<string> {"-gg", "--gamma-green"}, GammaGreen, 1);
            CliArgs.Register(new List<string> {"-gb", "--gamma-blue"}, GammaBlue, 1);

            CliArgs.Register(new List<string> {"--levels"}, Levels, 5);
            CliArgs.Register(new List<string> {"--levels-red"}, LevelsRed, 5);
            CliArgs.Register(new List<string> {"--levels-green"}, LevelsGreen, 5);
            CliArgs.Register(new List<string> {"--levels-blue"}, LevelsBlue, 5);
            CliArgs.Register(new List<string> {"-i", "--invert"}, Invert, 0);

            CliArgs.Register(new List<string> {"-l", "--lightness"}, LightnessGain, 1);
            CliArgs.Register(new List<string> {"-lc", "--lightness-contrast"}, LightnessContrast, 1);
            CliArgs.Register(new List<string> {"-lg", "--lightness-gamma"}, LightnessGamma, 1);
            CliArgs.Register(new List<string> {"--lightness-levels"}, LightnessLevels, 5);
            CliArgs.Register(new List<string> {"-li", "--lightness-invert"}, LightnessInvert, 0);

            CliArgs.Register(new List<string> {"-s", "--saturation"}, SaturationGain, 1);
            CliArgs.Register(new List<string> {"-sc", "--saturation-contrast"}, SaturationContrast, 1);
            CliArgs.Register(new List<string> {"-sg", "--saturation-gamma"}, SaturationGamma, 1);
            CliArgs.Register(new List<string> {"--saturation-levels"}, SaturationLevels, 5);
        }


        #region "Invert"

        public static RGB Invert(RGB rgb, params object[] args)
        {
            // todo clamp to min 0.0
            return new RGB(rgb) {Red = 1.0 - rgb.Red, Green = 1.0 - rgb.Green, Blue = 1.0 - rgb.Blue};
        }
        
        public static HSL LightnessInvert(HSL hsl, params object[] _)
        {
            var result = new HSL(hsl);
            result.Lightness = 1.0 - result.Lightness;
            return result;
        }

        #endregion
        
        #region "RGBGain"

        public static HSL SaturationGain(HSL hsl, params object[] args)
        {
            var result = new HSL(hsl);
            if (args.Any() && FilterUtils.IsNumberOrString(args[0])) {
                double gain = FilterUtils.GetDouble(args[0]);
                result.Saturation = result.Saturation * gain;
            }

            return result;
        }

        public static RGB RGBGain(RGB rgb, params object[] args)
        {
            var result = new RGB(rgb);
            if (args.Any() && FilterUtils.IsNumberOrString(args[0])) {
                double gain = FilterUtils.GetDouble(args[0]);
                result.Red = ColorMath.Gain(rgb.Red, gain);
                result.Green = ColorMath.Gain(rgb.Green, gain);
                result.Blue = ColorMath.Gain(rgb.Blue, gain);
            }

            return result;
        }
        
        public static HSL LightnessGain(HSL hsl, params object[] args)
        {
            var result = new HSL(hsl);
            if (args.Any() && args[0] is double gain) {
                result.Lightness = (result.Lightness * gain);
            }

            return result;
        }

        #endregion

        #region "Gamma"

        public static RGB Gamma(RGB rgb, params object[] args)
        {
            var result = new RGB(rgb);
            if (args.Any() && FilterUtils.IsNumberOrString(args[0])) {
                double gamma = FilterUtils.GetDouble(args[0]);
                result.Red = ColorMath.Gamma(rgb.Red, gamma);
                result.Green = ColorMath.Gamma(rgb.Green, gamma);
                result.Blue = ColorMath.Gamma(rgb.Blue, gamma);
            }

            return result;
        }

        public static RGB GammaRed(RGB rgb, params object[] args)
        {
            var result = new RGB(rgb);

            if (args.Any() && FilterUtils.IsNumberOrString(args[0])) {
                double saturationThreshold = args.Length >= 2 ? FilterUtils.GetDouble(args[1]) : -1;
                if (rgb.ToHSL().Saturation > saturationThreshold) {
                    double gamma = FilterUtils.GetDouble(args[0]);
                    result.Red = ColorMath.Gamma(rgb.Red, gamma);
                }
            }

            return result;
        }

        public static RGB GammaGreen(RGB rgb, params object[] args)
        {
            var result = new RGB(rgb);

            if (args.Any() && FilterUtils.IsNumberOrString(args[0])) {
                double saturationThreshold = args.Length >= 2 ? FilterUtils.GetDouble(args[1]) : -1;
                if (rgb.ToHSL().Saturation > saturationThreshold) {
                    double gamma = FilterUtils.GetDouble(args[0]);
                    result.Green = ColorMath.Gamma(rgb.Green, gamma);
                }
            }

            return result;
        }

        public static RGB GammaBlue(RGB rgb, params object[] args)
        {
            var result = new RGB(rgb);

            if (args.Any() && FilterUtils.IsNumberOrString(args[0])) {
                double saturationThreshold = args.Length >= 2 ? FilterUtils.GetDouble(args[1]) : -1;
                if (rgb.ToHSL().Saturation > saturationThreshold) {
                    double gamma = FilterUtils.GetDouble(args[0]);
                    result.Blue = ColorMath.Gamma(rgb.Blue, gamma);
                }
            }

            return result;
        }

        public static HSL SaturationGamma(HSL hsl, params object[] args)
        {
            var result = new HSL(hsl);
            if (args.Any() && FilterUtils.IsNumberOrString(args[0])) {
                double gamma = FilterUtils.GetDouble(args[0]);
                result.Saturation = ColorMath.Gamma(hsl.Saturation, gamma);
            }

            return result;
        }

        public static HSL LightnessGamma(HSL hsl, params object[] args)
        {
            var result = new HSL(hsl);
            if (args.Any() && FilterUtils.IsNumberOrString(args[0])) {
                double gamma = FilterUtils.GetDouble(args[0]);
                result.Lightness = ColorMath.Gamma(hsl.Lightness, gamma);
            }

            return result;
        }

        #endregion

        #region "Contrast"

        public static RGB Contrast(RGB hsl, params object[] args)
        {
            var result = new RGB(hsl);
            if (args.Any() && FilterUtils.IsNumberOrString(args[0])) {
                double strength = FilterUtils.GetDouble(args[0]);
                if (args.Length >= 2 && FilterUtils.IsNumberOrString(args[1])) {
                    double midpoint = FilterUtils.GetDouble(args[1]);
                    result.Red = ColorMath.SSpline(hsl.Red, strength, midpoint);
                    result.Green = ColorMath.SSpline(hsl.Green, strength, midpoint);
                    result.Blue = ColorMath.SSpline(hsl.Blue, strength, midpoint);
                } else {
                    result.Red = ColorMath.SSpline(hsl.Red, strength);
                    result.Green = ColorMath.SSpline(hsl.Green, strength);
                    result.Blue = ColorMath.SSpline(hsl.Blue, strength);
                }
            }

            return result;
        }

        public static HSL SaturationContrast(HSL hsl, params object[] args)
        {
            var result = new HSL(hsl);
            if (args.Any() && FilterUtils.IsNumberOrString(args[0])) {
                double strength = FilterUtils.GetDouble(args[0]);
                if (args.Length >= 2 && FilterUtils.IsNumberOrString(args[1])) {
                    double midpoint = FilterUtils.GetDouble(args[1]);
                    result.Saturation = ColorMath.SSpline(hsl.Saturation, strength, midpoint);
                } else {
                    result.Saturation = ColorMath.SSpline(hsl.Saturation, strength);
                }
            }

            return result;
        }

        public static HSL LightnessContrast(HSL hsl, params object[] args)
        {
            var result = new HSL(hsl);
            if (args.Any() && FilterUtils.IsNumberOrString(args[0])) {
                double strength = FilterUtils.GetDouble(args[0]);
                if (args.Length >= 2 && FilterUtils.IsNumberOrString(args[1])) {
                    double midpoint = FilterUtils.GetDouble(args[1]);
                    result.Lightness = ColorMath.SSpline(hsl.Lightness, strength, midpoint);
                } else {
                    result.Lightness = ColorMath.SSpline(hsl.Lightness, strength);
                }
            }

            return result;
        }

        #endregion


        #region "Levels based filters"

        public static RGB Levels(RGB rgb, params object[] args)
        {
            var result = new RGB(rgb);
            double rangeFactor = FilterUtils.GetRangeFactor(rgb, args, 5);
            result.Red = FilterUtils.CalcLevels(rgb.Red, rangeFactor, args);
            result.Green = FilterUtils.CalcLevels(rgb.Green, rangeFactor, args);
            result.Blue = FilterUtils.CalcLevels(rgb.Blue, rangeFactor, args);
            return result;
        }

        public static RGB LevelsRed(RGB rgb, params object[] args)
        {
            var result = new RGB(rgb);
            double rangeFactor = FilterUtils.GetRangeFactor(rgb, args, 5);
            result.Red = FilterUtils.CalcLevels(rgb.Red, rangeFactor, args);
            return result;
        }

        public static RGB LevelsGreen(RGB rgb, params object[] args)
        {
            var result = new RGB(rgb);
            double rangeFactor = FilterUtils.GetRangeFactor(rgb, args, 5);
            result.Green = FilterUtils.CalcLevels(rgb.Green, rangeFactor, args);
            return result;
        }

        public static RGB LevelsBlue(RGB rgb, params object[] args)
        {
            var result = new RGB(rgb);
            double rangeFactor = FilterUtils.GetRangeFactor(rgb, args, 5);
            result.Blue = FilterUtils.CalcLevels(rgb.Blue, rangeFactor, args);
            return result;
        }

        public static HSL LightnessLevels(HSL hsl, params object[] args)
        {
            var result = new HSL(hsl);
            double rangeFactor = FilterUtils.GetRangeFactor(hsl, args, 5);
            result.Lightness = FilterUtils.CalcLevels(hsl.Lightness, rangeFactor, args);
            return result;
        }

        public static HSL SaturationLevels(HSL hsl, params object[] args)
        {
            var result = new HSL(hsl);
            double rangeFactor = FilterUtils.GetRangeFactor(hsl, args, 5);
            result.Saturation = FilterUtils.CalcLevels(hsl.Saturation, rangeFactor, args);
            return result;
        }

        #endregion

        // just dummy template
        private static RGB DoNothing(RGB rgb, params object[] args)
        {
            var result = new RGB(rgb);
            // do something here
            return result;
        }
    }
}