using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

            // CliArgs.Register(new List<string> {"-h", "--hue"}, ShiftHslHue, 1,
            //     desc: "Hue shift. Accepts single parameter as degrees -360..360");
            // CliArgs.Register(new List<string> {"-s", "--saturation"}, GainHslSaturation, 1,
            //     desc: "HSL saturation multiplier. Accepts single parameter 0..x");
            // CliArgs.Register(new List<string> {"-g", "--gain"}, GainRgb, 1,
            //     desc: "RGB multiplier. Accepts single parameter 0..x");
            // CliArgs.Register(new List<string> {"-l", "--lightness"}, GainLightness, 1,
            //     desc: "HSL lightness multiplier. Accepts single parameter 0..x");
            // CliArgs.Register(new List<string> {"-v", "--value"}, GainValue, 1,
            //     desc: "HSV value multiplier. Accepts single parameter 0..x");
            // CliArgs.Register(new List<string> {"-S", "--hsv-saturation"}, GainHsvSaturation, 1,
            //     desc: "HSV saturation multiplier. Accepts single parameter 0..x");
            //
            // CliArgs.Register(new List<string> {"-c", "--contrast"}, ContrastRgb, 1, 2);
            // CliArgs.Register(new List<string> {"-cl", "--contrast-lightness"}, ContrastLightness, 1, 2);
            // CliArgs.Register(new List<string> {"-cv", "--contrast-value"}, ContrastValue, 1, 2);
            // CliArgs.Register(new List<string> {"-cs", "--contrast-saturation"}, ContrastHslSaturation, 1, 2);
            // CliArgs.Register(new List<string> {"-cS", "--contrast-hsv-saturation"}, ContrastHsvSaturation, 1, 2);
            //
            // CliArgs.Register(new List<string> {"-ga", "--gamma"}, GammaRgb, 1);
            // CliArgs.Register(new List<string> {"-gar", "--gamma-red"}, GammaRed, 1);
            // CliArgs.Register(new List<string> {"-gag", "--gamma-green"}, GammaGreen, 1);
            // CliArgs.Register(new List<string> {"-gab", "--gamma-blue"}, GammaBlue, 1);
            // CliArgs.Register(new List<string> {"-gal", "--gamma-lightness"}, GammaLightness, 1);
            // CliArgs.Register(new List<string> {"-gav", "--gamma-value"}, GammaValue, 1);
            // CliArgs.Register(new List<string> {"-gas", "--gamma-saturation"}, GammaHslSaturation, 1);
            // CliArgs.Register(new List<string> {"-gaS", "--gamma-hsv-saturation"}, GammaHsvSaturation, 1);
            //
            // CliArgs.Register(new List<string> {"-le", "--levels"}, LevelsRgb, 5);
            // CliArgs.Register(new List<string> {"-ler", "--levels-red"}, LevelsRed, 5);
            // CliArgs.Register(new List<string> {"-leg", "--levels-green"}, LevelsGreen, 5);
            // CliArgs.Register(new List<string> {"-leb", "--levels-blue"}, LevelsBlue, 5);
            // CliArgs.Register(new List<string> {"-lel", "--levels-lightness"}, LevelsLightness, 5);
            // CliArgs.Register(new List<string> {"-lev", "--levels-value"}, LevelsValue, 5);
            // CliArgs.Register(new List<string> {"-les", "--levels-saturation"}, LevelsHslSaturation, 5);
            // CliArgs.Register(new List<string> {"-leS", "--levels-hsv-saturation"}, LevelsHsvSaturation, 5);

            CliArgs.Register(new List<string> {"-i", "--invert-rgb"}, InvertRgb, 0);
            CliArgs.Register(new List<string> {"--bypass"}, ByBass, 0);
            // CliArgs.Register(new List<string> {"-il", "--invert-lightness"}, InvertLightness, 0);
            // CliArgs.Register(new List<string> {"-iv", "--invert-value"}, InvertValue, 0);
            //
            // CliArgs.Register(new List<string> {"-gsb", "--grayscale-brightness"}, BrightnessToGrayScale, 0, 0,
            //     desc: "Converts to gray scale based on perceived brightness");

            GetInstance()._isRegistered = true;
        }

        public static IEnumerable<Color> ByBass(IEnumerable<Color> colors, params object[] filterParams)
        {
            ColorRange range;
            (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
            foreach (var color in colors) {
                if (filterParams.Any()) {
                     (double h, double s, double l) = color.GetHslComponents();
                    var newColor = Color.FromHsl(h, s,l , color.Alpha);
                    yield return newColor;
                } else {

                    yield return color;
                }
            }
        }


        #region "Invert"

        public static IEnumerable<Color> InvertRgb(IEnumerable<Color> colors, params object[] filterParams)
        {
            ColorRange range;
            (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(range, color);
                var inverted = Color.FromRgb(
                    ColorMath.Invert(color.Red),
                    ColorMath.Invert(color.Green),
                    ColorMath.Invert(color.Blue),
                    color.Alpha);
                yield return color.InterpolateWith(inverted, rangeFactor);
            }
        }

        /*
                public static IEnumerable<Color> InvertLightness(IEnumerable<Color> colors, params object[] filterParams)
                {
                    ColorRange range;
                    (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
                    foreach (var color in colors) {
                        var hsl = color.ToHsl();
                        var rangeFactor = FilterUtils.GetRangeFactor(range, hsl);
        
                        var filtered = new Hsl(hsl) {Lightness = ColorMath.Invert(hsl.Lightness)};
        
                        yield return hsl.Interpolate(filtered, rangeFactor);
                    }
                }
        
                public static IEnumerable<Color> InvertValue(IEnumerable<Color> colors, params object[] filterParams)
                {
                    ColorRange range;
                    (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
                    foreach (var color in colors) {
                        var hsv = color.ToHsv();
                        var rangeFactor = FilterUtils.GetRangeFactor(range, hsv);
                        var filtered = color.ToHsv();
        
                        filtered.Value = ColorMath.Invert(hsv.Value);
        
                        yield return hsv.Interpolate(filtered, rangeFactor);
                    }
                }
        
                #endregion
        
                #region "Gain"
        
                public static IEnumerable<Color> GainHslSaturation(IEnumerable<Color> colors, params object[] filterParams)
                {
                    ColorRange range;
                    (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
                    foreach (var color in colors) {
                        var hsl = color.ToHsl();
                        var rangeFactor = FilterUtils.GetRangeFactor(range, hsl);
                        var filtered = color.ToHsl();
        
                        if (filterParams.Any()) {
                            double gain = (FilterUtils.TryParseDouble(filterParams[0]) ?? 1.0).LimitLow(0.0);
                            filtered.Saturation = filtered.Saturation * gain;
                        }
        
                        yield return hsl.Interpolate(filtered, rangeFactor);
                    }
                }
        
                public static IEnumerable<Color> GainHsvSaturation(IEnumerable<Color> colors, params object[] filterParams)
                {
                    ColorRange range;
                    (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
                    foreach (var color in colors) {
                        var hsv = color.ToHsv();
                        var rangeFactor = FilterUtils.GetRangeFactor(range, hsv);
                        var filtered = color.ToHsv();
        
                        if (filterParams.Any()) {
                            double gain = (FilterUtils.TryParseDouble(filterParams[0]) ?? 1.0).LimitLow(0.0);
                            filtered.Saturation = filtered.Saturation * gain;
                        }
        
                        yield return hsv.Interpolate(filtered, rangeFactor);
                    }
                }
        
                public static IEnumerable<Color> GainRgb(IEnumerable<Color> colors, params object[] filterParams)
                {
                    ColorRange range;
                    (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
                    foreach (var color in colors) {
                        var rgb = color.ToRgb();
                        var rangeFactor = FilterUtils.GetRangeFactor(range, rgb);
                        var filtered = color.ToRgb();
        
                        if (filterParams.Any()) {
                            double gain = (FilterUtils.TryParseDouble(filterParams[0]) ?? 1.0).LimitLow(0.0);
                            filtered.Red8 = ColorMath.Gain(rgb.Red8, gain);
                            filtered.Green8 = ColorMath.Gain(rgb.Green8, gain);
                            filtered.Blue8 = ColorMath.Gain(rgb.Blue8, gain);
                        }
        
                        yield return rgb.Interpolate(filtered, rangeFactor);
                    }
                }
        
                public static IEnumerable<Color> GainLightness(IEnumerable<Color> colors, params object[] filterParams)
                {
                    ColorRange range;
                    (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
                    foreach (var color in colors) {
                        var hsl = color.ToHsl();
                        var rangeFactor = FilterUtils.GetRangeFactor(range, hsl);
                        var filtered = color.ToHsl();
        
                        if (filterParams.Any()) {
                            double gain = (FilterUtils.TryParseDouble(filterParams[0]) ?? 1.0).LimitLow(0.0);
                            filtered.Lightness = filtered.Lightness * gain;
                        }
        
                        yield return hsl.Interpolate(filtered, rangeFactor);
                    }
                }
        
                public static IEnumerable<Color> GainValue(IEnumerable<Color> colors, params object[] filterParams)
                {
                    ColorRange range;
                    (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
                    foreach (var color in colors) {
                        var hsv = color.ToHsv();
                        var rangeFactor = FilterUtils.GetRangeFactor(range, hsv);
                        var filtered = color.ToHsv();
        
                        if (filterParams.Any()) {
                            double gain = (FilterUtils.TryParseDouble(filterParams[0]) ?? 1.0).LimitLow(0.0);
                            filtered.Value = filtered.Value * gain;
                        }
        
                        yield return hsv.Interpolate(filtered, rangeFactor);
                    }
                }
        
                #endregion
        
                #region "Gamma"
        
                public static IEnumerable<Color> GammaRgb(IEnumerable<Color> colors, params object[] filterParams)
                {
                    ColorRange range;
                    (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
                    foreach (var color in colors) {
                        var rgb = color.ToRgb();
                        var rangeFactor = FilterUtils.GetRangeFactor(range, rgb);
                        var filtered = color.ToRgb();
        
                        if (filterParams.Any()) {
                            double gamma = FilterUtils.TryParseDouble(filterParams[0]) ?? 1.0;
                            filtered.Red8 = ColorMath.Gamma(rgb.Red8, gamma);
                            filtered.Green8 = ColorMath.Gamma(rgb.Green8, gamma);
                            filtered.Blue8 = ColorMath.Gamma(rgb.Blue8, gamma);
                        }
        
                        yield return rgb.Interpolate(filtered, rangeFactor);
                    }
                }
        
                public static IEnumerable<Color> GammaRed(IEnumerable<Color> colors, params object[] filterParams)
                {
                    ColorRange range;
                    (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
                    foreach (var color in colors) {
                        var rgb = color.ToRgb();
                        var rangeFactor = FilterUtils.GetRangeFactor(range, rgb);
                        var filtered = color.ToRgb();
        
                        if (filterParams.Any()) {
                            double gain = FilterUtils.TryParseDouble(filterParams[0]) ?? 1.0;
                            filtered.Red8 = ColorMath.Gamma(rgb.Red8, gain);
                        }
        
                        yield return rgb.Interpolate(filtered, rangeFactor);
                    }
                }
        
                public static IEnumerable<Color> GammaGreen(IEnumerable<Color> colors, params object[] filterParams)
                {
                    ColorRange range;
                    (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
                    foreach (var color in colors) {
                        var rgb = color.ToRgb();
                        var rangeFactor = FilterUtils.GetRangeFactor(range, rgb);
                        var filtered = color.ToRgb();
        
        
                        if (filterParams.Any()) {
                            double gain = FilterUtils.TryParseDouble(filterParams[0]) ?? 1.0;
                            filtered.Green8 = ColorMath.Gamma(rgb.Green8, gain);
                        }
        
                        yield return rgb.Interpolate(filtered, rangeFactor);
                    }
                }
        
                public static IEnumerable<Color> GammaBlue(IEnumerable<Color> colors, params object[] filterParams)
                {
                    ColorRange range;
                    (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
                    foreach (var color in colors) {
                        var rgb = color.ToRgb();
                        var rangeFactor = FilterUtils.GetRangeFactor(range, rgb);
                        var filtered = color.ToRgb();
        
                        if (filterParams.Any()) {
                            double gain = FilterUtils.TryParseDouble(filterParams[0]) ?? 1.0;
                            filtered.Blue8 = ColorMath.Gamma(rgb.Blue8, gain);
                        }
        
                        yield return rgb.Interpolate(filtered, rangeFactor);
                    }
                }
        
                public static IEnumerable<Color> GammaHslSaturation(IEnumerable<Color> colors, params object[] filterParams)
                {
                    ColorRange range;
                    (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
                    foreach (var color in colors) {
                        var hsl = color.ToHsl();
                        var rangeFactor = FilterUtils.GetRangeFactor(range, hsl);
                        var filtered = color.ToHsl();
        
                        if (filterParams.Any()) {
                            double gamma = FilterUtils.TryParseDouble(filterParams[0]) ?? 1.0;
                            filtered.Saturation = ColorMath.Gamma(hsl.Saturation, gamma);
                        }
        
                        yield return hsl.Interpolate(filtered, rangeFactor);
                    }
                }
        
                public static IEnumerable<Color> GammaHsvSaturation(IEnumerable<Color> colors, params object[] filterParams)
                {
                    ColorRange range;
                    (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
                    foreach (var color in colors) {
                        var hsv = color.ToHsv();
                        var rangeFactor = FilterUtils.GetRangeFactor(range, hsv);
                        var filtered = color.ToHsv();
        
                        if (filterParams.Any()) {
                            double gamma = FilterUtils.TryParseDouble(filterParams[0]) ?? 1.0;
                            filtered.Saturation = ColorMath.Gamma(hsv.Saturation, gamma);
                        }
        
                        yield return hsv.Interpolate(filtered, rangeFactor);
                    }
                }
        
                public static IEnumerable<Color> GammaLightness(IEnumerable<Color> colors, params object[] filterParams)
                {
                    ColorRange range;
                    (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
                    foreach (var color in colors) {
                        var hsl = color.ToHsl();
                        var rangeFactor = FilterUtils.GetRangeFactor(range, hsl);
                        var filtered = color.ToHsl();
        
                        if (filterParams.Any()) {
                            double gamma = FilterUtils.TryParseDouble(filterParams[0]) ?? 1.0;
                            filtered.Lightness = ColorMath.Gamma(hsl.Lightness, gamma);
                        }
        
                        yield return hsl.Interpolate(filtered, rangeFactor);
                    }
                }
        
                public static IEnumerable<Color> GammaValue(IEnumerable<Color> colors, params object[] filterParams)
                {
                    ColorRange range;
                    (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
                    foreach (var color in colors) {
                        var hsv = color.ToHsv();
                        var rangeFactor = FilterUtils.GetRangeFactor(range, hsv);
                        var filtered = color.ToHsv();
        
                        if (filterParams.Any()) {
                            double gamma = FilterUtils.TryParseDouble(filterParams[0]) ?? 1.0;
                            filtered.Value = ColorMath.Gamma(hsv.Value, gamma);
                        }
        
                        yield return hsv.Interpolate(filtered, rangeFactor);
                    }
                }
        
                #endregion
        
                #region "Contrast"
        
                public static IEnumerable<Color> ContrastRgb(IEnumerable<Color> colors, params object[] filterParams)
                {
                    ColorRange range;
                    (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
                    foreach (var color in colors) {
                        var rgb = color.ToRgb();
                        var rangeFactor = FilterUtils.GetRangeFactor(range, rgb);
                        var filtered = color.ToRgb();
        
                        if (filterParams.Any()) {
                            double strength = FilterUtils.TryParseDouble(filterParams[0]) ?? 0.0;
                            double midpoint = 0.5;
                            if (filterParams.Length >= 2) {
                                midpoint = FilterUtils.TryParseDouble(filterParams[1]) ?? 0.5;
                            }
        
                            filtered.Red8 = ColorMath.SSpline(rgb.Red8, strength, midpoint);
                            filtered.Green8 = ColorMath.SSpline(rgb.Green8, strength, midpoint);
                            filtered.Blue8 = ColorMath.SSpline(rgb.Blue8, strength, midpoint);
                        }
        
                        yield return rgb.Interpolate(filtered, rangeFactor);
                    }
                }
        
                public static IEnumerable<Color> ContrastHslSaturation(IEnumerable<Color> colors,
                    params object[] filterParams)
                {
                    ColorRange range;
                    (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
                    foreach (var color in colors) {
                        var hsl = color.ToHsl();
                        var rangeFactor = FilterUtils.GetRangeFactor(range, hsl);
                        var filtered = color.ToHsl();
        
                        if (filterParams.Any()) {
                            double strength = FilterUtils.TryParseDouble(filterParams[0]) ?? 0.0;
                            double midpoint = 0.5;
                            if (filterParams.Length >= 2) {
                                midpoint = FilterUtils.TryParseDouble(filterParams[1]) ?? 0.5;
                            }
        
                            filtered.Saturation = ColorMath.SSpline(hsl.Saturation, strength, midpoint);
                        }
        
                        yield return hsl.Interpolate(filtered, rangeFactor);
                    }
                }
        
                public static IEnumerable<Color> ContrastHsvSaturation(IEnumerable<Color> colors,
                    params object[] filterParams)
                {
                    ColorRange range;
                    (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
                    foreach (var color in colors) {
                        var hsv = color.ToHsv();
                        var rangeFactor = FilterUtils.GetRangeFactor(range, hsv);
                        var filtered = color.ToHsv();
        
                        if (filterParams.Any()) {
                            double strength = FilterUtils.TryParseDouble(filterParams[0]) ?? 0.0;
                            double midpoint = 0.5;
                            if (filterParams.Length >= 2) {
                                midpoint = FilterUtils.TryParseDouble(filterParams[1]) ?? 0.5;
                            }
        
                            filtered.Saturation = ColorMath.SSpline(hsv.Saturation, strength, midpoint);
                        }
        
                        yield return hsv.Interpolate(filtered, rangeFactor);
                    }
                }
        
                public static IEnumerable<Color> ContrastLightness(IEnumerable<Color> colors, params object[] filterParams)
                {
                    ColorRange range;
                    (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
                    foreach (var color in colors) {
                        var hsl = color.ToHsl();
                        var rangeFactor = FilterUtils.GetRangeFactor(range, hsl);
                        var filtered = color.ToHsl();
        
                        if (filterParams.Any()) {
                            double strength = FilterUtils.TryParseDouble(filterParams[0]) ?? 0.0;
                            double midpoint = 0.5;
                            if (filterParams.Length >= 2) {
                                midpoint = FilterUtils.TryParseDouble(filterParams[1]) ?? 0.5;
                            }
        
                            filtered.Lightness = ColorMath.SSpline(hsl.Lightness, strength, midpoint);
                        }
        
                        yield return hsl.Interpolate(filtered, rangeFactor);
                    }
                }
        
                public static IEnumerable<Color> ContrastValue(IEnumerable<Color> colors, params object[] filterParams)
                {
                    ColorRange range;
                    (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
                    foreach (var color in colors) {
                        var hsv = color.ToHsv();
                        var rangeFactor = FilterUtils.GetRangeFactor(range, hsv);
                        var filtered = color.ToHsv();
        
                        if (filterParams.Any()) {
                            double strength = FilterUtils.TryParseDouble(filterParams[0]) ?? 0.0;
                            double midpoint = 0.5;
                            if (filterParams.Length >= 2) {
                                midpoint = FilterUtils.TryParseDouble(filterParams[1]) ?? 0.5;
                            }
        
                            filtered.Value = ColorMath.SSpline(hsv.Value, strength, midpoint);
                        }
        
                        yield return hsv.Interpolate(filtered, rangeFactor);
                    }
                }
        
                #endregion
        
                #region Hue
        
                public static IEnumerable<Color> ShiftHslHue(IEnumerable<Color> colors, params object[] filterParams)
                {
                    ColorRange range;
                    (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
                    foreach (var color in colors) {
                        var hsl = color.ToHsl();
                        var rangeFactor = FilterUtils.GetRangeFactor(range, hsl);
                        var filtered = color.ToHsl();
        
        
                        if (filterParams.Any()) {
                            double hueShift = FilterUtils.TryParseDouble(filterParams[0]) ?? 0.0;
                            filtered.Hue = hsl.Hue + hueShift;
                        }
        
                        yield return hsl.Interpolate(filtered, rangeFactor);
                    }
                }
        
                public static IEnumerable<Color> ShiftHsvHue(IEnumerable<Color> colors, params object[] filterParams)
                {
                    ColorRange range;
                    (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
                    foreach (var color in colors) {
                        var hsv = color.ToHsv();
                        var rangeFactor = FilterUtils.GetRangeFactor(range, hsv);
                        var filtered = color.ToHsv();
        
                        if (filterParams.Any()) {
                            double hueShift = FilterUtils.TryParseDouble(filterParams[0]) ?? 0.0;
                            filtered.Hue = hsv.Hue + hueShift;
                        }
        
                        yield return hsv.Interpolate(filtered, rangeFactor);
                    }
                }
        
                #endregion
        
                #region "Levels"
        
                public static IEnumerable<Color> LevelsRgb(IEnumerable<Color> colors, params object[] filterParams)
                {
                    ColorRange range;
                    (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
                    foreach (var color in colors) {
                        var rgb = color.ToRgb();
                        var rangeFactor = FilterUtils.GetRangeFactor(range, rgb);
                        var filtered = color.ToRgb();
        
                        (double inBlack, double inWhite, double gamma, double outBlack, double outWhite) =
                            FilterUtils.ParseLevelsParameters(filterParams);
        
                        filtered.Red8 = ColorMath.Levels(rgb.Red8, inBlack, inWhite, gamma, outBlack, outWhite);
                        filtered.Green8 = ColorMath.Levels(rgb.Green8, inBlack, inWhite, gamma, outBlack, outWhite);
                        filtered.Blue8 = ColorMath.Levels(rgb.Blue8, inBlack, inWhite, gamma, outBlack, outWhite);
        
                        yield return rgb.Interpolate(filtered, rangeFactor);
                    }
                }
        
                public static IEnumerable<Color> LevelsRed(IEnumerable<Color> colors, params object[] filterParams)
                {
                    ColorRange range;
                    (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
                    foreach (var color in colors) {
                        var rgb = color.ToRgb();
                        var rangeFactor = FilterUtils.GetRangeFactor(range, rgb);
                        var filtered = color.ToRgb();
        
                        (double inBlack, double inWhite, double gamma, double outBlack, double outWhite) =
                            FilterUtils.ParseLevelsParameters(filterParams);
        
                        filtered.Red8 = ColorMath.Levels(rgb.Red8, inBlack, inWhite, gamma, outBlack, outWhite);
        
                        yield return rgb.Interpolate(filtered, rangeFactor);
                    }
                }
        
                public static IEnumerable<Color> LevelsGreen(IEnumerable<Color> colors, params object[] filterParams)
                {
                    ColorRange range;
                    (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
                    foreach (var color in colors) {
                        var rgb = color.ToRgb();
                        var rangeFactor = FilterUtils.GetRangeFactor(range, rgb);
                        var filtered = color.ToRgb();
        
                        (double inBlack, double inWhite, double gamma, double outBlack, double outWhite) =
                            FilterUtils.ParseLevelsParameters(filterParams);
        
                        filtered.Green8 = ColorMath.Levels(rgb.Green8, inBlack, inWhite, gamma, outBlack, outWhite);
        
                        yield return rgb.Interpolate(filtered, rangeFactor);
                    }
                }
        
                public static IEnumerable<Color> LevelsBlue(IEnumerable<Color> colors, params object[] filterParams)
                {
                    ColorRange range;
                    (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
                    foreach (var color in colors) {
                        var rgb = color.ToRgb();
                        var rangeFactor = FilterUtils.GetRangeFactor(range, rgb);
                        var filtered = color.ToRgb();
        
                        (double inBlack, double inWhite, double gamma, double outBlack, double outWhite) =
                            FilterUtils.ParseLevelsParameters(filterParams);
        
                        filtered.Blue8 = ColorMath.Levels(rgb.Blue8, inBlack, inWhite, gamma, outBlack, outWhite);
        
                        yield return rgb.Interpolate(filtered, rangeFactor);
                    }
                }
        
                public static IEnumerable<Color> LevelsLightness(IEnumerable<Color> colors, params object[] filterParams)
                {
                    ColorRange range;
                    (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
                    foreach (var color in colors) {
                        var hsl = color.ToHsl();
                        var rangeFactor = FilterUtils.GetRangeFactor(range, hsl);
                        var filtered = color.ToHsl();
        
                        (double inBlack, double inWhite, double gamma, double outBlack, double outWhite) =
                            FilterUtils.ParseLevelsParameters(filterParams);
        
                        filtered.Lightness = ColorMath.Levels(hsl.Lightness, inBlack, inWhite, gamma, outBlack, outWhite);
        
                        yield return hsl.Interpolate(filtered, rangeFactor);
                    }
                }
        
                public static IEnumerable<Color> LevelsValue(IEnumerable<Color> colors, params object[] filterParams)
                {
                    ColorRange range;
                    (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
                    foreach (var color in colors) {
                        var hsl = color.ToHsl();
                        var rangeFactor = FilterUtils.GetRangeFactor(range, hsl);
                        var filtered = color.ToHsl();
        
                        (double inBlack, double inWhite, double gamma, double outBlack, double outWhite) =
                            FilterUtils.ParseLevelsParameters(filterParams);
        
                        filtered.Lightness = ColorMath.Levels(hsl.Lightness, inBlack, inWhite, gamma, outBlack, outWhite);
        
                        yield return hsl.Interpolate(filtered, rangeFactor);
                    }
                }
        
                public static IEnumerable<Color> LevelsHslSaturation(IEnumerable<Color> colors,
                    params object[] filterParams)
                {
                    ColorRange range;
                    (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
                    foreach (var color in colors) {
                        var hsl = color.ToHsl();
                        var rangeFactor = FilterUtils.GetRangeFactor(range, hsl);
                        var filtered = color.ToHsl();
        
                        (double inBlack, double inWhite, double gamma, double outBlack, double outWhite) =
                            FilterUtils.ParseLevelsParameters(filterParams);
        
                        filtered.Saturation = ColorMath.Levels(hsl.Saturation, inBlack, inWhite, gamma, outBlack, outWhite);
        
                        yield return hsl.Interpolate(filtered, rangeFactor);
                    }
                }
        
                public static IEnumerable<Color> LevelsHsvSaturation(IEnumerable<Color> colors, params object[] filterParams)
                {
                    ColorRange range;
                    (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
                    foreach (var color in colors) {
                        var hsv = color.ToHsv();
                        var rangeFactor = FilterUtils.GetRangeFactor(range, hsv);
                        var filtered = color.ToHsv();
        
                        (double inBlack, double inWhite, double gamma, double outBlack, double outWhite) =
                            FilterUtils.ParseLevelsParameters(filterParams);
        
                        filtered.Saturation = ColorMath.Levels(hsv.Saturation, inBlack, inWhite, gamma, outBlack, outWhite);
        
                        yield return hsv.Interpolate(filtered, rangeFactor);
                    }
                }
        
                #endregion
        
                #region "Misc"
        
                public static IEnumerable<Color> BrightnessToGrayScale(IEnumerable<Color> colors,
                    params object[] filterParams)
                {
                    ColorRange range;
                    (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
                    foreach (var color in colors) {
                        var rgb = color.ToRgb();
                        var rangeFactor = FilterUtils.GetRangeFactor(range, rgb);
                        var filtered = color.ToRgb();
        
                        if (filterParams.Any()) {
                            var br = ColorMath.RgbPerceivedBrightness(rgb.Red8, rgb.Green8, rgb.Blue8);
                            filtered.Red8 = br;
                            filtered.Green8 = br;
                            filtered.Blue8 = br;
                        }
        
                        yield return rgb.Interpolate(filtered, rangeFactor);
                    }
                }
        */

        #endregion
    }
}