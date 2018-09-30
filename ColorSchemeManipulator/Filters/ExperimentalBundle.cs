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

            // CliArgs.Register(new List<string> {"-leS", "--levels-hsv-saturation"}, LevelsHsvSaturation, 5,
            //     desc:
            //     "Adjusts levels of HSV saturation. Takes five parameters: input black (0..1), input white (0..1), gamma (0.01..9.99), output black (0..1), output white (0..1)");

            CliArgs.Register(new List<string> {"-ibc", "--invert-brightness-corr"},
                InvertPerceivedBrightnessWithCorrection, 0, 1,
                paramList: "[=<corr>]",
                desc: "Inverts perceived brightness with correction parameter.",
                paramDesc:
                "<corr> is value between 0..1, 0 is safest conversion, 1 is closest to truth but also causes clipping of some colors.");

            CliArgs.Register(new List<string> {"-ilv", "--invert-lightness-value"}, InvertMixedLightnessAndValue, 0, 1,
                paramList: "=<mix>",
                desc: "Inverts colors using both lightness and value, by mixing the result by parameter (0..1)",
                paramDesc: "<mix> is mix parameter 0..1, 0 is full lightness inversion, 1 is full value inversion.");

            CliArgs.Register(new List<string> {"-b2l", "--brightness-to-lightness"}, BrightnessToLightness, 0, 0,
                desc: "Translates perceived brightness to lightness."
            );

            CliArgs.Register(new List<string> {"-b2v", "--brightness-to-value"}, BrightnessToValue, 0, 0,
                desc: "Translates perceived brightness to value."
            );

            CliArgs.Register(new List<string> {"-cS", "--contrast-hsv-sat"}, ContrastHsvSaturation, 1, 2,
                paramList: "=<contrast>[,<ip>]",
                desc:
                "Applies contrast curve to HSV saturation.",
                paramDesc:
                "<contrast> is curvature strength in range of -1..1 (0), <ip> is inflection point in range of 0..1 (0.5)");


            CliArgs.Register(new List<string> {"-gaS", "--gamma-hsv-sat"}, GammaHsvSaturation, 1,
                paramList: "=<gamma>",
                desc: "Adjusts gamma of HSV saturation.",
                paramDesc: "<gamma> is value in range of 0.01..9.99 (1.0)");


            CliArgs.Register(new List<string> {"-S", "--hsv-saturation"}, GainHsvSaturation, 1,
                paramList: "=<value>",
                desc: "HSV saturation gain.",
                paramDesc: "<value> is multiplier in range of 0..10 (1.0)");

            CliArgs.Register(new List<string> {"--tolight"}, ToLight, 0, 0,
                desc: "A preset with multiple filters to convert dark scheme to light");

            // CliArgs.Register(new List<string> {"--bypass"}, ByBass, 0);

            GetInstance()._isRegistered = true;
        }

        public static IEnumerable<Color> InvertPerceivedBrightnessWithCorrection(IEnumerable<Color> colors,
            ColorRange range,
            params double[] filterParams)
        {
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
            ColorRange range,
            params double[] filterParams)
        {
            double mix = 0.333333;
            if (filterParams.Any() && FilterUtils.IsNumberOrString(filterParams[0])) {
                mix = (filterParams.Length >= 2 ? filterParams[1] : 0.5).Clamp(0, 1);
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

        public static IEnumerable<Color> BrightnessToLightness(IEnumerable<Color> colors, ColorRange range,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(range, color);
                var filtered = new Color(color);

                if (filterParams.Any()) {
                    var br = color.GetBrightness();
                    filtered.Lightness = br;
                    yield return color.InterpolateWith(filtered, rangeFactor);
                }
            }
        }

        public static IEnumerable<Color> GainHsvSaturation(IEnumerable<Color> colors, ColorRange range,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(range, color);
                var filtered = new Color(color);

                if (filterParams.Any()) {
                    double gain = (filterParams[0]);
                    filtered.SaturationHsv = (filtered.SaturationHsv * gain);
                }

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }

        public static IEnumerable<Color> BrightnessToValue(IEnumerable<Color> colors, ColorRange range,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(range, color);
                var filtered = new Color(color);

                if (filterParams.Any()) {
                    var br = ColorMath.RgbPerceivedBrightness(color.Red, color.Green, color.Blue);
                    filtered.Value = br;
                    yield return color.InterpolateWith(filtered, rangeFactor);
                }
            }
        }

        public static IEnumerable<Color> ToLight(IEnumerable<Color> colors, ColorRange range,
            params double[] filterParams)
        {
            var filterSet = new FilterSet()
                    .Add(FilterBundle.GainLightness,
                        new ColorRange().Brightness(0.7, 1, 0.15, 0)
                            .Saturation(0.7, 1, 0.1, 0),
                        0.6) // dampen "neon" rgb before so don't get too dark
                    .Add(FilterBundle.InvertPerceivedBrightness) // invert image
                    .Add(FilterBundle.AutoLevelsRgb, null, 0.15, 1, 1.2) // add some brightness
                    .Add(FilterBundle.GammaHslSaturation, new ColorRange().Saturation4P(0.1, 0.1, 1, 1),
                        1.3
                    )
                    //                    .Add(FilterBundle.GammaRgb, 1.7,
                    //                        new ColorRange()
                    //                            .Hue(37, 56, 6, 20).Lightness(0.04, 0.6, 0, 0.2)) // yellow-neon green boost
                    //                    .Add(FilterBundle.GainHslSaturation, 1.7,
                    //                        new ColorRange().Hue(37, 56, 6, 20).Lightness(0.04, 0.6, 0, 0.2)) // yellow-neon green boost
                    .Add(FilterBundle.GainRgb,
                        new ColorRange().Saturation4P(0.1, 0.3, 0.6, 0.9)
                            .Lightness4P(0, 0, 0.4, 0.7), 1.3
                    ) // add saturation for weak rgb
                    .Add(FilterBundle.GainHslSaturation,
                        new ColorRange().Saturation4P(0.1, 0.1, 0.3, 0.7)
                            .Lightness4P(0, 0, 0.3, 0.7), 2) // add saturation for weak rgb
                ;

            return filterSet.ApplyTo(colors);
        }

        public static IEnumerable<Color> LevelsHsvSaturation(IEnumerable<Color> colors, ColorRange range,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(range, color);
                var filtered = new Color(color);

                (double inBlack, double inWhite, double gamma, double outBlack, double outWhite) =
                    FilterUtils.GetLevelsParameters(filterParams);

                filtered.SaturationHsv =
                    ColorMath.Levels(color.SaturationHsv, inBlack, inWhite, gamma, outBlack, outWhite);

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }

        public static IEnumerable<Color> ContrastHsvSaturation(IEnumerable<Color> colors,
            ColorRange range,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(range, color);
                var filtered = new Color(color);

                if (filterParams.Any()) {
                    double strength = filterParams[0];
                    double midpoint = filterParams.Length >= 2 ? filterParams[1] : 0.5;

                    filtered.SaturationHsv = ColorMath.SSpline(color.SaturationHsv, strength, midpoint);
                }

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }


        public static IEnumerable<Color> GammaHsvSaturation(IEnumerable<Color> colors, ColorRange range,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(range, color);
                var filtered = new Color(color);

                if (filterParams.Any()) {
                    double gamma = filterParams[0];
                    filtered.SaturationHsv = ColorMath.Gamma(color.SaturationHsv, gamma);
                }

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }

        public static IEnumerable<Color> AutoLevelsRgbByBrightness(IEnumerable<Color> colors,
            ColorRange range,
            params double[] filterParams)
        {
            (double inBlack, double inWhite) = FilterUtils.GetLowestAndHighestLightness(colors);

            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(range, color);
                var filtered = new Color(color);

                (double outBlack, double outWhite, double gamma) =
                    FilterUtils.GetAutoLevelParameters(filterParams);

                filtered.Red = ColorMath.Levels(color.Red, inBlack, inWhite, gamma, outBlack, outWhite);
                filtered.Green = ColorMath.Levels(color.Green, inBlack, inWhite, gamma, outBlack, outWhite);
                filtered.Blue = ColorMath.Levels(color.Blue, inBlack, inWhite, gamma, outBlack, outWhite);

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }

        public static IEnumerable<Color> ByBass(IEnumerable<Color> colors, ColorRange range,
            params double[] filterParams)
        {
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