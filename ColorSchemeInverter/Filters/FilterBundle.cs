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
    // Todo Fix issue: parameter list that has comma at beginning is not propeperly handled causing slow prosessing (images)
    // arguments are empty string that will get default values but parsing with exception handling makes it slow
    // Todo Better argument validation could be the answer
    // other option would be pre-parsing to correct type and not parsing again when the filter is reapplied.

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

            CliArgs.Register(new List<string> {"-b", "--brightness"}, RgbGain, 1);
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

        public static Rgb Invert(Rgb rgb, params object[] filterParams)
        {
            double rangeFactor;
            (rangeFactor, _) = FilterUtils.GetRangeFactorAndRemainingParams(rgb, filterParams);
            Rgb inverted = new Rgb(rgb)
            {
                Red = ColorMath.Invert(rgb.Red),
                Green = ColorMath.Invert(rgb.Green),
                Blue = ColorMath.Invert(rgb.Blue),
                Alpha = rgb.Alpha
            };

            return rgb.Interpolate(inverted, rangeFactor);
        }

        public static Hsl LightnessInvert(Hsl hsl, params object[] filterParams)
        {
            double rangeFactor;
            (rangeFactor, _) = FilterUtils.GetRangeFactorAndRemainingParams(hsl, filterParams);
            Hsl filtered = new Hsl(hsl);
            filtered.Lightness = ColorMath.Invert(hsl.Lightness);

            return hsl.Interpolate(filtered, rangeFactor);
        }

        #endregion

        #region "RGBGain"

        public static Hsl SaturationGain(Hsl hsl, params object[] filterParams)
        {
            var filtered = new Hsl(hsl);
            double rangeFactor;
            (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(hsl, filterParams);
            if (filterParams.Any() && FilterUtils.IsNumberOrString(filterParams[0])) {
                double gain = FilterUtils.TryParseDouble(filterParams[0]) ?? 1.0;
                filtered.Saturation = filtered.Saturation * gain;
            }

            return hsl.Interpolate(filtered, rangeFactor);
        }

        public static Rgb RgbGain(Rgb rgb, params object[] filterParams)
        {
            var filtered = new Rgb(rgb);
            double rangeFactor;
            (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(rgb, filterParams);
            if (filterParams.Any() && FilterUtils.IsNumberOrString(filterParams[0])) {
                double gain = FilterUtils.TryParseDouble(filterParams[0]) ?? 1.0;
                filtered.Red = ColorMath.Gain(rgb.Red, gain);
                filtered.Green = ColorMath.Gain(rgb.Green, gain);
                filtered.Blue = ColorMath.Gain(rgb.Blue, gain);
            }

            return rgb.Interpolate(filtered, rangeFactor);
        }

        public static Hsl LightnessGain(Hsl hsl, params object[] filterParams)
        {
            var filtered = new Hsl(hsl);
            double rangeFactor;
            (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(hsl, filterParams);
            if (filterParams.Any() && FilterUtils.IsNumberOrString(filterParams[0])) {
                double gain = FilterUtils.TryParseDouble(filterParams[0]) ?? 1.0;
                filtered.Lightness = filtered.Lightness * gain;
            }

            return hsl.Interpolate(filtered, rangeFactor);
        }

        #endregion

        #region "Gamma"

        public static Rgb Gamma(Rgb rgb, params object[] filterParams)
        {
            var filtered = new Rgb(rgb);
            double rangeFactor;
            (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(rgb, filterParams);
            if (filterParams.Any() && FilterUtils.IsNumberOrString(filterParams[0])) {
                double gain = FilterUtils.TryParseDouble(filterParams[0]) ?? 1.0;
                filtered.Red = ColorMath.Gamma(rgb.Red, gain);
                filtered.Green = ColorMath.Gamma(rgb.Green, gain);
                filtered.Blue = ColorMath.Gamma(rgb.Blue, gain);
            }

            return rgb.Interpolate(filtered, rangeFactor);
        }

        public static Rgb GammaRed(Rgb rgb, params object[] filterParams)
        {
            var filtered = new Rgb(rgb);
            double rangeFactor;
            (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(rgb, filterParams);
            if (filterParams.Any() && FilterUtils.IsNumberOrString(filterParams[0])) {
                double gain = FilterUtils.TryParseDouble(filterParams[0]) ?? 1.0;
                filtered.Red = ColorMath.Gamma(rgb.Red, gain);
            }

            return rgb.Interpolate(filtered, rangeFactor);
        }

        public static Rgb GammaGreen(Rgb rgb, params object[] filterParams)
        {
            var filtered = new Rgb(rgb);
            double rangeFactor;
            (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(rgb, filterParams);
            if (filterParams.Any() && FilterUtils.IsNumberOrString(filterParams[0])) {
                double gain = FilterUtils.TryParseDouble(filterParams[0]) ?? 1.0;
                filtered.Green = ColorMath.Gamma(rgb.Green, gain);
            }

            return rgb.Interpolate(filtered, rangeFactor);
        }

        public static Rgb GammaBlue(Rgb rgb, params object[] filterParams)
        {
            var filtered = new Rgb(rgb);
            double rangeFactor;
            (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(rgb, filterParams);
            if (filterParams.Any() && FilterUtils.IsNumberOrString(filterParams[0])) {
                double gain = FilterUtils.TryParseDouble(filterParams[0]) ?? 1.0;
                filtered.Blue = ColorMath.Gamma(rgb.Blue, gain);
            }

            return rgb.Interpolate(filtered, rangeFactor);
        }

        public static Hsl SaturationGamma(Hsl hsl, params object[] filterParams)
        {
            var filtered = new Hsl(hsl);
            double rangeFactor;
            (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(hsl, filterParams);
            if (filterParams.Any() && FilterUtils.IsNumberOrString(filterParams[0])) {
                double gamma = FilterUtils.TryParseDouble(filterParams[0]) ?? 1.0;
                filtered.Saturation = ColorMath.Gamma(hsl.Saturation, gamma);
            }
            
            return hsl.Interpolate(filtered, rangeFactor);
        }

        public static Hsl LightnessGamma(Hsl hsl, params object[] filterParams)
        {
            var filtered = new Hsl(hsl);
            double rangeFactor;
            (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(hsl, filterParams);
            if (filterParams.Any() && FilterUtils.IsNumberOrString(filterParams[0])) {
                double gamma = FilterUtils.TryParseDouble(filterParams[0]) ?? 1.0;
                filtered.Lightness = ColorMath.Gamma(hsl.Lightness, gamma);
            }

            return hsl.Interpolate(filtered, rangeFactor);
        }

        #endregion

        #region "Contrast"

        public static Rgb Contrast(Rgb rgb, params object[] filterParams)
        {
            var filtered = new Rgb(rgb);
            double rangeFactor;
            (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(rgb, filterParams);
            
            if (filterParams.Any() && FilterUtils.IsNumberOrString(filterParams[0])) {
                double strength = FilterUtils.TryParseDouble(filterParams[0]) ?? 0.0;
                if (filterParams.Length >= 2 && FilterUtils.IsNumberOrString(filterParams[1])) {
                    double midpoint = FilterUtils.TryParseDouble(filterParams[1]) ?? 0.5;
                    filtered.Red = ColorMath.SSpline(rgb.Red, strength, midpoint);
                    filtered.Green = ColorMath.SSpline(rgb.Green, strength, midpoint);
                    filtered.Blue = ColorMath.SSpline(rgb.Blue, strength, midpoint);
                } else {
                    filtered.Red = ColorMath.SSpline(rgb.Red, strength);
                    filtered.Green = ColorMath.SSpline(rgb.Green, strength);
                    filtered.Blue = ColorMath.SSpline(rgb.Blue, strength);
                }
            }

            return rgb.Interpolate(filtered, rangeFactor);
        }

        public static Hsl SaturationContrast(Hsl hsl, params object[] filterParams)
        {
            var filtered = new Hsl(hsl);
            double rangeFactor;
            (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(hsl, filterParams);
            
            if (filterParams.Any() && FilterUtils.IsNumberOrString(filterParams[0])) {
                double strength = FilterUtils.TryParseDouble(filterParams[0]) ?? 0.0;
                if (filterParams.Length >= 2 && FilterUtils.IsNumberOrString(filterParams[1])) {
                    double midpoint = FilterUtils.TryParseDouble(filterParams[1]) ?? 0.5;
                    filtered.Saturation = ColorMath.SSpline(hsl.Saturation, strength, midpoint);
                } else {
                    filtered.Saturation = ColorMath.SSpline(hsl.Saturation, strength);
                }
            }

            return hsl.Interpolate(filtered, rangeFactor);
        }

        public static Hsl LightnessContrast(Hsl hsl, params object[] filterParams)
        {
            var filtered = new Hsl(hsl);
            double rangeFactor;
            (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(hsl, filterParams);
            
            if (filterParams.Any() && FilterUtils.IsNumberOrString(filterParams[0])) {
                double strength = FilterUtils.TryParseDouble(filterParams[0]) ?? 0.0;
                if (filterParams.Length >= 2 && FilterUtils.IsNumberOrString(filterParams[1])) {
                    double midpoint = FilterUtils.TryParseDouble(filterParams[1]) ?? 0.5;
                    filtered.Lightness = ColorMath.SSpline(hsl.Lightness, strength, midpoint);
                } else {
                    filtered.Lightness = ColorMath.SSpline(hsl.Lightness, strength);
                }
            }
            
            return hsl.Interpolate(filtered, rangeFactor);
        }

        #endregion

        #region "Levels based filters"

        public static Rgb Levels(Rgb rgb, params object[] filterParams)
        {
            var result = new Rgb(rgb);
            double rangeFactor;
            (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(rgb, filterParams);
            result.Red = FilterUtils.CalcLevels(rgb.Red, rangeFactor, filterParams);
            result.Green = FilterUtils.CalcLevels(rgb.Green, rangeFactor, filterParams);
            result.Blue = FilterUtils.CalcLevels(rgb.Blue, rangeFactor, filterParams);
            return result;
        }

        public static Rgb LevelsRed(Rgb rgb, params object[] filterParams)
        {
            var result = new Rgb(rgb);
            double rangeFactor;
            (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(rgb, filterParams);
            result.Red = FilterUtils.CalcLevels(rgb.Red, rangeFactor, filterParams);
            return result;
        }

        public static Rgb LevelsGreen(Rgb rgb, params object[] filterParams)
        {
            var result = new Rgb(rgb);
            double rangeFactor;
            (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(rgb, filterParams);
            result.Green = FilterUtils.CalcLevels(rgb.Green, rangeFactor, filterParams);
            return result;
        }

        public static Rgb LevelsBlue(Rgb rgb, params object[] filterParams)
        {
            var result = new Rgb(rgb);
            double rangeFactor;
            (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(rgb, filterParams);
            result.Blue = FilterUtils.CalcLevels(rgb.Blue, rangeFactor, filterParams);
            return result;
        }

        public static Hsl LightnessLevels(Hsl hsl, params object[] filterParams)
        {
            var result = new Hsl(hsl);
            double rangeFactor;
            (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(hsl, filterParams);
            result.Lightness = FilterUtils.CalcLevels(hsl.Lightness, rangeFactor, filterParams);
            return result;
        }

        public static Hsl SaturationLevels(Hsl hsl, params object[] filterParams)
        {
            var result = new Hsl(hsl);
            double rangeFactor;
            (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(hsl, filterParams);
            result.Saturation = FilterUtils.CalcLevels(hsl.Saturation, rangeFactor, filterParams);
            return result;
        }

        #endregion
    }
}