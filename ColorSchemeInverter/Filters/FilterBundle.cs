using System;
using System.Collections.Generic;
using System.Data.Common;
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
            
            CliArgs.Register(new List<string> { "-b", "--brightness"}, Gain, 1);
            CliArgs.Register(new List<string> { "-c", "--contrast"}, Contrast, 1);
            CliArgs.Register(new List<string> { "-g", "--gamma"}, Gamma, 1);
            CliArgs.Register(new List<string> { "--levels"}, Levels, 5);
            CliArgs.Register(new List<string> { "--levels-red"}, LevelsRed, 5);
            CliArgs.Register(new List<string> { "--levels-green"}, LevelsGreen, 5); 
            CliArgs.Register(new List<string> { "--levels-blue"}, LevelsBlue, 5); 
            CliArgs.Register(new List<string> { "-i", "--invert"}, Invert, 0); 
            
            CliArgs.Register(new List<string> { "-l", "--lightness"}, LightnessGain, 1);
            CliArgs.Register(new List<string> { "-lc", "--lightness-contrast"}, LightnessContrast, 1);
            CliArgs.Register(new List<string> { "-lg", "--lightness-gamma"}, LightnessGamma, 1);
            CliArgs.Register(new List<string> { "--lightness-levels"}, LightnessLevels, 5); 
            CliArgs.Register(new List<string> { "-li", "--lightness-invert"}, LightnessInvert, 0);

            CliArgs.Register(new List<string> { "-s", "--saturation"}, SaturationGain, 1);
            CliArgs.Register(new List<string> { "-sc", "--saturation-contrast"}, SaturationContrast, 1);
            CliArgs.Register(new List<string> { "-sg", "--saturation-gamma"}, SaturationGamma, 1);
            CliArgs.Register(new List<string> { "--saturation-levels"}, SaturationLevels, 5); 
        }
        
        public static HSL LightnessInvert(HSL hsl, params object[] _)
        {
            var result = new HSL(hsl);
            result.Lightness = 1.0 - result.Lightness;
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

        public static HSL SaturationGain(HSL hsl, params object[] args)
        {
            var result = new HSL(hsl);
            if (args.Any() && FilterUtils.IsNumberOrString(args[0])) {
                double gain = FilterUtils.GetDouble(args[0]);
                result.Saturation = result.Saturation * gain;
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

        public static HSL SaturationLevels(HSL hsl, params object[] args)
        {
            var result = new HSL(hsl);
        
            if (args.Length >= 5) {
                bool skipGrayScaleColors = false;
                if (args.Length >= 6) {
                    skipGrayScaleColors = FilterUtils.GetBoolean(args[5]);
                }
                if (! hsl.ToHSL().Saturation.AboutEqual(0.0) || ! skipGrayScaleColors) {
                    double inputBlack = FilterUtils.GetDouble(args[0]);
                    double inputWhite = FilterUtils.GetDouble(args[1]);
                    double midtones = FilterUtils.GetDouble(args[2]);
                    double outputWhite = FilterUtils.GetDouble(args[3]);
                    double outputBlack = FilterUtils.GetDouble(args[4]);
                    result.Saturation = ColorMath.Levels(hsl.Saturation, inputBlack, inputWhite, midtones, outputWhite, outputBlack); 
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
        
        public static HSL LightnessLevels(HSL hsl, params object[] args)
        {
            var result = new HSL(hsl);
        
            if (args.Length >= 5) {
                bool skipGrayScaleColors = false;
                if (args.Length >= 6) {
                    skipGrayScaleColors = FilterUtils.GetBoolean(args[5]);
                }
                if (! hsl.ToHSL().Saturation.AboutEqual(0.0) || ! skipGrayScaleColors) {
                    double inputBlack = FilterUtils.GetDouble(args[0]);
                    double inputWhite = FilterUtils.GetDouble(args[1]);
                    double midtones = FilterUtils.GetDouble(args[2]);
                    double outputWhite = FilterUtils.GetDouble(args[3]);
                    double outputBlack = FilterUtils.GetDouble(args[4]);
                    result.Lightness = ColorMath.Levels(hsl.Lightness, inputBlack, inputWhite, midtones, outputWhite, outputBlack); 
                }
            }

            return result;
        }

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

        public static RGB Gain(RGB rgb, params object[] args)
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

        public static RGB Invert(RGB rgb, params object[] args)
        {
            return new RGB(rgb) {Red = 1.0 - rgb.Red, Green = 1.0 - rgb.Green, Blue = 1.0 - rgb.Blue};
        }

        public static RGB Levels(RGB rgb, params object[] args)
        {
            var result = new RGB(rgb);

            if (args.Length >= 5) {
                bool skipGrayScaleColors = false;
                if (args.Length >= 6) {
                    skipGrayScaleColors = FilterUtils.GetBoolean(args[5]);
                }

                if (!rgb.ToHSL().Saturation.AboutEqual(0.0) || !skipGrayScaleColors) {
                    double inputBlack = FilterUtils.GetDouble(args[0]);
                    double inputWhite = FilterUtils.GetDouble(args[1]);
                    double midtones = FilterUtils.GetDouble(args[2]);
                    double outputWhite = FilterUtils.GetDouble(args[3]);
                    double outputBlack = FilterUtils.GetDouble(args[4]);
                    result.Red = ColorMath.Levels(rgb.Red, inputBlack, inputWhite, midtones, outputWhite, outputBlack);
                    result.Green = ColorMath.Levels(rgb.Green, inputBlack, inputWhite, midtones, outputWhite,
                        outputBlack);
                    result.Blue = ColorMath.Levels(rgb.Blue, inputBlack, inputWhite, midtones, outputWhite,
                        outputBlack);
                }
            }

            return result;
        }

        public static RGB LevelsRed(RGB rgb, params object[] args)
        {
            var result = new RGB(rgb);
        
            if (args.Length >= 5) {
                bool skipGrayScaleColors = false;
                if (args.Length >= 6) {
                    skipGrayScaleColors = FilterUtils.GetBoolean(args[5]);
                }
                if (! rgb.ToHSL().Saturation.AboutEqual(0.0) || ! skipGrayScaleColors) {
                    double inputBlack = FilterUtils.GetDouble(args[0]);
                    double inputWhite = FilterUtils.GetDouble(args[1]);
                    double midtones = FilterUtils.GetDouble(args[2]);
                    double outputWhite = FilterUtils.GetDouble(args[3]);
                    double outputBlack = FilterUtils.GetDouble(args[4]);
                    result.Red = ColorMath.Levels(rgb.Red, inputBlack, inputWhite, midtones, outputWhite, outputBlack); 
                }
            }

            return result;
        }

        public static RGB LevelsGreen(RGB rgb, params object[] args)
        {
            var result = new RGB(rgb);

            if (args.Length >= 5) {
                bool skipGrayScaleColors = false;
                if (args.Length >= 6) {
                    skipGrayScaleColors = FilterUtils.GetBoolean(args[5]);
                }

                if (!rgb.ToHSL().Saturation.AboutEqual(0.0) || !skipGrayScaleColors) {
                    double inputBlack = FilterUtils.GetDouble(args[0]);
                    double inputWhite = FilterUtils.GetDouble(args[1]);
                    double midtones = FilterUtils.GetDouble(args[2]);
                    double outputWhite = FilterUtils.GetDouble(args[3]);
                    double outputBlack = FilterUtils.GetDouble(args[4]);
                    result.Green = ColorMath.Levels(rgb.Green, inputBlack, inputWhite, midtones, outputWhite,
                        outputBlack);
                }
            }

            return result;
        }
        
        public static RGB LevelsBlue(RGB rgb, params object[] args)
        {
            var result = new RGB(rgb);

            if (args.Length >= 5) {
                bool skipGrayScaleColors = false;
                if (args.Length >= 6) {
                    skipGrayScaleColors = FilterUtils.GetBoolean(args[5]);
                }
                if (! rgb.ToHSL().Saturation.AboutEqual(0.0) || ! skipGrayScaleColors) {
                double inputBlack = FilterUtils.GetDouble(args[0]);
                double inputWhite = FilterUtils.GetDouble(args[1]);           
                double midtones = FilterUtils.GetDouble(args[2]);
                double outputWhite = FilterUtils.GetDouble(args[3]);
                double outputBlack = FilterUtils.GetDouble(args[4]);
                result.Blue = ColorMath.Levels(rgb.Blue, inputBlack, inputWhite, midtones, outputWhite, outputBlack); 
                    }
            }

            return result;
        }
        


        // just dummy template
        private static RGB DoNothing(RGB rgb, params object[] args)
        {
            var result = new RGB(rgb);
            // do something here
            return result;
        }
    }
}