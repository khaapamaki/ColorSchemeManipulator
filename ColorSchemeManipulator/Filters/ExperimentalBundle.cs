using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

            CliArgs.Register(new List<string> {"-ibc", "--invert-brightness-corrected"},
                InvertPerceivedBrightnessWithCorrection, 0, 0,
                desc: "Inverts perceived brightness - experimental");
            CliArgs.Register(new List<string> {"-ilv", "--invert-lightness-value"}, InvertMixedLightnessAndValue, 0, 1,
                desc: "Inverts colors using both lightness and value, by mixing the result - experimental");
            CliArgs.Register(new List<string> {"-b2l", "--brightness-to-lightness"}, BrightnessToLightness, 0, 0
            );
            CliArgs.Register(new List<string> {"-b2v", "--brightness-to-value"}, BrightnessToValue, 0, 0
            );
            CliArgs.Register(new List<string> {"--tolight"}, ToLight, 0, 0
            );
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
                var targetBrightness = (1 - brightness).Clamp(0, 1);

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
                r = r.Clamp(0, 1);
                g = g.Clamp(0, 1);
                b = b.Clamp(0, 1);
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
                mix = (FilterUtils.TryParseDouble(filterParams[0]) ?? 0.5).LimitLow(0.0);
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
                    filtered.Lightness = br.Clamp(0, 1);
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
                    filtered.Value = br.Clamp(0, 1);
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
                    .Add(FilterBundle.AutoLevelsLightness, 0.15, 1, 1.2) // add some brightness
                    .Add(FilterBundle.GammaHslSaturation, 1.3,
                        
                    new ColorRange().Saturation4P(0.1, 0.1, 1,1)
                    )
//                    .Add(FilterBundle.GammaRgb, 1.7,
//                        new ColorRange()
//                            .Hue(37, 56, 6, 20).Lightness(0.04, 0.6, 0, 0.2)) // yellow-neon green boost
//                    .Add(FilterBundle.GainHslSaturation, 1.7,
//                        new ColorRange().Hue(37, 56, 6, 20).Lightness(0.04, 0.6, 0, 0.2)) // yellow-neon green boost
                    .Add(FilterBundle.GainRgb, 1.3
                       ,
                        new ColorRange().Saturation4P(0.1, 0.3, 0.6, 0.9)
                        .Lightness4P(0,0,0.4,0.7)
                       ) // add saturation for weak rgb
                    .Add(FilterBundle.GainHslSaturation, 2,
                        new ColorRange().Saturation4P(0.1, 0.1, 0.3, 0.7)
                            .Lightness4P(0,0,0.3,0.7)) // add saturation for weak rgb
                ;

            return filterSet.ApplyTo(colors);
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