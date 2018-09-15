using System.Collections.Generic;
using System.Linq;
using ColorSchemeManipulator.CLI;
using ColorSchemeManipulator.Colors;
using ColorSchemeManipulator.Common;

namespace ColorSchemeManipulator.Filters
{
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
            
            CliArgs.Register(new List<string> {"-ib", "--invert-brightness"}, InvertPerceivedBrightness, 0, 0,
                desc: "Inverts perceived brightness - experimental");
            CliArgs.Register(new List<string> {"-ilv", "--invert-lightness-value"}, InvertMixedLightnessAndValue, 0, 1,
                desc: "Inverts colors using both lightness and value, by mixing the result - experimental");
            CliArgs.Register(new List<string> {"-b2l", "--brightness-to-lightness"}, BrightnessToLightness, 0, 0,
                desc: null);
            CliArgs.Register(new List<string> {"-b2v", "--brightness-to-value"}, BrightnessToValue, 0, 0,
                desc: null);
            CliArgs.Register(new List<string> { "--tolight"}, ToLight, 0, 0,
                desc: null);

            GetInstance()._isRegistered = true;
        }

        public static IEnumerable<Color> InvertPerceivedBrightness(IEnumerable<Color> colors,
            params object[] filterParams)
        {
            ColorRange range;
            (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
            foreach (var color in colors) {
                var hsl = color.ToHsl();
                var rgb = color.ToRgb();
                var rangeFactor = FilterUtils.GetRangeFactor(range, hsl);

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

                yield return rgb.Interpolate(corrected, rangeFactor);
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
                var hsl = color.ToHsl();
                var rangeFactor = FilterUtils.GetRangeFactor(range, hsl);
                var hslFiltered = color.ToHsl();
                var hsv = color.ToHsv();
                var hsvFiltered = new Hsv(hsv);
                hslFiltered.Lightness = ColorMath.Invert(hsl.Lightness);
                hsvFiltered.Value = ColorMath.Invert(hsv.Value);
                hsl = hsl.Interpolate(hslFiltered, rangeFactor);
                hsv = hsv.Interpolate(hsvFiltered, rangeFactor);

                yield return hsl.Interpolate(hsv.ToHsl(), mix);
            }
        }

        public static IEnumerable<Color> BrightnessToLightness(IEnumerable<Color> colors, params object[] filterParams)
        {
            ColorRange range;
            (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
            foreach (var color in colors) {
                var rgb = color.ToRgb();
                var rangeFactor = FilterUtils.GetRangeFactor(range, rgb);
                var filtered = color.ToHsl();

                if (filterParams.Any()) {
                    var br = ColorMath.RgbPerceivedBrightness(rgb.Red, rgb.Green, rgb.Blue);
                    filtered.Lightness = br.Clamp(0, 1);
                    // filtered.Saturation = 0;
                    yield return rgb.Interpolate(filtered.ToRgb(), rangeFactor);
                }
            }
        }

        public static IEnumerable<Color> BrightnessToValue(IEnumerable<Color> colors, params object[] filterParams)
        {
            ColorRange range;
            (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
            foreach (var color in colors) {
                var rgb = color.ToRgb();
                var rangeFactor = FilterUtils.GetRangeFactor(range, rgb);
                var filtered = color.ToHsv();

                if (filterParams.Any()) {
                    var br = ColorMath.RgbPerceivedBrightness(rgb.Red, rgb.Green, rgb.Blue);
                    filtered.Value = br.Clamp(0, 1);
                    // filtered.Saturation = 0;
                    yield return rgb.Interpolate(filtered.ToRgb(), rangeFactor);
                }
            }
        }

        public static IEnumerable<Color> ToLight(IEnumerable<Color> colors, params object[] filterParams)
        {
           
            FilterSet filterSet = new FilterSet()
                .Add(FilterBundle.GainLightness, 0.6,
                    new ColorRange().Brightness(0.7, 1, 0.15, 0)
                        .Saturation(0.7, 1, 0.1, 0)) // dampen "neon" rgb before so don't get too dark
                .Add(ExperimentalBundle.InvertPerceivedBrightness) // invert image
                .Add(FilterBundle.LevelsLightness, 0.1, 0.9, 1, 0.1, 1) // add some brightness
                .Add(FilterBundle.GammaRgb, 1.7,
                    new ColorRange()
                        .Hue(37, 56, 6, 20).Lightness(0.04, 0.6, 0, 0.2)) // yellow-neon green boost
                .Add(FilterBundle.GainHslSaturation, 1.7,
                    new ColorRange().Hue(37, 56, 6, 20).Lightness(0.04, 0.6, 0, 0.2)) // yellow-neon green boost
                .Add(FilterBundle.GammaHslSaturation, 1.4,
                    new ColorRange().Saturation4P(0.1, 0.1, 0.5, 0.7)) // add saturation for weak rgb
                ;
        
            return filterSet.ApplyTo(colors);
        }
    }
}