using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            CliArgs.Register(new List<string> {"-leS", "--levels-hsv-saturation"}, LevelsHsvSaturation, 5,
                desc:
                "Adjusts levels of HSV saturation. Takes five parameters: input black (0..1), input white (0..1), gamma (0.01..9.99), output black (0..1), output white (0..1)");

            CliArgs.Register(new List<string> {"-ipc"},
                InvertPerceivedLightnessWithCorrection, 0, 1,
                paramList: "[=<corr>]",
                desc: "Inverts perceived brightness with correction parameter.",
                paramDesc:
                "<corr> is value between 0..1, 0 is safest conversion, 1 is closest to truth but also causes clipping of some colors.");

            CliArgs.Register(new List<string> {"-ilv"}, InvertMixedLightnessAndValue, 0, 1,
                paramList: "=<mix>",
                desc: "Inverts colors using both lightness and value, by mixing the result by parameter (0..1)",
                paramDesc: "<mix> is mix parameter 0..1, 0 is full lightness inversion, 1 is full value inversion.");

            CliArgs.Register(new List<string> {"-ipm"},
                InvertMixedPerceivedLightnessAndValue, 0, 1,
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

            CliArgs.Register(new List<string> {"-ipv", "--invert-per-value"}, InvertPerceivedValue, 0, 0,
                desc: "Inverts perceived brightness.");

            // CliArgs.Register(new List<string> {"--bypass"}, ByBass, 0);

            GetInstance()._isRegistered = true;
        }

        public static Color InvertPerceivedLightnessWithCorrection(Color color,
            ColorRange range,
            params double[] filterParams)
        {
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
            var corr = targetBrightness / newBrightness + (targetBrightness / newBrightness - 1); // / 4;
            if (filterParams.Any())
                corr = ColorMath.LinearInterpolation(filterParams[0], 0, 1, 1, corr);
            else {
                corr = 1;
            }

            double r = inverted.Red * corr;
            double g = inverted.Green * corr;
            double b = inverted.Blue * corr;

            var corrected = Color.FromRgb(r, g, b, color.Alpha);

            // var correctedBrightness = ColorMath.RgbPerceivedBrightness(corrected.Red,
            //     corrected.Green, corrected.Blue);
            color.InterpolateWith(corrected, rangeFactor);
            return color;
        }

        public static Color InvertPerceivedValue(Color color,
            ColorRange colorRange = null,
            params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var brightness = ColorMath.RgbPerceivedBrightness(color.Red, color.Green, color.Blue);
            var targetBrightness = (1 - brightness);

            // using brightness as lightness is not accurate but we can correct this later
            // how ever it seems that completely correct value produces worse outcome
            // so we may use something in between
            var inverted = Color.FromHsv(color.HueHsv, color.SaturationHsv, targetBrightness, color.Alpha);

            color.InterpolateWith(inverted, rangeFactor);
            return color;
        }

        public static Color InvertMixedPerceivedLightnessAndValue(Color color,
            ColorRange range,
            params double[] filterParams)
        {
            double mix = 0.5;
            if (filterParams.Any()) {
                mix = filterParams[0].Clamp(0, 1);
            }

            var brightness = ColorMath.RgbPerceivedBrightness(color.Red, color.Green, color.Blue);
            var targetBrightness = (1 - brightness);

            // using brightness as lightness is not accurate but we can correct this later
            // how ever it seems that completely correct value produces worse outcome
            // so we may use something in between
            var inverted = Color.FromHsl(color.Hue, color.Saturation, targetBrightness, color.Alpha);

            var rangeFactor = FilterUtils.GetRangeFactor(range, color);
            var hslFiltered = Color.FromHsl(color.Hue, color.Saturation, targetBrightness, color.Alpha);
            var hsvFiltered = Color.FromHsv(color.HueHsv, color.SaturationHsv, targetBrightness, color.Alpha);
            var hsl = color.InterpolateWith(hslFiltered, rangeFactor).Clone();
            var hsv = color.InterpolateWith(hsvFiltered, rangeFactor).Clone();

            hsl.InterpolateWith(hsv, mix);
            return hsl;
        }

        public static Color InvertMixedLightnessAndValue(Color color,
            ColorRange range,
            params double[] filterParams)
        {
            double mix = 0.5;
            if (filterParams.Any()) {
                mix = filterParams[0].Clamp(0, 1);
            }

            var rangeFactor = FilterUtils.GetRangeFactor(range, color);
            var hslFiltered = new Color(color);
            var hsvFiltered = new Color(color);
            hslFiltered.Lightness = ColorMath.Invert(color.Lightness);
            hsvFiltered.Value = ColorMath.Invert(color.Value);
            var hsl = color.InterpolateWith(hslFiltered, rangeFactor).Clone();
            var hsv = color.InterpolateWith(hsvFiltered, rangeFactor).Clone();

            hsl.InterpolateWith(hsv, mix);
            return hsl;
        }

        public static Color BrightnessToLightness(Color color, ColorRange colorRange = null, params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var filtered = new Color(color);

            var br = color.GetBrightness();
            filtered.Lightness = br;

            color.InterpolateWith(filtered, rangeFactor);
            return color;
        }

        public static Color GainHsvSaturation(Color color, ColorRange colorRange = null, params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var filtered = new Color(color);

            if (filterParams.Any()) {
                double gain = (filterParams[0]);
                filtered.SaturationHsv = (filtered.SaturationHsv * gain);
            }

            color.InterpolateWith(filtered, rangeFactor);
            return color;
        }

        public static Color BrightnessToValue(Color color, ColorRange colorRange = null, params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var filtered = new Color(color);

            var br = ColorMath.RgbPerceivedBrightness(color.Red, color.Green, color.Blue);
            filtered.Value = br;

            color.InterpolateWith(filtered, rangeFactor);
            return color;
        }


        public static IEnumerable<Color> ToLight(IEnumerable<Color> colors, ColorRange range,
            params double[] filterParams)
        {
            var filterChain = new FilterChain()
                    // .Add(FilterBundle.GainLightness,
                    //     new ColorRange()
                    //         .Brightness(0.7, 1, 0.2, 0)
                    //         .Saturation(0.7, 1, 0.2, 0),
                    //     0.8) // dampen "neon" rgb before so don't get too dark
                    .Add(InvertMixedPerceivedLightnessAndValue, null, 0.4) // invert image
                    // .Add(FilterBundle.Clamp)
                    .Add(FilterBundle.AutoLevelsLightness, null, 0.1, 1, 1.05) // add some brightness
                    .Add(FilterBundle.LevelsRgb,
                        new ColorRange()
                            .Saturation4P(0.1, 0.1, 1, 1)
                            .Brightness4P(0, 0, 0.3, 0.5),
                        0, 1, 1, 0.3, 1)
                    .Add(FilterBundle.GammaHslSaturation,
                        new ColorRange()
                            .Saturation4P(0.1, 0.1, 0.3, 0.6)
                            .Brightness4P(0, 0.1, 0.4, 0.7),
                        2.4
                    )
                    .Add(FilterBundle.GammaRgb,
                        new ColorRange()
                            .Hue(37, 56, 6, 20)
                            .Lightness(0.04, 0.6, 0, 0.2), 1.2) // yellow-neon green boost
                    .Add(FilterBundle.GainHslSaturation,
                        new ColorRange()
                            .Hue(37, 56, 6, 20)
                            .Lightness(0.04, 0.6, 0, 0.2), 1.3)
                ; // yellow-neon green boost

            return filterChain.ApplyTo(colors);
        }

        public static Color LevelsHsvSaturation(Color color, ColorRange colorRange = null, params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var filtered = new Color(color);

            (double inBlack, double inWhite, double gamma, double outBlack, double outWhite) =
                FilterUtils.GetLevelsParameters(filterParams);

            filtered.SaturationHsv =
                ColorMath.Levels(color.SaturationHsv, inBlack, inWhite, gamma, outBlack, outWhite);

            color.InterpolateWith(filtered, rangeFactor);
            return color;
        }

        public static Color ContrastHsvSaturation(Color color,
            ColorRange range,
            params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(range, color);
            var filtered = new Color(color);

            if (filterParams.Any()) {
                double strength = filterParams[0];
                double midpoint = filterParams.Length >= 2 ? filterParams[1] : 0.5;

                filtered.SaturationHsv = ColorMath.SSpline(color.SaturationHsv, strength, midpoint);
            }

            color.InterpolateWith(filtered, rangeFactor);
            return color;
        }

        public static Color GammaHsvSaturation(Color color, ColorRange colorRange = null, params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var filtered = new Color(color);

            if (filterParams.Any()) {
                double gamma = filterParams[0];
                filtered.SaturationHsv = ColorMath.Gamma(color.SaturationHsv, gamma);
            }

            color.InterpolateWith(filtered, rangeFactor);
            return color;
        }

        public static IEnumerable<Color> AutoLevelsRgbByBrightness(IEnumerable<Color> colors,
            ColorRange range,
            params double[] filterParams)
        {
            List<Color> cache = colors.ToList();
            (double inBlack, double inWhite) = FilterUtils.GetLowestAndHighestLightness(cache);

            var result = cache.AsParallel().AsOrdered().WithDegreeOfParallelism(2).Select(
                color =>
                {
                    var rangeFactor = FilterUtils.GetRangeFactor(range, color);
                    var filtered = new Color(color);

                    (double outBlack, double outWhite, double gamma) =
                        FilterUtils.GetAutoLevelParameters(filterParams);

                    filtered.Red = ColorMath.Levels(color.Red, inBlack, inWhite, gamma, outBlack, outWhite);
                    filtered.Green = ColorMath.Levels(color.Green, inBlack, inWhite, gamma, outBlack, outWhite);
                    filtered.Blue = ColorMath.Levels(color.Blue, inBlack, inWhite, gamma, outBlack, outWhite);

                    color.InterpolateWith(filtered, rangeFactor);
                    return color;
                }
            );

            foreach (var color in result) {
                yield return color;
            }
        }

        public static Color ByBass(Color color, ColorRange colorRange = null, params double[] filterParams)
        {
            if (filterParams.Any()) {
                (double h, double s, double l) = color.GetHslComponents();
                var newColor = Color.FromHsl(h, s, l, color.Alpha);
                return newColor;
            } else {
                return color;
            }
        }
    }
}