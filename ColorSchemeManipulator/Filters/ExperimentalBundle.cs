using System.Collections.Generic;
using System.Linq;
using ColorSchemeManipulator.CLI;
using ColorSchemeManipulator.Colors;
using ColorSchemeManipulator.Common;

namespace ColorSchemeManipulator.Filters
{
    /// <summary>
    /// A set of predefined filters for experimental purposes
    /// </summary>
    public sealed class ExperimentalBundle
    {
        private static ExperimentalBundle _instance;
        private static readonly object Padlock = new object();

        private ExperimentalBundle() { }

        private static ExperimentalBundle GetInstance()
        {
            lock (Padlock) {
                return _instance ?? (_instance = new ExperimentalBundle());
            }
        }

        private bool _isRegistered = false;

        public static void RegisterCliOptions()
        {
            if (GetInstance()._isRegistered)
                return;
            CliArgs.Register(new List<string> {"-lel", "--levels-lightness"}, LevelsLightness, 5,
                paramDesc: "=<ib>,<iw>,<g>,<ob>,<ow>",
                desc:
                "Adjusts levels of HSL lightness. Takes five parameters: input black (0..1), input white (0..1), gamma (0.01..9.99), output black (0..1), output white (0..1)");
            
            CliArgs.Register(new List<string> {"-lev", "--levels-value"}, LevelsValue, 5,
                paramDesc: "=<ib>,<iw>,<g>,<ob>,<ow>",
                desc:
                "Adjusts levels of HSV value. Takes five parameters: input black (0..1), input white (0..1), gamma (0.01..9.99), output black (0..1), output white (0..1)");

            // CliArgs.Register(new List<string> {"-leS", "--levels-hsv-saturation"}, LevelsHsvSaturation, 5,
            //     desc:
            //     "Adjusts levels of HSV saturation. Takes five parameters: input black (0..1), input white (0..1), gamma (0.01..9.99), output black (0..1), output white (0..1)");
            
            CliArgs.Register(new List<string> {"-ibc", "--invert-brightness-corr"}, InvertPerceivedBrightnessWithCorrection, 0, 1,
                paramDesc: "[=<corr>]",
                desc: "Inverts perceived brightness with correction parameter (0..1)");
            
            CliArgs.Register(new List<string> {"-ilv", "--invert-lightness-value"}, InvertMixedLightnessAndValue, 0, 1,
                paramDesc: "=<mix>",
                desc: "Inverts colors using both lightness and value, by mixing the result by parameter (0..1)");
            
            CliArgs.Register(new List<string> {"-b2l", "--brightness-to-lightness"}, BrightnessToLightness, 0, 0
            );
            
            CliArgs.Register(new List<string> {"-b2v", "--brightness-to-value"}, BrightnessToValue, 0, 0
            );
            
            CliArgs.Register(new List<string> {"-cl", "--contrast-lightness"}, ContrastLightness, 1, 2,
                paramDesc: "=<c>[,<ip>]",
                desc:
                "Adjusts contrast of HSL lightness. Takes one mandatory and one optional parameter, curve strength (-1..1), inflection point (0..1 default 0.5)"
                + " Strength adjustments below zero will cause erroneuos coloring of dark tones");
            
            CliArgs.Register(new List<string> {"-cv", "--contrast-value"}, ContrastValue, 1, 2,
            paramDesc: "=<c>[,<ip>]",
                desc:
                "Adjusts contrast of HSV value. Takes one mandatory and one optional parameter, curve strength (-1..1), inflection point (0..1 default 0.5)"
                + " Strength adjustments below zero will cause erroneuos coloring of dark tones");

            CliArgs.Register(new List<string> {"-cS", "--contrast-hsv-saturation"}, ContrastHsvSaturation, 1, 2,
                paramDesc: "=<c>[,<ip>]",
                desc:
                "Adjusts contrast of HSV saturation. Takes one mandatory and one optional parameter, curve strength (-1..1), inflection point (0..1 default 0.5)");
            
            CliArgs.Register(new List<string> {"-gaS", "--gamma-hsv-saturation"}, GammaHsvSaturation, 1,
                paramDesc: "=<g>",
                desc: "Adjusts gamma of HSV saturation. Takes a single parameter (0.01..9.99)");
            
            CliArgs.Register(new List<string> {"--tolight"}, ToLight, 0, 0,
                desc: "A preset with multiple filters to convert dark scheme to light");
            
            // CliArgs.Register(new List<string> {"--bypass"}, ByBass, 0);

            GetInstance()._isRegistered = true;
        }

