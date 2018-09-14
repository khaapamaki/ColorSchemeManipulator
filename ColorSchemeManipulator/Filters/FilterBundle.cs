using System.Collections.Generic;
using System.Linq;
using ColorSchemeManipulator.CLI;
using ColorSchemeManipulator.Colors;
using ColorSchemeManipulator.Common;

namespace ColorSchemeManipulator.Filters
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

            CliArgs.Register(new List<string> {"-h", "--hue"}, ShiftHslHue, 1,
                desc: "Hue shift. Accepts single parameter as degrees -360..360");
            CliArgs.Register(new List<string> {"-s", "--saturation"}, GainHslSaturation, 1,
                desc: "HSL saturation multiplier. Accepts single parameter 0..x");
            CliArgs.Register(new List<string> {"-g", "--gain"}, GainRgb, 1,
                desc: "RGB multiplier. Accepts single parameter 0..x");
            CliArgs.Register(new List<string> {"-l", "--lightness"}, GainLightness, 1,
                desc: "HSL lightness multiplier. Accepts single parameter 0..x");
            CliArgs.Register(new List<string> {"-v", "--value"}, GainValue, 1,
                desc: "HSV value multiplier. Accepts single parameter 0..x");
            CliArgs.Register(new List<string> {"-S", "--hsv-saturation"}, GainHsvSaturation, 1,
                desc: "HSV saturation multiplier. Accepts single parameter 0..x");

            CliArgs.Register(new List<string> {"-c", "--contrast"}, ContrastRgb, 1, 2);
            CliArgs.Register(new List<string> {"-cl", "--contrast-lightness"}, ContrastLightness, 1, 2);
            CliArgs.Register(new List<string> {"-cv", "--contrast-value"}, ContrastValue, 1, 2);
            CliArgs.Register(new List<string> {"-cs", "--contrast-saturation"}, ContrastHslSaturation, 1, 2);
            CliArgs.Register(new List<string> {"-cS", "--contrast-hsv-saturation"}, ContrastHsvSaturation, 1, 2);

            CliArgs.Register(new List<string> {"-ga", "--gamma"}, GammaRgb, 1);
            CliArgs.Register(new List<string> {"-gar", "--gamma-red"}, GammaRed, 1);
            CliArgs.Register(new List<string> {"-gag", "--gamma-green"}, GammaGreen, 1);
            CliArgs.Register(new List<string> {"-gab", "--gamma-blue"}, GammaBlue, 1);
            CliArgs.Register(new List<string> {"-gal", "--gamma-lightness"}, GammaLightness, 1);
            CliArgs.Register(new List<string> {"-gav", "--gamma-value"}, GammaValue, 1);
            CliArgs.Register(new List<string> {"-gas", "--gamma-saturation"}, GammaHslSaturation, 1);
            CliArgs.Register(new List<string> {"-gaS", "--gamma-hsv-saturation"}, GammaHsvSaturation, 1);

            CliArgs.Register(new List<string> {"-le", "--levels"}, LevelsRgb, 5);
            CliArgs.Register(new List<string> {"-ler", "--levels-red"}, LevelsRed, 5);
            CliArgs.Register(new List<string> {"-leg", "--levels-green"}, LevelsGreen, 5);
            CliArgs.Register(new List<string> {"-leb", "--levels-blue"}, LevelsBlue, 5);
            CliArgs.Register(new List<string> {"-lel", "--levels-lightness"}, LevelsLightness, 5);
            CliArgs.Register(new List<string> {"-lev", "--levels-value"}, LevelsValue, 5);
            CliArgs.Register(new List<string> {"-les", "--levels-saturation"}, LevelsHslSaturation, 5);
            CliArgs.Register(new List<string> {"-leS", "--levels-hsv-saturation"}, LevelsHsvSaturation, 5);

            CliArgs.Register(new List<string> {"-i", "--invert-rgb"}, InvertRgb, 0);
            CliArgs.Register(new List<string> {"-ib", "--invert-brightness"}, InvertPerceivedBrightness, 0, 0,
                desc: "Inverts perceived brightness - experimental");
            CliArgs.Register(new List<string> {"-il", "--invert-lightness"}, InvertLightness, 0);
            CliArgs.Register(new List<string> {"-iv", "--invert-value"}, InvertValue, 0);
            CliArgs.Register(new List<string> {"-ilv", "--invert-lightness-value"}, InvertMixedLightnessAndValue, 0, 1,
                desc: "Inverts colors using both lightness and value, by mixing the result - experimental");
            CliArgs.Register(new List<string> {"-gsb", "--grayscale-brightness"}, BrightnessToGrayScale, 0, 0,
                desc: "Converts to gray scale based on perceived brightness");
        }

        #region "Invert"

        public static Rgb InvertRgb(Rgb rgb, params object[] filterParams)
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

        public static Hsl InvertLightness(Hsl hsl, params object[] filterParams)
        {
            double rangeFactor;
            (rangeFactor, _) = FilterUtils.GetRangeFactorAndRemainingParams(hsl, filterParams);
            Hsl filtered = new Hsl(hsl);
            filtered.Lightness = ColorMath.Invert(hsl.Lightness);

            return hsl.Interpolate(filtered, rangeFactor);
        }

        public static Hsl InvertMixedLightnessAndValue(Hsl hsl, params object[] filterParams)
        {
            double rangeFactor;
            (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(hsl, filterParams);
            double mix = 0.333333;
            if (filterParams.Any() && FilterUtils.IsNumberOrString(filterParams[0])) {
                mix = (FilterUtils.TryParseDouble(filterParams[0]) ?? 0.5).LimitLow(0.0);
            }

            Hsl hslFiltered = new Hsl(hsl);
            Hsv hsv = new Hsv(hsl.ToHsv());
            Hsv hsvFiltered = new Hsv(hsv);
            hslFiltered.Lightness = ColorMath.Invert(hsl.Lightness);
            hsvFiltered.Value = ColorMath.Invert(hsv.Value);
            hsl = hsl.Interpolate(hslFiltered, rangeFactor);
            hsv = hsv.Interpolate(hsvFiltered, rangeFactor);

            return hsl.Interpolate(hsv.ToHsl(), mix);
        }


        public static Hsv InvertValue(Hsv hsv, params object[] filterParams)
        {
            double rangeFactor;
            (rangeFactor, _) = FilterUtils.GetRangeFactorAndRemainingParams(hsv, filterParams);
            Hsv filtered = new Hsv(hsv);
            filtered.Value = ColorMath.Invert(hsv.Value);

            return hsv.Interpolate(filtered, rangeFactor);
        }

        public static Rgb InvertPerceivedBrightness(Rgb rgb, params object[] filterParams)
        {
            double rangeFactor;
            (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(rgb, filterParams);
            var hsl = rgb.ToHsl();
            var brightness = ColorMath.RgbPerceivedBrightness(rgb.Red, rgb.Green, rgb.Blue);
            var targetBrightness = (1 - brightness).Clamp(0, 1);

            // using brightness as lightness is not accurate but we can correct this later
            // how ever it seems that completely correct value produces worse outcome
            // so we may use something in between
            Hsl invertedHsl = new Hsl(hsl.Hue, hsl.Saturation, targetBrightness, hsl.Alpha);

            var invertedRgb = invertedHsl.ToRgb();
            var newBrightness =
                ColorMath.RgbPerceivedBrightness(invertedRgb.Red, invertedRgb.Green, invertedRgb.Blue);

            //var delta = targetBrightness / newBrightness - 1;
            var corr = targetBrightness / newBrightness + (targetBrightness / newBrightness - 1) / 4;


            var corrected = new Rgb(invertedRgb.Red * corr, invertedRgb.Green * corr,
                invertedRgb.Blue * corr, rgb.Alpha);

            // var correctedBrightness = ColorMath.RgbPerceivedBrightness(corrected.Red,
            //     corrected.Green, corrected.Blue);

            return rgb.Interpolate(corrected, rangeFactor);
        }

        #endregion

        #region "Gain"

        public static Hsl GainHslSaturation(Hsl hsl, params object[] filterParams)
        {
            var filtered = new Hsl(hsl);
            double rangeFactor;
            (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(hsl, filterParams);
            if (filterParams.Any() && FilterUtils.IsNumberOrString(filterParams[0])) {
                double gain = (FilterUtils.TryParseDouble(filterParams[0]) ?? 1.0).LimitLow(0.0);
                filtered.Saturation = filtered.Saturation * gain;
            }

            return hsl.Interpolate(filtered, rangeFactor);
        }

        public static Hsv GainHsvSaturation(Hsv hsv, params object[] filterParams)
        {
            var filtered = new Hsv(hsv);
            double rangeFactor;
            (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(hsv, filterParams);
            if (filterParams.Any() && FilterUtils.IsNumberOrString(filterParams[0])) {
                double gain = (FilterUtils.TryParseDouble(filterParams[0]) ?? 1.0).LimitLow(0.0);
                filtered.Saturation = filtered.Saturation * gain;
            }

            return hsv.Interpolate(filtered, rangeFactor);
        }

        public static Rgb GainRgb(Rgb rgb, params object[] filterParams)
        {
            var filtered = new Rgb(rgb);
            double rangeFactor;
            (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(rgb, filterParams);
            if (filterParams.Any() && FilterUtils.IsNumberOrString(filterParams[0])) {
                double gain = (FilterUtils.TryParseDouble(filterParams[0]) ?? 1.0).LimitLow(0.0);
                filtered.Red = ColorMath.Gain(rgb.Red, gain);
                filtered.Green = ColorMath.Gain(rgb.Green, gain);
                filtered.Blue = ColorMath.Gain(rgb.Blue, gain);
            }

            return rgb.Interpolate(filtered, rangeFactor);
        }

        public static Hsl GainLightness(Hsl hsl, params object[] filterParams)
        {
            var filtered = new Hsl(hsl);
            double rangeFactor;
            (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(hsl, filterParams);
            if (filterParams.Any() && FilterUtils.IsNumberOrString(filterParams[0])) {
                double gain = (FilterUtils.TryParseDouble(filterParams[0]) ?? 1.0).LimitLow(0.0);
                filtered.Lightness = filtered.Lightness * gain;
            }

            return hsl.Interpolate(filtered, rangeFactor);
        }

        public static Hsv GainValue(Hsv hsv, params object[] filterParams)
        {
            var filtered = new Hsv(hsv);
            double rangeFactor;
            (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(hsv, filterParams);
            if (filterParams.Any() && FilterUtils.IsNumberOrString(filterParams[0])) {
                double gain = (FilterUtils.TryParseDouble(filterParams[0]) ?? 1.0).LimitLow(0.0);
                filtered.Value = filtered.Value * gain;
            }

            return hsv.Interpolate(filtered, rangeFactor);
        }

        #endregion

        #region "Gamma"

        public static Rgb GammaRgb(Rgb rgb, params object[] filterParams)
        {
            var filtered = new Rgb(rgb);
            double rangeFactor;
            (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(rgb, filterParams);
            if (filterParams.Any() && FilterUtils.IsNumberOrString(filterParams[0])) {
                double gamma = FilterUtils.TryParseDouble(filterParams[0]) ?? 1.0;
                filtered.Red = ColorMath.Gamma(rgb.Red, gamma);
                filtered.Green = ColorMath.Gamma(rgb.Green, gamma);
                filtered.Blue = ColorMath.Gamma(rgb.Blue, gamma);
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

        public static Hsl GammaHslSaturation(Hsl hsl, params object[] filterParams)
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

        public static Hsv GammaHsvSaturation(Hsv hsv, params object[] filterParams)
        {
            var filtered = new Hsv(hsv);
            double rangeFactor;
            (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(hsv, filterParams);
            if (filterParams.Any() && FilterUtils.IsNumberOrString(filterParams[0])) {
                double gamma = FilterUtils.TryParseDouble(filterParams[0]) ?? 1.0;
                filtered.Saturation = ColorMath.Gamma(hsv.Saturation, gamma);
            }

            return hsv.Interpolate(filtered, rangeFactor);
        }

        public static Hsl GammaLightness(Hsl hsl, params object[] filterParams)
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

        public static Hsv GammaValue(Hsv hsv, params object[] filterParams)
        {
            var filtered = new Hsv(hsv);
            double rangeFactor;
            (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(hsv, filterParams);
            if (filterParams.Any() && FilterUtils.IsNumberOrString(filterParams[0])) {
                double gamma = FilterUtils.TryParseDouble(filterParams[0]) ?? 1.0;
                filtered.Value = ColorMath.Gamma(hsv.Value, gamma);
            }

            return hsv.Interpolate(filtered, rangeFactor);
        }

        #endregion

        #region "Contrast"

        public static Rgb ContrastRgb(Rgb rgb, params object[] filterParams)
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

        public static Hsl ContrastHslSaturation(Hsl hsl, params object[] filterParams)
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

        public static Hsv ContrastHsvSaturation(Hsv hsv, params object[] filterParams)
        {
            var filtered = new Hsv(hsv);
            double rangeFactor;
            (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(hsv, filterParams);

            if (filterParams.Any() && FilterUtils.IsNumberOrString(filterParams[0])) {
                double strength = FilterUtils.TryParseDouble(filterParams[0]) ?? 0.0;
                if (filterParams.Length >= 2 && FilterUtils.IsNumberOrString(filterParams[1])) {
                    double midpoint = FilterUtils.TryParseDouble(filterParams[1]) ?? 0.5;
                    filtered.Saturation = ColorMath.SSpline(hsv.Saturation, strength, midpoint);
                } else {
                    filtered.Saturation = ColorMath.SSpline(hsv.Saturation, strength);
                }
            }

            return hsv.Interpolate(filtered, rangeFactor);
        }

        public static Hsl ContrastLightness(Hsl hsl, params object[] filterParams)
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

        public static Hsv ContrastValue(Hsv hsv, params object[] filterParams)
        {
            var filtered = new Hsv(hsv);
            double rangeFactor;
            (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(hsv, filterParams);

            if (filterParams.Any() && FilterUtils.IsNumberOrString(filterParams[0])) {
                double strength = FilterUtils.TryParseDouble(filterParams[0]) ?? 0.0;
                if (filterParams.Length >= 2 && FilterUtils.IsNumberOrString(filterParams[1])) {
                    double midpoint = FilterUtils.TryParseDouble(filterParams[1]) ?? 0.5;
                    filtered.Value = ColorMath.SSpline(hsv.Value, strength, midpoint);
                } else {
                    filtered.Value = ColorMath.SSpline(hsv.Value, strength);
                }
            }

            return hsv.Interpolate(filtered, rangeFactor);
        }

        #endregion

        #region Hue

        public static Hsl ShiftHslHue(Hsl hsl, params object[] filterParams)
        {
            var filtered = new Hsl(hsl);
            double rangeFactor;
            (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(hsl, filterParams);
            if (filterParams.Any() && FilterUtils.IsNumberOrString(filterParams[0])) {
                double hueShift = FilterUtils.TryParseDouble(filterParams[0]) ?? 0.0;
                filtered.Hue = hsl.Hue + hueShift;
            }

            return hsl.Interpolate(filtered, rangeFactor);
        }

        public static Hsv ShiftHsvHue(Hsv hsv, params object[] filterParams)
        {
            var filtered = new Hsv(hsv);
            double rangeFactor;
            (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(hsv, filterParams);
            if (filterParams.Any() && FilterUtils.IsNumberOrString(filterParams[0])) {
                double hueShift = FilterUtils.TryParseDouble(filterParams[0]) ?? 0.0;
                filtered.Hue = hsv.Hue + hueShift;
            }

            return hsv.Interpolate(filtered, rangeFactor);
        }

        #endregion

        #region "Levels"

        public static Rgb LevelsRgb(Rgb rgb, params object[] filterParams)
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

        public static Hsl LevelsLightness(Hsl hsl, params object[] filterParams)
        {
            var result = new Hsl(hsl);
            double rangeFactor;
            (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(hsl, filterParams);
            result.Lightness = FilterUtils.CalcLevels(hsl.Lightness, rangeFactor, filterParams);
            return result;
        }

        public static Hsv LevelsValue(Hsv hsv, params object[] filterParams)
        {
            var result = new Hsv(hsv);
            double rangeFactor;
            (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(hsv, filterParams);
            result.Value = FilterUtils.CalcLevels(hsv.Value, rangeFactor, filterParams);
            return result;
        }


        public static Hsl LevelsHslSaturation(Hsl hsl, params object[] filterParams)
        {
            var result = new Hsl(hsl);
            double rangeFactor;
            (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(hsl, filterParams);
            result.Saturation = FilterUtils.CalcLevels(hsl.Saturation, rangeFactor, filterParams);
            return result;
        }

        public static Hsv LevelsHsvSaturation(Hsv hsv, params object[] filterParams)
        {
            var result = new Hsv(hsv);
            double rangeFactor;
            (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(hsv, filterParams);
            result.Saturation = FilterUtils.CalcLevels(hsv.Saturation, rangeFactor, filterParams);
            return result;
        }

        #endregion

        #region "Misc"

        public static Rgb BrightnessToGrayScale(Rgb rgb, params object[] filterParams)
        {
            var filtered = new Rgb(rgb);
            double rangeFactor;
            (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(rgb, filterParams);
            var br = ColorMath.RgbPerceivedBrightness(rgb.Red, rgb.Green, rgb.Blue);
            filtered.Red = br;
            filtered.Green = br;
            filtered.Blue = br;
            return rgb.Interpolate(filtered, rangeFactor);
        }

        #endregion
    }
}