using System;
using System.Collections.Generic;
using System.Linq;
using ColorSchemeManipulator.CLI;
using ColorSchemeManipulator.Colors;
using ColorSchemeManipulator.Common;

namespace ColorSchemeManipulator.Filters
{
    // Todo Fix issue: parameter list that has comma at beginning is not properly handled causing slow prosessing (images)
    // arguments are empty string that will get default values but parsing with exception handling makes it slow
    // Todo Better argument validation could be the answer
    // other options would be pre-parsing to correct type and not parsing again when the filter is reapplied.

    /// <summary>
    /// A set of predefined filters
    /// </summary>
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
                paramList: "=<offset>",
                desc: "Hue shift.",
                paramDesc: "<offset> is hue offset in colorRange of -360..360 (0)");

            CliArgs.Register(new List<string> {"-s", "--saturation"}, GainHslSaturation, 1,
                paramList: "=<gain>",
                desc: "Saturation gain.",
                paramDesc: "<gain> is multiplier in colorRange of 0..10 (1.0)");
            
            CliArgs.Register(new List<string> {"-g", "--gain"}, GainRgb, 1,
                paramList: "=<gain>",
                desc: "RGB gain.",
                paramDesc: "<gain> is multiplier in colorRange of 0..10 (1.0)");

            CliArgs.Register(new List<string> {"-l", "--lightness"}, GainLightness, 1,
                paramList: "=<gain>",
                desc: "Lightness gain.",
                paramDesc: "<gain> is multiplier in colorRange of 0..10 (1.0)");

            CliArgs.Register(new List<string> {"-v", "--value"}, GainValue, 1,
                paramList: "=<gain>",
                desc: "Value gain.",
                paramDesc: "<gain> is multiplier in colorRange of 0..10 (1.0)");

            CliArgs.Register(new List<string> {"-c", "--contrast"}, ContrastRgb, 1, 2,
                paramList: "=<contrast>[,<ip>]",
                desc: "Adjusts contrast by S-spline curve.",
                paramDesc:
                "<contrast> is curvature strength in colorRange of -1..1 (0.0), <ip> is inflection point in colorRange of 0..1 (0.5)");

            CliArgs.Register(new List<string> {"-cl", "--contrast-lightness"}, ContrastLightness, 1, 2,
                paramList: "=<contrast>[,<ip>]",
                desc:
                "Applies contrast curve to lightness.",
                paramDesc:
                "<contrast> is curvature strength in colorRange of -1..1 (0), <ip> is inflection point in colorRange of 0..1 (0.5)");


            CliArgs.Register(new List<string> {"-cv", "--contrast-value"}, ContrastValue, 1, 2,
                paramList: "=<contrast>[,<ip>]",
                desc:
                "Applies contrast curve to value.",
                paramDesc:
                "<contrast> is curvature strength in colorRange of -1..1 (0), <ip> is inflection point in colorRange of 0..1 (0.5)");

            CliArgs.Register(new List<string> {"-cs", "--contrast-saturation"}, ContrastHslSaturation, 1, 2,
                paramList: "=<contrast>[,<ip>]",
                desc:
                "Applies contrast curve to saturation.",
                paramDesc:
                "<contrast> is curvature strength in colorRange of -1..1 (0), <ip> is inflection point in colorRange of 0..1 (0.5)");

            CliArgs.Register(new List<string> {"-ga", "--gamma"}, GammaRgb, 1, 1,
                paramList: "=<gamma>",
                desc: "Gamma correction for all RGB channels equally.",
                paramDesc: "<gamma> is value in colorRange of 0.01..9.99 (1.0)");

            CliArgs.Register(new List<string> {"-gar", "--gamma-red"}, GammaRed, 1,
                paramList: "=<gamma>",
                desc: "Adjusts gamma of red channel.",
                paramDesc: "<gamma> is value in colorRange of 0.01..9.99 (1.0)");

            CliArgs.Register(new List<string> {"-gag", "--gamma-green"}, GammaGreen, 1,
                paramList: "=<gamma>",
                desc: "Adjusts gamma of green channel.",
                paramDesc: "<gamma> is value in colorRange of 0.01..9.99 (1.0)");

            CliArgs.Register(new List<string> {"-gab", "--gamma-blue"}, GammaBlue, 1,
                paramList: "=<gamma>",
                desc: "Adjusts gamma of blue channel.",
                paramDesc: "<gamma> is value in colorRange of 0.01..9.99 (1.0)");

            CliArgs.Register(new List<string> {"-gal", "--gamma-lightness"}, GammaLightness, 1,
                paramList: "=<gamma>",
                desc: "Adjusts gamma of lightness.",
                paramDesc: "<gamma> is value in colorRange of 0.01..9.99 (1.0)");

            CliArgs.Register(new List<string> {"-gav", "--gamma-value"}, GammaValue, 1,
                paramList: "=<gamma>",
                desc: "Adjusts gamma of value.",
                paramDesc: "<gamma> is value in colorRange of 0.01..9.99 (1.0)");

            CliArgs.Register(new List<string> {"-gas", "--gamma-saturation"}, GammaHslSaturation, 1,
                paramList: "=<gamma>",
                desc: "Adjusts gamma of saturation.",
                paramDesc: "<gamma> is value in colorRange of 0.01..9.99 (1.0)");

            CliArgs.Register(new List<string> {"-le", "--levels"}, LevelsRgb, 5, 5,
                paramList: "=<ib>,<iw>,<g>,<ob>,<ow>",
                desc:
                "Adjusts levels of all RGB channels.",
                paramDesc:
                "<ib> is input black 0..1 (0), <iw> is input white 0..1 (1), <g> is gamma 0.01..9.99 (1), <ob> is output black 0..1 (0), <ow> is output white 0..1 (1)");

            CliArgs.Register(new List<string> {"-ler", "--levels-red"}, LevelsRed, 5, 5,
                paramList: "=<ib>,<iw>,<g>,<ob>,<ow>",
                desc:
                "Adjusts levels of red channel.",
                paramDesc:
                "<ib> is input black 0..1 (0), <iw> is input white 0..1 (1), <g> is gamma 0.01..9.99 (1), <ob> is output black 0..1 (0), <ow> is output white 0..1 (1)");

            CliArgs.Register(new List<string> {"-leg", "--levels-green"}, LevelsGreen, 5, 5,
                paramList: "=<ib>,<iw>,<g>,<ob>,<ow>",
                desc:
                "Adjusts levels of red channel.",
                paramDesc:
                "<ib> is input black 0..1 (0), <iw> is input white 0..1 (1), <g> is gamma 0.01..9.99 (1), <ob> is output black 0..1 (0), <ow> is output white 0..1 (1)");

            CliArgs.Register(new List<string> {"-leb", "--levels-blue"}, LevelsBlue, 5, 5,
                paramList: "=<ib>,<iw>,<g>,<ob>,<ow>",
                desc:
                "Adjusts levels of red channel.",
                paramDesc:
                "<ib> is input black 0..1 (0), <iw> is input white 0..1 (1), <g> is gamma 0.01..9.99 (1), <ob> is output black 0..1 (0), <ow> is output white 0..1 (1)");

            CliArgs.Register(new List<string> {"-al", "--auto-levels"}, AutoLevelsRgb, 0, 3,
                paramList: "=<min>,<max>,<g>",
                desc:
                "Auto levels RGB channels by normalizing them by HSV values to full scale between given minimum and maximum.",
                paramDesc: "<min> is output min 0..1 (0), <max> is output max 0..1 (1), <g> is gamma 0.01..9.99 (1)");
            
            CliArgs.Register(new List<string> {"-all", "--auto-levels-lightness"}, AutoLevelsLightness, 0, 3,
                paramList: "=<min>,<max>,<g>",
                desc:
                "Auto levels lightness by normalizing values to full scale between given minimum and maximum.",
                paramDesc: "<min> is output min 0..1 (0), <max> is output max 0..1 (1), <g> is gamma 0.01..9.99 (1)");

            CliArgs.Register(new List<string> {"-lel", "--levels-lightness"}, LevelsLightness, 5,
                paramList: "=<ib>,<iw>,<g>,<ob>,<ow>",
                desc:
                "Adjusts levels of lightness.",
                paramDesc:
                "<ib> is input black 0..1 (0), <iw> is input white 0..1 (1), <g> is gamma 0.01..9.99 (1), <ob> is output black 0..1 (0), <ow> is output white 0..1 (1)");

            CliArgs.Register(new List<string> {"-lev", "--levels-value"}, LevelsValue, 5,
                paramList: "=<ib>,<iw>,<g>,<ob>,<ow>",
                desc:
                "Adjusts levels of value.",
                paramDesc:
                "<ib> is input black 0..1 (0), <iw> is input white 0..1 (1), <g> is gamma 0.01..9.99 (1), <ob> is output black 0..1 (0), <ow> is output white 0..1 (1)");


            CliArgs.Register(new List<string> {"-les", "--levels-saturation"}, LevelsHslSaturation, 5, 5,
                paramList: "=<ib>,<iw>,<g>,<ob>,<ow>",
                desc: "Adjusts levels of saturation.",
                paramDesc:
                "<ib> is input black 0..1 (0), <iw> is input white (1), <g> is gamma 0.01..9.99 (1), <ob> is output black 0..1 (0), <ow> is output white 0..1 (1)");

            CliArgs.Register(new List<string> {"-i", "--invert-rgb"}, InvertRgb, 0, 0,
                desc: "Inverts RGB channels.");

            CliArgs.Register(new List<string> {"-il", "--invert-lightness"}, InvertLightness, 0, 0,
                desc: "Inverts lightness.");

            CliArgs.Register(new List<string> {"-iv", "--invert-value"}, InvertValue, 0, 0,
                desc: "Inverts value.");

            CliArgs.Register(new List<string> {"-ipl", "--invert-perc-lightness"}, InvertPerceivedLightness, 0, 0,
                desc: "Inverts perceived lightness.");

            CliArgs.Register(new List<string> {"-gsb", "--grayscale-brightness"}, BrightnessToGrayScale, 0, 0,
                desc: "Converts to gray scale based on perceived brightness.");

            CliArgs.Register(new List<string> {"-gsl", "--grayscale-ligthness"}, LightnessToGrayScale, 0, 0,
                desc: "Converts to gray scale based on perceived brightness.");

            CliArgs.Register(new List<string> {"-gsv", "--grayscale-value"}, ValueToGrayScale, 0, 0,
                desc: "Converts to gray scale based on perceived brightness.");

            CliArgs.Register(new List<string> {"--min-lightness"}, MinLightness, 1, 1,
                paramList: "=<min>",
                desc: "Limits lower end of lightness.",
                paramDesc: "<min> minimum lightness"
                );

            CliArgs.Register(new List<string> {"--max-lightness"}, MaxLightness, 1, 1,
                paramList: "=<max>",
                desc: "Limits higher end of lightness.",
                paramDesc: "<max> max lightness"
            );
 
            CliArgs.Register(new List<string> {"--min-value"}, MinValue, 1, 1,
                paramList: "=<min>",
                desc: "Limits lower end of HSV value.",
                paramDesc: "<min> minimum lightness"
            );

            CliArgs.Register(new List<string> {"--max-value"}, MaxValue, 1, 1,
                paramList: "=<max>",
                desc: "Limits higher end of value.",
                paramDesc: "<max> max HSV value"
            );
 
            CliArgs.Register(new List<string> {"--max-saturation"}, MaxHslSaturation, 1, 1,
                paramList: "=<max>",
                desc: "Limits higher end of saturation.",
                paramDesc: "<max> max saturation"
            );
            
            CliArgs.Register(new List<string> {"--max-saturation-hsv"}, MaxHsvSaturation, 1, 1,
                paramList: "=<max>",
                desc: "Limits higher end of HSV saturation.",
                paramDesc: "<max> max saturation"
            );
            
            CliArgs.Register(new List<string> {"--clamp"}, Clamp, 0, 0,
                desc:
                "Clamps color values to normal colorRange of 0..1. Tries to preserve hue. This is automatically done as last filter.");

            GetInstance()._isRegistered = true;
        }

        #region "Invert"

        public static IEnumerable<Color> InvertRgb(IEnumerable<Color> colors, ColorRange colorRange = null,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                var inverted = Color.FromRgb(
                    ColorMath.Invert(color.Red),
                    ColorMath.Invert(color.Green),
                    ColorMath.Invert(color.Blue),
                    color.Alpha);
                yield return color.InterpolateWith(inverted, rangeFactor);
            }
        }

        public static IEnumerable<Color> InvertLightness(IEnumerable<Color> colors, ColorRange colorRange = null,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);

                var filtered = new Color(color);

                filtered.Lightness = ColorMath.Invert(filtered.Lightness);

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }

        public static IEnumerable<Color> InvertValue(IEnumerable<Color> colors, ColorRange colorRange = null,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                var filtered = new Color(color);

                filtered.Value = ColorMath.Invert(color.Value);

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }

        public static IEnumerable<Color> InvertPerceivedLightness(IEnumerable<Color> colors,
            ColorRange colorRange = null,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);

                var brightness = ColorMath.RgbPerceivedBrightness(color.Red, color.Green, color.Blue);
                var targetBrightness = (1 - brightness);

                // using brightness as lightness is not accurate but we can correct this later
                // how ever it seems that completely correct value produces worse outcome
                // so we may use something in between
                var inverted = Color.FromHsl(color.Hue, color.Saturation, targetBrightness, color.Alpha);

                yield return color.InterpolateWith(inverted, rangeFactor);
            }
        }

        #endregion

        #region "Gain"

        public static IEnumerable<Color> GainHslSaturation(IEnumerable<Color> colors, ColorRange colorRange = null,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                var filtered = new Color(color);

                if (filterParams.Any()) {
                    double gain = filterParams[0];
                    filtered.Saturation = (filtered.Saturation * gain);
                }

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }

        public static IEnumerable<Color> GainRgb(IEnumerable<Color> colors, ColorRange colorRange = null,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                var filtered = new Color(color);

                if (filterParams.Any()) {
                    double gain = filterParams[0];
                    filtered.Red = ColorMath.Gain(color.Red, gain);
                    filtered.Green = ColorMath.Gain(color.Green, gain);
                    filtered.Blue = ColorMath.Gain(color.Blue, gain);
                }

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }

        public static IEnumerable<Color> GainLightness(IEnumerable<Color> colors, ColorRange colorRange = null,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                var filtered = new Color(color);

                if (filterParams.Any()) {
                    double gain = filterParams[0];
                    filtered.Lightness = (filtered.Lightness * gain);
                }

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }

        public static IEnumerable<Color> GainValue(IEnumerable<Color> colors, ColorRange colorRange = null,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                var filtered = new Color(color);

                if (filterParams.Any()) {
                    double gain = filterParams[0];
                    filtered.Value = (filtered.Value * gain);
                }

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }

        #endregion

        #region "Gamma"

        public static IEnumerable<Color> GammaRgb(IEnumerable<Color> colors, ColorRange colorRange = null,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                var filtered = new Color(color);

                if (filterParams.Any()) {
                    double gamma = filterParams[0];
                    filtered.Red = ColorMath.Gamma(color.Red, gamma);
                    filtered.Green = ColorMath.Gamma(color.Green, gamma);
                    filtered.Blue = ColorMath.Gamma(color.Blue, gamma);
                }

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }

        public static IEnumerable<Color> GammaRed(IEnumerable<Color> colors, ColorRange colorRange = null,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                var filtered = new Color(color);

                if (filterParams.Any()) {
                    double gain = filterParams[0];
                    filtered.Red = ColorMath.Gamma(color.Red, gain);
                }

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }

        public static IEnumerable<Color> GammaGreen(IEnumerable<Color> colors, ColorRange colorRange = null,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                var filtered = new Color(color);


                if (filterParams.Any()) {
                    double gain = filterParams[0];
                    filtered.Green = ColorMath.Gamma(color.Green, gain);
                }

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }

        public static IEnumerable<Color> GammaBlue(IEnumerable<Color> colors, ColorRange colorRange = null,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                var filtered = new Color(color);

                if (filterParams.Any()) {
                    double gain = filterParams[0];
                    filtered.Blue = ColorMath.Gamma(color.Blue, gain);
                }

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }

        public static IEnumerable<Color> GammaHslSaturation(IEnumerable<Color> colors, ColorRange colorRange = null,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                var filtered = new Color(color);

                if (filterParams.Any()) {
                    double gamma = filterParams[0];
                    filtered.Saturation = ColorMath.Gamma(color.Saturation, gamma);
                }

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }

        public static IEnumerable<Color> GammaLightness(IEnumerable<Color> colors, ColorRange colorRange = null,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                var filtered = new Color(color);

                if (filterParams.Any()) {
                    double gamma = filterParams[0];
                    filtered.Lightness = ColorMath.Gamma(color.Lightness, gamma);
                }

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }

        public static IEnumerable<Color> GammaValue(IEnumerable<Color> colors, ColorRange colorRange = null,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                var filtered = new Color(color);

                if (filterParams.Any()) {
                    double gamma = filterParams[0];
                    filtered.Value = ColorMath.Gamma(color.Value, gamma);
                }

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }

        #endregion

        #region "Contrast"

        public static IEnumerable<Color> ContrastRgb(IEnumerable<Color> colors, ColorRange colorRange = null,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                var filtered = new Color(color);

                if (filterParams.Any()) {
                    double strength = filterParams[0];
                    double midpoint = filterParams.Length >= 2 ? filterParams[1] : 0.5;

                    filtered.Red = ColorMath.SSpline(color.Red, strength, midpoint);
                    filtered.Green = ColorMath.SSpline(color.Green, strength, midpoint);
                    filtered.Blue = ColorMath.SSpline(color.Blue, strength, midpoint);
                }

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }

        public static IEnumerable<Color> ContrastLightness(IEnumerable<Color> colors, ColorRange colorRange = null,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                var filtered = new Color(color);

                if (filterParams.Any()) {
                    double strength = filterParams[0];
                    double midpoint = filterParams.Length >= 2 ? filterParams[1] : 0.5;

                    filtered.Lightness = ColorMath.SSpline(color.Lightness, strength, midpoint);
                }

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }

        public static IEnumerable<Color> ContrastValue(IEnumerable<Color> colors, ColorRange colorRange = null,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                var filtered = new Color(color);

                if (filterParams.Any()) {
                    double strength = filterParams[0];
                    double midpoint = filterParams.Length >= 2 ? filterParams[1] : 0.5;

                    filtered.Value = ColorMath.SSpline(color.Value, strength, midpoint);
                }

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }

        public static IEnumerable<Color> ContrastHslSaturation(IEnumerable<Color> colors,
            ColorRange colorRange,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                var filtered = new Color(color);

                if (filterParams.Any()) {
                    double strength = filterParams[0];
                    double midpoint = filterParams.Length >= 2 ? filterParams[1] : 0.5;

                    filtered.Saturation = ColorMath.SSpline(color.Saturation, strength, midpoint);
                }

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }

        #endregion

        #region Hue

        public static IEnumerable<Color> ShiftHslHue(IEnumerable<Color> colors, ColorRange colorRange = null,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                var filtered = new Color(color);


                if (filterParams.Any()) {
                    double hueShift = filterParams[0];
                    filtered.Hue = color.Hue + hueShift;
                }

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }

        #endregion

        #region "Levels"

        public static IEnumerable<Color> LevelsRgb(IEnumerable<Color> colors, ColorRange colorRange = null,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                var filtered = new Color(color);

                (double inBlack, double inWhite, double gamma, double outBlack, double outWhite) =
                    FilterUtils.GetLevelsParameters(filterParams);

                filtered.Red = ColorMath.Levels(color.Red, inBlack, inWhite, gamma, outBlack, outWhite);
                filtered.Green = ColorMath.Levels(color.Green, inBlack, inWhite, gamma, outBlack, outWhite);
                filtered.Blue = ColorMath.Levels(color.Blue, inBlack, inWhite, gamma, outBlack, outWhite);

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }

        public static IEnumerable<Color> LevelsRed(IEnumerable<Color> colors, ColorRange colorRange = null,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                var filtered = new Color(color);

                (double inBlack, double inWhite, double gamma, double outBlack, double outWhite) =
                    FilterUtils.GetLevelsParameters(filterParams);

                filtered.Red = ColorMath.Levels(color.Red, inBlack, inWhite, gamma, outBlack, outWhite);

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }

        public static IEnumerable<Color> LevelsGreen(IEnumerable<Color> colors, ColorRange colorRange = null,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                var filtered = new Color(color);

                (double inBlack, double inWhite, double gamma, double outBlack, double outWhite) =
                    FilterUtils.GetLevelsParameters(filterParams);

                filtered.Green = ColorMath.Levels(color.Green, inBlack, inWhite, gamma, outBlack, outWhite);

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }

        public static IEnumerable<Color> LevelsBlue(IEnumerable<Color> colors, ColorRange colorRange = null,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                var filtered = new Color(color);

                (double inBlack, double inWhite, double gamma, double outBlack, double outWhite) =
                    FilterUtils.GetLevelsParameters(filterParams);

                filtered.Blue = ColorMath.Levels(color.Blue, inBlack, inWhite, gamma, outBlack, outWhite);

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }

        public static IEnumerable<Color> AutoLevelsRgb(IEnumerable<Color> colors, ColorRange colorRange = null,
            params double[] filterParams)
        {        
            // This is to avoid multiple enumeration
            List<Color> cache = colors.ToList();
            
            (double inBlack, double inWhite) = FilterUtils.GetLowestAndHighestValue(cache);

#if DEBUG
            Console.WriteLine($"  (Auto rgb levels - source min {inBlack:F3}, max {inWhite:F3})");
#endif

            foreach (var color in cache) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                var filtered = new Color(color);

                (double outBlack, double outWhite, double gamma) =
                    FilterUtils.GetAutoLevelParameters(filterParams);

                filtered.Red = ColorMath.Levels(color.Red, inBlack, inWhite, gamma, outBlack, outWhite);
                filtered.Green = ColorMath.Levels(color.Green, inBlack, inWhite, gamma, outBlack, outWhite);
                filtered.Blue = ColorMath.Levels(color.Blue, inBlack, inWhite, gamma, outBlack, outWhite);

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }
        
        public static IEnumerable<Color> AutoLevelsLightness(IEnumerable<Color> colors, ColorRange colorRange = null,
            params double[] filterParams)
        {        
            // This is to avoid multiple enumeration
            List<Color> cache = colors.ToList();
            
            (double inBlack, double inWhite) = FilterUtils.GetLowestAndHighestLightness(cache);

#if DEBUG
            Console.WriteLine($"  (Auto lightness levels - source min {inBlack:F3}, max {inWhite:F3})");
#endif

            foreach (var color in cache) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                var filtered = new Color(color);

                (double outBlack, double outWhite, double gamma) =
                    FilterUtils.GetAutoLevelParameters(filterParams);

                filtered.Lightness = ColorMath.Levels(color.Lightness, inBlack, inWhite, gamma, outBlack, outWhite);

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }
        

        public static IEnumerable<Color> LevelsLightness(IEnumerable<Color> colors, ColorRange colorRange = null,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                var filtered = new Color(color);

                (double inBlack, double inWhite, double gamma, double outBlack, double outWhite) =
                    FilterUtils.GetLevelsParameters(filterParams);

                filtered.Lightness = ColorMath.Levels(color.Lightness, inBlack, inWhite, gamma, outBlack, outWhite);

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }

        public static IEnumerable<Color> LevelsValue(IEnumerable<Color> colors, ColorRange colorRange = null,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                var filtered = new Color(color);

                (double inBlack, double inWhite, double gamma, double outBlack, double outWhite) =
                    FilterUtils.GetLevelsParameters(filterParams);

                filtered.Value = ColorMath.Levels(color.Value, inBlack, inWhite, gamma, outBlack, outWhite);

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }


        public static IEnumerable<Color> LevelsHslSaturation(IEnumerable<Color> colors,
            ColorRange colorRange,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                var filtered = new Color(color);

                (double inBlack, double inWhite, double gamma, double outBlack, double outWhite) =
                    FilterUtils.GetLevelsParameters(filterParams);

                filtered.Saturation = ColorMath.Levels(color.Saturation, inBlack, inWhite, gamma, outBlack, outWhite);

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }

        #endregion

        #region "Limit"
        
        public static IEnumerable<Color> MaxLightness(IEnumerable<Color> colors,
            ColorRange colorRange,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                var filtered = new Color(color);
                
                if (filterParams.Any())
                    filtered.Lightness = color.Lightness.LimitHigh(filterParams[0]); 

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }
        
        public static IEnumerable<Color> MinLightness(IEnumerable<Color> colors,
            ColorRange colorRange,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                var filtered = new Color(color);
                
                if (filterParams.Any())
                    filtered.Lightness = color.Lightness.LimitLow(filterParams[0]); 

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }
        
        public static IEnumerable<Color> MaxValue(IEnumerable<Color> colors,
            ColorRange colorRange,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                var filtered = new Color(color);
                
                if (filterParams.Any())
                    filtered.Value = color.Value.LimitHigh(filterParams[0]); 

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }
        
        public static IEnumerable<Color> MinValue(IEnumerable<Color> colors,
            ColorRange colorRange,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                var filtered = new Color(color);
                
                if (filterParams.Any())
                    filtered.Value = color.Value.LimitLow(filterParams[0]); 

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }
        
        public static IEnumerable<Color> MaxHslSaturation(IEnumerable<Color> colors,
            ColorRange colorRange,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                var filtered = new Color(color);
                
                if (filterParams.Any())
                    filtered.Saturation = color.Saturation.LimitHigh(filterParams[0]); 

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }
        
        public static IEnumerable<Color> MaxHsvSaturation(IEnumerable<Color> colors,
            ColorRange colorRange,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                var filtered = new Color(color);
                
                if (filterParams.Any())
                    filtered.SaturationHsv = color.SaturationHsv.LimitHigh(filterParams[0]); 

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }
        
        #endregion
        
        #region "Misc"

        public static IEnumerable<Color> BrightnessToGrayScale(IEnumerable<Color> colors,
            ColorRange colorRange,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                var filtered = new Color(color);

                var br = ColorMath.RgbPerceivedBrightness(color.Red, color.Green, color.Blue);
                filtered.Red = br;
                filtered.Green = br;
                filtered.Blue = br;

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }

        public static IEnumerable<Color> LightnessToGrayScale(IEnumerable<Color> colors,
            ColorRange colorRange,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                var filtered = new Color(color);

                filtered.Red = color.Lightness;
                filtered.Green = color.Lightness;
                filtered.Blue = color.Lightness;

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }

        public static IEnumerable<Color> ValueToGrayScale(IEnumerable<Color> colors,
            ColorRange colorRange,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                var filtered = new Color(color);

                filtered.Red = color.Value;
                filtered.Green = color.Value;
                filtered.Blue = color.Value;

                yield return color.InterpolateWith(filtered, rangeFactor);
            }
        }

        public static IEnumerable<Color> Clamp(IEnumerable<Color> colors, ColorRange colorRange = null,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                color.ClampExceedingColors();
                yield return color;
            }
        }

        #endregion
    }
}