        public static IEnumerable<Color> InvertPerceivedBrightnessWithCorrection(IEnumerable<Color> colors,
            params object[] filterParams)
        {
            ColorRange range;
            (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(range, color);

                var brightness = ColorMath.RgbPerceivedBrightness(color.Red, color.Green, color.Blue);
                var targetBrightness = (1 - brightness);

                // using brightness as lightness is not accurate but we can correct this later
                // how ever it seems that completely correct value produces worse outcome
                // so we may use something in between
                var inverted = Color.FromHsl(color.Hue, color.Saturation, targetBrightness, color.Alpha);

                var newBrightness =
                    ColorMath.RgbPerceivedBrightness(inverted.Red, inverted.Green, inverted.Blue);

                //var delta = targetBrightness / newBrightness - 1;
                var corr = targetBrightness / newBrightness + (targetBrightness / newBrightness - 1) / 4;
                corr = 1;
                double r = inverted.Red * corr;
                double g = inverted.Green * corr;
                double b = inverted.Blue * corr;

                var corrected = Color.FromRgb(r, g, b, color.Alpha);

                // var correctedBrightness = ColorMath.RgbPerceivedBrightness(corrected.Red,
                //     corrected.Green, corrected.Blue);

                yield return color.InterpolateWith(corrected, rangeFactor);
            }
        }


        public static IEnumerable<Color> InvertMixedLightnessAndValue(IEnumerable<Color> colors,
            params object[] filterParams)
        {
            ColorRange range;
            (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);

            double mix = 0.333333;
            if (filterParams.Any() && FilterUtils.IsNumberOrString(filterParams[0])) {
                mix = (FilterUtils.TryParseDouble(filterParams[0]) ?? 0.5).Clamp(0, 1);
            }

            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(range, color);
                var hslFiltered = new Color(color);
                var hsvFiltered = new Color(color);
                hslFiltered.Lightness = ColorMath.Invert(color.Lightness);
                hsvFiltered.Value = ColorMath.Invert(color.Value);
                var hsl = color.InterpolateWith(hslFiltered, rangeFactor);
                var hsv = color.InterpolateWith(hsvFiltered, rangeFactor);

                yield return hsl.InterpolateWith(hsv, mix);
            }
        }

        public static IEnumerable<Color> BrightnessToLightness(IEnumerable<Color> colors, params object[] filterParams)
        {
            ColorRange range;
            (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(range, color);
                var filtered = new Color(color);
                ;

                if (filterParams.Any()) {
                    var br = color.GetBrightness();
                    filtered.Lightness = br;
                    // filtered.Saturation = 0;
                    yield return color.InterpolateWith(filtered, rangeFactor);
                }
            }
        }

        public static IEnumerable<Color> BrightnessToValue(IEnumerable<Color> colors, params object[] filterParams)
        {
            ColorRange range;
            (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(range, color);
                var filtered = new Color(color);
                ;

                if (filterParams.Any()) {
                    var br = ColorMath.RgbPerceivedBrightness(color.Red, color.Green, color.Blue);
                    filtered.Value = br;
                    // filtered.Saturation = 0;
                    yield return color.InterpolateWith(filtered, rangeFactor);
                }
            }
        }

        public static IEnumerable<Color> ToLight(IEnumerable<Color> colors, params object[] filterParams)
        {
            FilterSet filterSet = new FilterSet()
                    .Add(FilterBundle.GainLightness, 0.6,
                        new ColorRange().Brightness(0.7, 1, 0.15, 0)
                            .Saturation(0.7, 1, 0.1, 0)) // dampen "neon" rgb before so don't get too dark
                    .Add(FilterBundle.InvertPerceivedBrightness) // invert image
                    .Add(FilterBundle.AutoLevelsRgb, 0.15, 1, 1.2) // add some brightness
                    .Add(FilterBundle.GammaHslSaturation, 1.3,
                        new ColorRange().Saturation4P(0.1, 0.1, 1, 1)
                    )
                    //                    .Add(FilterBundle.GammaRgb, 1.7,
                    //                        new ColorRange()
                    //                            .Hue(37, 56, 6, 20).Lightness(0.04, 0.6, 0, 0.2)) // yellow-neon green boost
                    //                    .Add(FilterBundle.GainHslSaturation, 1.7,
                    //                        new ColorRange().Hue(37, 56, 6, 20).Lightness(0.04, 0.6, 0, 0.2)) // yellow-neon green boost
                    .Add(FilterBundle.GainRgb, 1.3,
                        new ColorRange().Saturation4P(0.1, 0.3, 0.6, 0.9)
                            .Lightness4P(0, 0, 0.4, 0.7)
                    ) // add saturation for weak rgb
                    .Add(FilterBundle.GainHslSaturation, 2,
                        new ColorRange().Saturation4P(0.1, 0.1, 0.3, 0.7)
                            .Lightness4P(0, 0, 0.3, 0.7)) // add saturation for weak rgb
                ;

            return filterSet.ApplyTo(colors);
        }

