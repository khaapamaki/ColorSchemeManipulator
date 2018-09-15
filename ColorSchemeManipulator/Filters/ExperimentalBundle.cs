using System.Collections.Generic;
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

            // CliArgs.Register(new List<string> {"-b2l", "--brightness-to-lightness"}, BrightnessToLightness, 0, 0,
            //     desc: null);
            // CliArgs.Register(new List<string> {"-b2v", "--brightness-to-value"}, BrightnessToValue, 0, 0,
            //     desc: null);
            // CliArgs.Register(new List<string> { "--tolight"}, ToLight, 0, 0,
            //     desc: null);

        }

        // public static IEnumerable<Color> BrightnessToLightness(IEnumerable<Color> colors, params object[] filterParams)
        // {
        //     var filtered = new Hsl(rgb);
        //     double rangeFactor;
        //     (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(rgb, filterParams);
        //     var br = ColorMath.RgbPerceivedBrightness(rgb.Red, rgb.Green, rgb.Blue);
        //     filtered.Lightness = br.Clamp(0, 1);
        //     // filtered.Saturation = 0;
        //     return rgb.Interpolate(filtered.ToRgb(), rangeFactor);
        // }
        //
        // public static IEnumerable<Color> BrightnessToValue(IEnumerable<Color> colors, params object[] filterParams)
        // {
        //     var filtered = new Hsv(rgb);
        //     double rangeFactor;
        //     (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(rgb, filterParams);
        //     var br = ColorMath.RgbPerceivedBrightness(rgb.Red, rgb.Green, rgb.Blue);
        //     filtered.Value = br.Clamp(0, 1);
        //     // filtered.Saturation = 0;
        //     return rgb.Interpolate(filtered.ToRgb(), rangeFactor);
        // }
        //
        // public static IEnumerable<Color> ToLight(IEnumerable<Color> colors, params object[] filterParams)
        // {
        //     FilterSet filterSet = new FilterSet()
        //         .Add(FilterBundle.GainLightness, 0.6,
        //             new ColorRange().Brightness(0.7, 1, 0.15, 0)
        //                 .Saturation(0.7, 1, 0.1, 0)) // dampen "neon" rgb before so don't get too dark
        //         .Add(FilterBundle.InvertPerceivedBrightness) // invert image
        //         .Add(FilterBundle.LevelsLightness, 0.1, 0.9, 1, 0.1, 1) // add some brightness
        //         .Add(FilterBundle.GammaRgb, 1.7,
        //             new ColorRange()
        //                 .Hue(37, 56, 6, 20).Lightness(0.04, 0.6, 0, 0.2)) // yellow-neon green boost
        //         .Add(FilterBundle.GainHslSaturation, 1.7,
        //             new ColorRange().Hue(37, 56, 6, 20).Lightness(0.04, 0.6, 0, 0.2)) // yellow-neon green boost
        //         .Add(FilterBundle.GammaHslSaturation, 1.4,
        //             new ColorRange().Saturation4P(0.1, 0.1, 0.5, 0.7)) // add saturation for weak rgb
        //         ;
        //
        //     return filterSet.ApplyTo(rgb);
        // }
    }
}