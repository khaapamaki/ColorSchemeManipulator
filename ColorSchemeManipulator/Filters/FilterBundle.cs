using System.Collections.Generic;
using System.Linq;
using ColorSchemeManipulator.CLI;
using ColorSchemeManipulator.Colors;
using ColorSchemeManipulator.Common;
using ColorSchemeManipulator.Ranges;

namespace ColorSchemeManipulator.Filters
{
    // Todo Fix issue: parameter list that has comma at beginning is not properly handled causing slow processing (images)
    // arguments are empty string that will get default values but parsing with exception handling makes it slow (true anymore?)
    // Todo Better argument validation could be the answer
    // other options would be pre-parsing to correct type and not parsing again when the filter is reapplied.

    /// <summary>
    /// A set of predefined filters
    /// </summary>
    public sealed class FilterBundle
    {
        private const int DegreeOfParallelism = 2;

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

            CliArgs.Register(new CliArgBuilder()
                .Filter(ShiftHslHue)
                .AddOptions("-h", "--hue")
                .Params(1)
                .ParamString("=<offset>")
                .Description("Hue shift.")
                .ParamDescription("<offset> is hue offset in range of -360..360 (0)")
            );

            CliArgs.Register(new List<string> {"-s", "--saturation"}, GainHslSaturation, 1,
                paramList: "=<gain>",
                desc: "Saturation gain.",
                paramDesc: "<gain> is multiplier in range of 0..10 (1.0)");

            CliArgs.Register(new List<string> {"-g", "--gain"}, GainRgb, 1,
                paramList: "=<gain>",
                desc: "RGB gain.",
                paramDesc: "<gain> is multiplier in range of 0..10 (1.0)");

            CliArgs.Register(new List<string> {"-l", "--lightness"}, GainLightness, 1,
                paramList: "=<gain>",
                desc: "Lightness gain.",
                paramDesc: "<gain> is multiplier in range of 0..10 (1.0)");

            CliArgs.Register(new List<string> {"-v", "--value"}, GainValue, 1,
                paramList: "=<gain>",
                desc: "Value gain.",
                paramDesc: "<gain> is multiplier in range of 0..10 (1.0)");

            CliArgs.Register(new List<string> {"-c", "--contrast"}, ContrastRgb, 1, 2,
                paramList: "=<contrast>[,<ip>]",
                desc: "Adjusts contrast by S-spline curve.",
                paramDesc:
                "<contrast> is curvature strength in range of -1..1 (0.0), <ip> is inflection point in range of 0..1 (0.5)");

            CliArgs.Register(new List<string> {"-cl", "--contrast-lightness"}, ContrastLightness, 1, 2,
                paramList: "=<contrast>[,<ip>]",
                desc:
                "Applies contrast curve to lightness.",
                paramDesc:
                "<contrast> is curvature strength in range of -1..1 (0), <ip> is inflection point in range of 0..1 (0.5)");

            CliArgs.Register(new List<string> {"-cv", "--contrast-value"}, ContrastValue, 1, 2,
                paramList: "=<contrast>[,<ip>]",
                desc:
                "Applies contrast curve to value.",
                paramDesc:
                "<contrast> is curvature strength in range of -1..1 (0), <ip> is inflection point in range of 0..1 (0.5)");

            CliArgs.Register(new List<string> {"-cs", "--contrast-saturation"}, ContrastHslSaturation, 1, 2,
                paramList: "=<contrast>[,<ip>]",
                desc:
                "Applies contrast curve to saturation.",
                paramDesc:
                "<contrast> is curvature strength in range of -1..1 (0), <ip> is inflection point in range of 0..1 (0.5)");

            CliArgs.Register(new List<string> {"-ga", "--gamma"}, GammaRgb, 1, 1,
                paramList: "=<gamma>",
                desc: "Gamma correction for all RGB channels equally.",
                paramDesc: "<gamma> is value in range of 0.01..9.99 (1.0)");