        public static IEnumerable<Color> LevelsLightness(IEnumerable<Color> colors, params object[] filterParams)
        {
            ColorRange range;
            (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(range, color);
                var filtered = new Color(color);

                (double inBlack, double inWhite, double gamma, double outBlack, double outWhite) =
                    FilterUtils.ParseLevelsParameters(filterParams);

                filtered.Lightness = ColorMath.Levels(color.Lightness, inBlack, inWhite, gamma, outBlack, outWhite);

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }

        public static IEnumerable<Color> LevelsValue(IEnumerable<Color> colors, params object[] filterParams)
        {
            ColorRange range;
            (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(range, color);
                var filtered = new Color(color);

                (double inBlack, double inWhite, double gamma, double outBlack, double outWhite) =
                    FilterUtils.ParseLevelsParameters(filterParams);

                filtered.Value = ColorMath.Levels(color.Value, inBlack, inWhite, gamma, outBlack, outWhite);

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }

        public static IEnumerable<Color> LevelsHsvSaturation(IEnumerable<Color> colors, params object[] filterParams)
        {
            ColorRange range;
            (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(range, color);
                var filtered = new Color(color);

                (double inBlack, double inWhite, double gamma, double outBlack, double outWhite) =
                    FilterUtils.ParseLevelsParameters(filterParams);

                filtered.SaturationHsv =
                    ColorMath.Levels(color.SaturationHsv, inBlack, inWhite, gamma, outBlack, outWhite);

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }

        public static IEnumerable<Color> ContrastHsvSaturation(IEnumerable<Color> colors,
            params object[] filterParams)
        {
            ColorRange range;
            (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(range, color);
                var filtered = new Color(color);

                if (filterParams.Any()) {
                    double strength = FilterUtils.TryParseDouble(filterParams[0]) ?? 0.0;
                    double midpoint = 0.5;
                    if (filterParams.Length >= 2) {
                        midpoint = FilterUtils.TryParseDouble(filterParams[1]) ?? 0.5;
                    }

                    filtered.SaturationHsv = ColorMath.SSpline(color.SaturationHsv, strength, midpoint);
                }

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }

        public static IEnumerable<Color> ContrastLightness(IEnumerable<Color> colors, params object[] filterParams)
        {
            ColorRange range;
            (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(range, color);
                var filtered = new Color(color);

                if (filterParams.Any()) {
                    double strength = FilterUtils.TryParseDouble(filterParams[0]) ?? 0.0;
                    double midpoint = 0.5;
                    if (filterParams.Length >= 2) {
                        midpoint = FilterUtils.TryParseDouble(filterParams[1]) ?? 0.5;
                    }

                    filtered.Lightness = ColorMath.SSpline(color.Lightness, strength, midpoint);
                }

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }

        public static IEnumerable<Color> ContrastValue(IEnumerable<Color> colors, params object[] filterParams)
        {
            ColorRange range;
            (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(range, color);
                var filtered = new Color(color);

                if (filterParams.Any()) {
                    double strength = FilterUtils.TryParseDouble(filterParams[0]) ?? 0.0;
                    double midpoint = 0.5;
                    if (filterParams.Length >= 2) {
                        midpoint = FilterUtils.TryParseDouble(filterParams[1]) ?? 0.5;
                    }

                    filtered.Value = ColorMath.SSpline(color.Value, strength, midpoint);
                }

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }

        public static IEnumerable<Color> GammaHsvSaturation(IEnumerable<Color> colors, params object[] filterParams)
        {
            ColorRange range;
            (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(range, color);
                var filtered = new Color(color);

                if (filterParams.Any()) {
                    double gamma = FilterUtils.TryParseDouble(filterParams[0]) ?? 1.0;
                    filtered.SaturationHsv = ColorMath.Gamma(color.SaturationHsv, gamma);
                }

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }

        public static IEnumerable<Color> AutoLevelsRgbByBrightness(IEnumerable<Color> colors,
            params object[] filterParams)
        {
            ColorRange range;
            (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
            (double inBlack, double inWhite) = FilterUtils.GetLowestAndHighestLightness(colors);

            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(range, color);
                var filtered = new Color(color);

                (double outBlack, double outWhite, double gamma) =
                    FilterUtils.ParseAutoLevelParameters(filterParams);

                filtered.Red = ColorMath.Levels(color.Red, inBlack, inWhite, gamma, outBlack, outWhite);
                filtered.Green = ColorMath.Levels(color.Green, inBlack, inWhite, gamma, outBlack, outWhite);
                filtered.Blue = ColorMath.Levels(color.Blue, inBlack, inWhite, gamma, outBlack, outWhite);

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }

        public static IEnumerable<Color> ByBass(IEnumerable<Color> colors, params object[] filterParams)
        {
            ColorRange range;
            (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
            foreach (var color in colors) {
                if (filterParams.Any()) {
                    (double h, double s, double l) = color.GetHslComponents();
                    var newColor = Color.FromHsl(h, s, l, color.Alpha);
                    yield return newColor;
                } else {
                    yield return color;
                }
            }
        }
    }
}