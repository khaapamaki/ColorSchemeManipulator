using System.Text;
using ColorSchemeManipulator.Colors;

namespace ColorSchemeManipulator.Filters
{
    public class ColorRange
    {
        public ColorRange() { }

        public ParameterRange SaturationRange { get; set; } = null;
        public ParameterRange LightnessRange { get; set; } = null;
        public ParameterRange ValueRange { get; set; } = null;
        public ParameterRange RedRange { get; set; } = null;
        public ParameterRange GreenRange { get; set; } = null;
        public ParameterRange BlueRange { get; set; } = null;
        public ParameterRange HueRange { get; set; } = null;
        public ParameterRange BrightnessRange { get; set; } = null;

        public double InRangeFactor(Color color)
        {
            double result = 1.0;
            result *= RedRange?.InRangeFactor(color.Red) ?? 1;
            result *= GreenRange?.InRangeFactor(color.Green) ?? 1;
            result *= BlueRange?.InRangeFactor(color.Blue) ?? 1;
            result *= LightnessRange?.InRangeFactor(color.Lightness) ?? 1;
            result *= SaturationRange?.InRangeFactor(color.Saturation) ?? 1;
            result *= HueRange?.InRangeFactor(color.Hue) ?? 1;
            result *= ValueRange?.InRangeFactor(color.Value) ?? 1;
            result *= BrightnessRange?.InRangeFactor(color.GetBrightness()) ?? 1;
            return result;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(BrightnessRange != null ? $"Bri: " + BrightnessRange : "");
            sb.Append(HueRange != null ? $"{(sb.Length > 0 ? ", " : "")}Hue: " + HueRange : "");
            sb.Append(SaturationRange != null ? $"{(sb.Length > 0 ? ", " : "")}Sat: " + SaturationRange : "");
            sb.Append(LightnessRange != null ? $"{(sb.Length > 0 ? ", " : "")}Lgh: " + LightnessRange : "");
            sb.Append(RedRange != null ? $"{(sb.Length > 0 ? ", " : "")}Red: " + RedRange : "");
            sb.Append(GreenRange != null ? $"{(sb.Length > 0 ? ", " : "")}Gre: " + GreenRange: "");
            sb.Append(BlueRange != null ? $"{(sb.Length > 0 ? ", " : "")}Blu: " + BlueRange : "");

            return sb.ToString();
        }

        public ColorRange Brightness(double min, double max, double minSlope = 0, double maxSlope = 0)
        {
            BrightnessRange = ParameterRange.Range(min, max, minSlope, maxSlope);
            return this;
        }

        public ColorRange Brightness4P(double minStart, double minEnd, double maxStart, double maxEnd)
        {
            BrightnessRange = ParameterRange.FourPointRange(minStart, minEnd, maxStart, maxEnd);
            return this;
        }

        public ColorRange Hue(double min, double max, double minSlope = 0, double maxSlope = 0)
        {
            HueRange = ParameterRange.Range(min, max, minSlope, maxSlope, 360);
            return this;
        }

        public ColorRange Hue4P(double minStart, double minEnd, double maxStart, double maxEnd)
        {
            HueRange = ParameterRange.FourPointRange(minStart, minEnd, maxStart, maxEnd, 360);
            return this;
        }

        public ColorRange Saturation(double min, double max, double minSlope = 0, double maxSlope = 0)
        {
            SaturationRange = ParameterRange.Range(min, max, minSlope, maxSlope);
            return this;
        }

        public ColorRange Saturation4P(double minStart, double minEnd, double maxStart, double maxEnd)
        {
            SaturationRange = ParameterRange.FourPointRange(minStart, minEnd, maxStart, maxEnd);
            return this;
        }

        public ColorRange Lightness(double min, double max, double minSlope = 0, double maxSlope = 0)
        {
            LightnessRange = ParameterRange.Range(min, max, minSlope, maxSlope);
            return this;
        }

        public ColorRange Lightness4P(double minStart, double minEnd, double maxStart, double maxEnd)
        {
            LightnessRange = ParameterRange.FourPointRange(minStart, minEnd, maxStart, maxEnd);
            return this;
        }

        public ColorRange Value(double min, double max, double minSlope = 0, double maxSlope = 0)
        {
            ValueRange = ParameterRange.Range(min, max, minSlope, maxSlope);
            return this;
        }

        public ColorRange Value4P(double minStart, double minEnd, double maxStart, double maxEnd)
        {
            ValueRange = ParameterRange.FourPointRange(minStart, minEnd, maxStart, maxEnd);
            return this;
        }

        public ColorRange Red(double min, double max, double minSlope = 0, double maxSlope = 0)
        {
            RedRange = ParameterRange.Range(min, max, minSlope, maxSlope);
            return this;
        }

        public ColorRange Red4P(double minStart, double minEnd, double maxStart, double maxEnd)
        {
            RedRange = ParameterRange.FourPointRange(minStart, minEnd, maxStart, maxEnd);
            return this;
        }

        public ColorRange Green(double min, double max, double minSlope = 0, double maxSlope = 0)
        {
            GreenRange = ParameterRange.Range(min, max, minSlope, maxSlope);
            return this;
        }

        public ColorRange Green4P(double minStart, double minEnd, double maxStart, double maxEnd)
        {
            GreenRange = ParameterRange.FourPointRange(minStart, minEnd, maxStart, maxEnd);
            return this;
        }

        public ColorRange Blue(double min, double max, double minSlope = 0, double maxSlope = 0)
        {
            BlueRange = ParameterRange.Range(min, max, minSlope, maxSlope);
            return this;
        }

        public ColorRange Blue4P(double minStart, double minEnd, double maxStart, double maxEnd)
        {
            BlueRange = ParameterRange.Range(minStart, minEnd, maxStart, maxEnd);
            return this;
        }
    }
}