            CliArgs.Register(new List<string> {"-gar", "--gamma-red"}, GammaRed, 1,
                paramList: "=<gamma>",
                desc: "Adjusts gamma of red channel.",
                paramDesc: "<gamma> is value in range of 0.01..9.99 (1.0)");

            CliArgs.Register(new List<string> {"-gag", "--gamma-green"}, GammaGreen, 1,
                paramList: "=<gamma>",
                desc: "Adjusts gamma of green channel.",
                paramDesc: "<gamma> is value in range of 0.01..9.99 (1.0)");

            CliArgs.Register(new List<string> {"-gab", "--gamma-blue"}, GammaBlue, 1,
                paramList: "=<gamma>",
                desc: "Adjusts gamma of blue channel.",
                paramDesc: "<gamma> is value in range of 0.01..9.99 (1.0)");

            CliArgs.Register(new List<string> {"-gal", "--gamma-lightness"}, GammaLightness, 1,
                paramList: "=<gamma>",
                desc: "Adjusts gamma of lightness.",
                paramDesc: "<gamma> is value in range of 0.01..9.99 (1.0)");

            CliArgs.Register(new List<string> {"-gav", "--gamma-value"}, GammaValue, 1,
                paramList: "=<gamma>",
                desc: "Adjusts gamma of value.",
                paramDesc: "<gamma> is value in range of 0.01..9.99 (1.0)");

            CliArgs.Register(new List<string> {"-gas", "--gamma-saturation"}, GammaHslSaturation, 1,
                paramList: "=<gamma>",
                desc: "Adjusts gamma of saturation.",
                paramDesc: "<gamma> is value in range of 0.01..9.99 (1.0)");

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

            CliArgs.Register(new List<string> {"-ipl", "--invert-per-lightness"}, InvertPerceivedLightness, 0, 0,
                desc: "Inverts perceived lightness.");

            CliArgs.Register(new List<string> {"-gsb", "--grayscale-brightness"}, BrightnessToGrayScale, 0, 0,
                desc: "Converts to gray scale based on perceived brightness.");

            CliArgs.Register(new List<string> {"-gsl", "--grayscale-ligthness"}, LightnessToGrayScale, 0, 0,
                desc: "Converts to gray scale based on lightness.");

            CliArgs.Register(new List<string> {"-gsv", "--grayscale-value"}, ValueToGrayScale, 0, 0,
                desc: "Converts to gray scale based on HSV value.");

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
                "Clamps color values to normal range of 0..1. Tries to preserve hue. This is automatically applied as last filter.");

