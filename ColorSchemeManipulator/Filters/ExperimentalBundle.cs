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

            CliArgs.Register(new List<string> {"-b2l", "--brightness-to-lightness"}, BrightnessToLightness, 0, 0,
                desc: null);
            CliArgs.Register(new List<string> {"-b2v", "--brightness-to-value"}, BrightnessToValue, 0, 0,
                desc: null);

        }

        public static Rgb BrightnessToLightness(Rgb rgb, params object[] filterParams)
        {
            var filtered = new Hsl(rgb);
            double rangeFactor;
            (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(rgb, filterParams);
            var br = ColorMath.RgbPerceivedBrightness(rgb.Red, rgb.Green, rgb.Blue);
            filtered.Lightness = br.Clamp(0, 1);
            // filtered.Saturation = 0;
            return rgb.Interpolate(filtered.ToRgb(), rangeFactor);
        }

        public static Rgb BrightnessToValue(Rgb rgb, params object[] filterParams)
        {
            var filtered = new Hsv(rgb);
            double rangeFactor;
            (rangeFactor, filterParams) = FilterUtils.GetRangeFactorAndRemainingParams(rgb, filterParams);
            var br = ColorMath.RgbPerceivedBrightness(rgb.Red, rgb.Green, rgb.Blue);
            filtered.Value = br.Clamp(0, 1);
            // filtered.Saturation = 0;
            return rgb.Interpolate(filtered.ToRgb(), rangeFactor);
        }
    }
}