            GetInstance()._isRegistered = true;
        }

        public static Color InvertRgb1(
            Color color,
            ColorRange colorRange = null,
            params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var inverted = Color.FromRgb(
                ColorMath.Invert(color.Red),
                ColorMath.Invert(color.Green),
                ColorMath.Invert(color.Blue),
                color.Alpha);
            color.InterpolateWith(inverted, rangeFactor);

            return color;
        }

        #region "Invert"

        public static Color InvertRgb(Color color, ColorRange colorRange, params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var inverted = Color.FromRgb(
                ColorMath.Invert(color.Red),
                ColorMath.Invert(color.Green),
                ColorMath.Invert(color.Blue),
                color.Alpha);
            color.InterpolateWith(inverted, rangeFactor);

            return color;
        }

        public static Color InvertLightness(Color color, ColorRange colorRange, params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var filtered = new Color(color);
            filtered.Lightness = ColorMath.Invert(filtered.Lightness);
            color.InterpolateWith(filtered, rangeFactor);
            return color;
        }

        public static Color InvertValue(Color color, ColorRange colorRange, params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var filtered = new Color(color);
            filtered.Value = ColorMath.Invert(color.Value);
            color.InterpolateWith(filtered, rangeFactor);
            return color;
        }

        public static Color InvertPerceivedLightness(Color color,
            ColorRange colorRange = null,
            params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var brightness = ColorMath.RgbPerceivedBrightness(color.Red, color.Green, color.Blue);
            var targetBrightness = (1 - brightness);

            // using brightness as lightness is not accurate but we can correct this later
            // how ever it seems that completely correct value produces worse outcome
            // so we may use something in between
            var inverted = Color.FromHsl(color.Hue, color.Saturation, targetBrightness, color.Alpha);
            color.InterpolateWith(inverted, rangeFactor);
            return color;
        }

        #endregion

        #region "Gain"

        public static Color GainHslSaturation(Color color, ColorRange colorRange, params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var filtered = new Color(color);

            if (filterParams.Any()) {
                double gain = filterParams[0];
                filtered.Saturation = (filtered.Saturation * gain);
            }

            color.InterpolateWith(filtered, rangeFactor);
            return color;
        }

        public static Color GainRgb(Color color, ColorRange colorRange, params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var filtered = new Color(color);

            if (filterParams.Any()) {
                double gain = filterParams[0];
                filtered.Red = ColorMath.Gain(color.Red, gain);
                filtered.Green = ColorMath.Gain(color.Green, gain);
                filtered.Blue = ColorMath.Gain(color.Blue, gain);
            }

            color.InterpolateWith(filtered, rangeFactor);
            return color;
        }

        public static Color GainLightness(Color color, ColorRange colorRange, params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var filtered = new Color(color);

            if (filterParams.Any()) {
                double gain = filterParams[0];
                filtered.Lightness = (filtered.Lightness * gain);
            }

            color.InterpolateWith(filtered, rangeFactor);
            return color;
        }

        public static Color GainValue(Color color, ColorRange colorRange, params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var filtered = new Color(color);
            if (filterParams.Any()) {
                double gain = filterParams[0];
                filtered.Value = (filtered.Value * gain);
            }

            color.InterpolateWith(filtered, rangeFactor);
            return color;
        }

        #endregion

        #region "Gamma"

        public static Color GammaRgb(Color color, ColorRange colorRange, params double[] filterParams)
        {
            if (filterParams.Any()) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                var filtered = new Color(color);
                double gamma = filterParams[0];
                filtered.Red = ColorMath.Gamma(color.Red, gamma);
                filtered.Green = ColorMath.Gamma(color.Green, gamma);
                filtered.Blue = ColorMath.Gamma(color.Blue, gamma);
                color.InterpolateWith(filtered, rangeFactor);
            }

            return color;
        }

        public static Color GammaRed(Color color, ColorRange colorRange, params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var filtered = new Color(color);
            if (filterParams.Any()) {
                double gain = filterParams[0];
                filtered.Red = ColorMath.Gamma(color.Red, gain);
            }

            color.InterpolateWith(filtered, rangeFactor);
            return color;
        }

        public static Color GammaGreen(Color color, ColorRange colorRange, params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var filtered = new Color(color);
            if (filterParams.Any()) {
                double gain = filterParams[0];
                filtered.Green = ColorMath.Gamma(color.Green, gain);
            }

            color.InterpolateWith(filtered, rangeFactor);
            return color;
        }

        public static Color GammaBlue(Color color, ColorRange colorRange, params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var filtered = new Color(color);
            if (filterParams.Any()) {
                double gain = filterParams[0];
                filtered.Blue = ColorMath.Gamma(color.Blue, gain);
            }

            color.InterpolateWith(filtered, rangeFactor);
            return color;
        }

        public static Color GammaHslSaturation(Color color, ColorRange colorRange, params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var filtered = new Color(color);
            if (filterParams.Any()) {
                double gamma = filterParams[0];
                filtered.Saturation = ColorMath.Gamma(color.Saturation, gamma);
            }

            color.InterpolateWith(filtered, rangeFactor);
            return color;
        }

        public static Color GammaLightness(Color color, ColorRange colorRange, params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var filtered = new Color(color);
            if (filterParams.Any()) {
                double gamma = filterParams[0];
                filtered.Lightness = ColorMath.Gamma(color.Lightness, gamma);
            }

            color.InterpolateWith(filtered, rangeFactor);
            return color;
        }

        public static Color GammaValue(Color color, ColorRange colorRange, params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var filtered = new Color(color);
            if (filterParams.Any()) {
                double gamma = filterParams[0];
                filtered.Value = ColorMath.Gamma(color.Value, gamma);
            }

            color.InterpolateWith(filtered, rangeFactor);
            return color;
        }

        #endregion

        #region "Contrast"

        public static Color ContrastRgb(Color color, ColorRange colorRange, params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var filtered = new Color(color);

            if (filterParams.Any()) {
                double strength = filterParams[0];
                double midpoint = filterParams.Length >= 2 ? filterParams[1] : 0.5;
                filtered.Red = ColorMath.SSpline(color.Red, strength, midpoint);
                filtered.Green = ColorMath.SSpline(color.Green, strength, midpoint);
                filtered.Blue = ColorMath.SSpline(color.Blue, strength, midpoint);
            }

            color.InterpolateWith(filtered, rangeFactor);
            return color;
        }

        public static Color ContrastLightness(Color color, ColorRange colorRange, params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var filtered = new Color(color);

            if (filterParams.Any()) {
                double strength = filterParams[0];
                double midpoint = filterParams.Length >= 2 ? filterParams[1] : 0.5;
                filtered.Lightness = ColorMath.SSpline(color.Lightness, strength, midpoint);
            }

            color.InterpolateWith(filtered, rangeFactor);
            return color;
        }

        public static Color ContrastValue(Color color, ColorRange colorRange, params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var filtered = new Color(color);

            if (filterParams.Any()) {
                double strength = filterParams[0];
                double midpoint = filterParams.Length >= 2 ? filterParams[1] : 0.5;
                filtered.Value = ColorMath.SSpline(color.Value, strength, midpoint);
            }

            color.InterpolateWith(filtered, rangeFactor);
            return color;
        }

        public static Color ContrastHslSaturation(Color color,
            ColorRange colorRange,
            params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var filtered = new Color(color);

            if (filterParams.Any()) {
                double strength = filterParams[0];
                double midpoint = filterParams.Length >= 2 ? filterParams[1] : 0.5;
                filtered.Saturation = ColorMath.SSpline(color.Saturation, strength, midpoint);
            }

            color.InterpolateWith(filtered, rangeFactor);
            return color;
        }

        #endregion

        #region Hue

        public static Color ShiftHslHue(Color color, ColorRange colorRange, params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var filtered = new Color(color);

            if (filterParams.Any()) {
                double hueShift = filterParams[0];
                filtered.Hue = color.Hue + hueShift;
            }

            color.InterpolateWith(filtered, rangeFactor);
            return color;
        }

        #endregion

        #region "Levels"

        public static Color LevelsRgb(Color color, ColorRange colorRange, params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var filtered = new Color(color);

            (double inBlack, double inWhite, double gamma, double outBlack, double outWhite) =
                FilterUtils.GetLevelsParameters(filterParams);

            filtered.Red = ColorMath.Levels(color.Red, inBlack, inWhite, gamma, outBlack, outWhite);
            filtered.Green = ColorMath.Levels(color.Green, inBlack, inWhite, gamma, outBlack, outWhite);
            filtered.Blue = ColorMath.Levels(color.Blue, inBlack, inWhite, gamma, outBlack, outWhite);

            color.InterpolateWith(filtered, rangeFactor);
            return color;
        }

        public static Color LevelsRed(Color color, ColorRange colorRange, params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var filtered = new Color(color);
            (double inBlack, double inWhite, double gamma, double outBlack, double outWhite) =
                FilterUtils.GetLevelsParameters(filterParams);
            filtered.Red = ColorMath.Levels(color.Red, inBlack, inWhite, gamma, outBlack, outWhite);

            color.InterpolateWith(filtered, rangeFactor);
            return color;
        }

        public static Color LevelsGreen(Color color, ColorRange colorRange, params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var filtered = new Color(color);
            (double inBlack, double inWhite, double gamma, double outBlack, double outWhite) =
                FilterUtils.GetLevelsParameters(filterParams);
            filtered.Green = ColorMath.Levels(color.Green, inBlack, inWhite, gamma, outBlack, outWhite);

            color.InterpolateWith(filtered, rangeFactor);
            return color;
        }

        public static Color LevelsBlue(Color color, ColorRange colorRange, params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var filtered = new Color(color);
            (double inBlack, double inWhite, double gamma, double outBlack, double outWhite) =
                FilterUtils.GetLevelsParameters(filterParams);
            filtered.Blue = ColorMath.Levels(color.Blue, inBlack, inWhite, gamma, outBlack, outWhite);

            color.InterpolateWith(filtered, rangeFactor);
            return color;
        }

        public static IEnumerable<Color> AutoLevelsRgb(IEnumerable<Color> colors, ColorRange colorRange,
            params double[] filterParams)
        {
            // This is to avoid multiple enumeration
            List<Color> cache = colors.ToList();
            (double inBlack, double inWhite) = FilterUtils.GetLowestAndHighestValue(cache);
#if DEBUG
            Console.WriteLine($"  (Auto rgb levels - source min {inBlack:F3}, max {inWhite:F3})");
#endif

            IEnumerable<Color> result;
            if (DegreeOfParallelism > 0) {
                result = cache
                    .AsParallel()
                    .AsOrdered()
                    .WithDegreeOfParallelism(DegreeOfParallelism);
            } else {
                result = cache;
            }

            result = result
                .Select(
                    color =>
                    {
                        var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                        var filtered = new Color(color);
                        (double outBlack, double outWhite, double gamma) =
                            FilterUtils.GetAutoLevelParameters(filterParams);
                        filtered.Red = ColorMath.Levels(color.Red, inBlack, inWhite, gamma, outBlack, outWhite);
                        filtered.Green = ColorMath.Levels(color.Green, inBlack, inWhite, gamma, outBlack, outWhite);
                        filtered.Blue = ColorMath.Levels(color.Blue, inBlack, inWhite, gamma, outBlack, outWhite);

                        color.InterpolateWith(filtered, rangeFactor);
                        return color;
                    });

            foreach (var color in result) {
                yield return color;
            }
        }

        public static IEnumerable<Color> AutoLevelsLightness(IEnumerable<Color> colors, ColorRange colorRange,
            params double[] filterParams)
        {
            // This is to avoid multiple enumeration
            List<Color> cache = colors.ToList();
            (double inBlack, double inWhite) = FilterUtils.GetLowestAndHighestLightness(cache);
#if DEBUG
            Console.WriteLine($"  (Auto lightness levels - source min {inBlack:F3}, max {inWhite:F3})");
#endif

            IEnumerable<Color> result;

            if (DegreeOfParallelism > 0) {
                result = cache
                    .AsParallel()
                    .AsOrdered()
                    .WithDegreeOfParallelism(DegreeOfParallelism);
            } else {
                result = cache;
            }

            result = result
                .Select(
                    color =>
                    {
                        var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                        var filtered = new Color(color);
                        (double outBlack, double outWhite, double gamma) =
                            FilterUtils.GetAutoLevelParameters(filterParams);
                        filtered.Lightness =
                            ColorMath.Levels(color.Lightness, inBlack, inWhite, gamma, outBlack, outWhite);
                        color.InterpolateWith(filtered, rangeFactor);
                        return color;
                    });

            foreach (var color in result) {
                yield return color;
            }
        }

        public static Color LevelsLightness(Color color, ColorRange colorRange, params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var filtered = new Color(color);
            (double inBlack, double inWhite, double gamma, double outBlack, double outWhite) =
                FilterUtils.GetLevelsParameters(filterParams);
            filtered.Lightness = ColorMath.Levels(color.Lightness, inBlack, inWhite, gamma, outBlack, outWhite);
            color.InterpolateWith(filtered, rangeFactor);
            return color;
        }

        public static Color LevelsValue(Color color, ColorRange colorRange, params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var filtered = new Color(color);
            (double inBlack, double inWhite, double gamma, double outBlack, double outWhite) =
                FilterUtils.GetLevelsParameters(filterParams);
            filtered.Value = ColorMath.Levels(color.Value, inBlack, inWhite, gamma, outBlack, outWhite);
            color.InterpolateWith(filtered, rangeFactor);
            return color;
        }

        public static Color LevelsHslSaturation(Color color,
            ColorRange colorRange,
            params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var filtered = new Color(color);

            (double inBlack, double inWhite, double gamma, double outBlack, double outWhite) =
                FilterUtils.GetLevelsParameters(filterParams);

            filtered.Saturation =
                ColorMath.Levels(color.Saturation, inBlack, inWhite, gamma, outBlack, outWhite);

            color.InterpolateWith(filtered, rangeFactor);
            return color;
        }

        #endregion

        #region "Limit"

        public static Color MaxLightness(Color color,
            ColorRange colorRange,
            params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var filtered = new Color(color);
            if (filterParams.Any()) {
                filtered.Lightness = color.Lightness.LimitHigh(filterParams[0]);
            }

            color.InterpolateWith(filtered, rangeFactor);
            return color;
        }

        public static Color MinLightness(Color color,
            ColorRange colorRange,
            params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var filtered = new Color(color);
            if (filterParams.Any()) {
                filtered.Lightness = color.Lightness.LimitLow(filterParams[0]);
            }

            color.InterpolateWith(filtered, rangeFactor);
            return color;
        }

        public static Color MaxValue(Color color,
            ColorRange colorRange,
            params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var filtered = new Color(color);
            if (filterParams.Any()) {
                filtered.Value = color.Value.LimitHigh(filterParams[0]);
            }

            color.InterpolateWith(filtered, rangeFactor);
            return color;
        }

        public static Color MinValue(Color color,
            ColorRange colorRange,
            params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var filtered = new Color(color);

            if (filterParams.Any()) {
                filtered.Value = color.Value.LimitLow(filterParams[0]);
            }

            color.InterpolateWith(filtered, rangeFactor);
            return color;
        }

        public static Color MaxHslSaturation(Color color,
            ColorRange colorRange,
            params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var filtered = new Color(color);

            if (filterParams.Any()) {
                filtered.Saturation = color.Saturation.LimitHigh(filterParams[0]);
            }

            color.InterpolateWith(filtered, rangeFactor);
            return color;
        }

        public static Color MaxHsvSaturation(Color color,
            ColorRange colorRange,
            params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var filtered = new Color(color);

            if (filterParams.Any()) {
                filtered.SaturationHsv = color.SaturationHsv.LimitHigh(filterParams[0]);
            }

            color.InterpolateWith(filtered, rangeFactor);
            return color;
        }

        #endregion

        #region "Misc"

        public static Color BrightnessToGrayScale(Color color,
            ColorRange colorRange,
            params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var filtered = new Color(color);
            var br = ColorMath.RgbPerceivedBrightness(color.Red, color.Green, color.Blue);
            filtered.Red = br;
            filtered.Green = br;
            filtered.Blue = br;
            color.InterpolateWith(filtered, rangeFactor);
            return color;
        }

        public static Color LightnessToGrayScale(Color color,
            ColorRange colorRange,
            params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var filtered = new Color(color);
            filtered.Red = color.Lightness;
            filtered.Green = color.Lightness;
            filtered.Blue = color.Lightness;
            color.InterpolateWith(filtered, rangeFactor);
            return color;
        }

        public static Color ValueToGrayScale(Color color,
            ColorRange colorRange,
            params double[] filterParams)
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var filtered = new Color(color);
            filtered.Red = color.Value;
            filtered.Green = color.Value;
            filtered.Blue = color.Value;
            color.InterpolateWith(filtered, rangeFactor);
            return color;
        }

        public static Color Clamp(Color color, ColorRange colorRange, params double[] filterParams)
        {
            color.ClampExceedingColors();
            return color;
        }

        #endregion
    }
}