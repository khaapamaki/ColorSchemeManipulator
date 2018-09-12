using System;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using ColorSchemeInverter.Colors;
using ColorSchemeInverter.Common;

namespace ColorSchemeInverter.Filters
{
    public class ColorRange
    {
        // Todo constructors
        // Todo CLI system for ranges
        // Todo implement slopes value

        public ColorRange() { }
        public ParameterRange SaturationRange { get; set; } = null;
        public ParameterRange LightnessRange { get; set; } = null;
        public ParameterRange ValueRange { get; set; } = null;
        public ParameterRange RedRange { get; set; } = null;
        public ParameterRange GreenRange { get; set; } = null;
        public ParameterRange BlueRange { get; set; } = null;
        public ParameterRange HueRange { get; set; } = null;
        public ParameterRange BrightnessRange { get; set; } = null;

        // private double _minHue = 0.0;
        // private double _maxHue = 360.0;
        // private double _minHueSlope = 0.0;
        // private double _maxHueSlope = 0.0;
        // private bool _hueRange = false;

        // public ColorRange Hue(double min, double max, double minSlope = 0.0, double maxSlope = 0.0)
        // {
        //     _minHue = min.NormalizeLoopingValue(360.0);
        //     _maxHue = max.NormalizeLoopingValue(360.0);
        //     _minHueSlope = minSlope;
        //     _maxHueSlope = maxSlope;
        //     _hueRange = true;
        //     return this;
        // }
        public double InRangeFactor(Rgb rgb)
        {
            double result = 1.0;
            if (HslOrHsvProcessingNeeded()) {
                result = HslFactors(rgb.ToHsl(), result);
            }

            if (HsvProcessingNeeded()) {
                result = HsvFactors(rgb.ToHsv(), result);
            }

            if (RgbProcessingNeeded()) {
                result = RgbFactors(rgb, result);
            }

            return result;
        }

        private double RgbFactors(Rgb rgb, double result = 1.0)
        {
            result *= RedRange?.InRangeFactor(rgb.Red) ?? 1;
            result *= GreenRange?.InRangeFactor(rgb.Green) ?? 1;
            result *= BlueRange?.InRangeFactor(rgb.Blue) ?? 1;
            result *= BrightnessRange?.InRangeFactor(
                          ColorMath.RgbPerceivedBrightness(rgb.Red, rgb.Green, rgb.Blue)) ?? 1;
            return result;
        }

        private double HsvFactors(Hsv hsv, double result = 1.0)
        {
            result *= ValueRange?.InRangeFactor(hsv.Value) ?? 1;
            return result;
        }

        private double HslFactors(Hsl hsl, double result = 1.0)
        {
            result *= HueRange?.InRangeFactor(hsl.Hue) ?? 1;
            result *= SaturationRange?.InRangeFactor(hsl.Saturation) ?? 1;
            result *= LightnessRange?.InRangeFactor(hsl.Lightness) ?? 1;
            return result;
        }

        public double InRangeFactor(Hsl hsl)
        {
            double result = 1.0;
            if (HslOrHsvProcessingNeeded()) {
                result = HslFactors(hsl, result);
            }

            if (HsvProcessingNeeded()) {
                result = HsvFactors(hsl.ToHsv(), result);
            }

            if (RgbProcessingNeeded()) {
                result = RgbFactors(hsl.ToRgb(), result);
            }

            return result;
        }


        public double InRangeFactor(Hsv hsv)
        {
            double result = 1.0;
            if (HslOrHsvProcessingNeeded()) {
                result = HslFactors(hsv.ToHsl(), result);
            }

            if (HsvProcessingNeeded()) {
                result = HsvFactors(hsv, result);
            }

            if (RgbProcessingNeeded()) {
                result = RgbFactors(hsv.ToRgb(), result);
            }

            return result;
        }

        private bool RgbProcessingNeeded()
        {
            return RedRange != null || GreenRange != null || BlueRange != null || BrightnessRange != null;
        }

        private bool HslOrHsvProcessingNeeded()
        {
            return SaturationRange != null || HueRange != null || LightnessRange != null || ValueRange != null;
        }

        private bool HslProcessingNeeded()
        {
            return LightnessRange != null;
        }

        private bool HsvProcessingNeeded()
        {
            return ValueRange != null;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(BrightnessRange != null ? $"h:" + BrightnessRange.ToString() + " " : "");
            sb.Append(HueRange != null ? $"h:" + HueRange.ToString() + " " : "");
            sb.Append(SaturationRange != null ? $"s:" + SaturationRange.ToString() + " " : "");
            sb.Append(LightnessRange != null ? $"l:" + LightnessRange.ToString() + " " : "");
            sb.Append(RedRange != null ? $"r:"+ RedRange.ToString() + " " : "");
            sb.Append(GreenRange != null ? $"g:" + GreenRange.ToString() + " " : "");
            sb.Append(BlueRange != null ? $"b:" + BlueRange.ToString() + " " : "");
           
            return sb.ToString();
        }

        public ColorRange Brightness(double min, double max, double minSlope = 0, double maxSlope = 0)
        {
            BrightnessRange = ParameterRange.Range(min, max, minSlope, maxSlope);
            return this;
        }
        
        public ColorRange Brightness4P(double minStart, double minEnd, double maxStart, double maxEnd)
        {
            BrightnessRange = ParameterRange.Range(minStart, minEnd, maxStart, maxEnd);
            return this;
        }
        
        public ColorRange Hue(double min, double max, double minSlope = 0, double maxSlope = 0)
        {
            HueRange = ParameterRange.Range(min, max, minSlope, maxSlope, 360);
            return this;
        }
        
        public ColorRange Hue4P(double minStart, double minEnd, double maxStart, double maxEnd)
        {
            HueRange = ParameterRange.Range(minStart, minEnd, maxStart, maxEnd, 360);
            return this;
        }
        
        public ColorRange Saturation(double min, double max, double minSlope = 0, double maxSlope = 0)
        {
            SaturationRange = ParameterRange.Range(min, max, minSlope, maxSlope);
            return this;
        }
        
        public ColorRange Saturation4P(double minStart, double minEnd, double maxStart, double maxEnd)
        {
            SaturationRange = ParameterRange.Range(minStart, minEnd, maxStart, maxEnd);
            return this;
        }
        
        public ColorRange Lightness(double min, double max, double minSlope = 0, double maxSlope = 0)
        {
            LightnessRange = ParameterRange.Range(min, max, minSlope, maxSlope);
            return this;
        }
        
        public ColorRange Lightness4P(double minStart, double minEnd, double maxStart, double maxEnd)
        {
            LightnessRange = ParameterRange.Range(minStart, minEnd, maxStart, maxEnd);
            return this;
        }
        
        public ColorRange Value(double min, double max, double minSlope = 0, double maxSlope = 0)
        {
            ValueRange = ParameterRange.Range(min, max, minSlope, maxSlope);
            return this;
        }
        
        public ColorRange Value4P(double minStart, double minEnd, double maxStart, double maxEnd)
        {
            ValueRange = ParameterRange.Range(minStart, minEnd, maxStart, maxEnd);
            return this;
        }
        
        public ColorRange Red(double min, double max, double minSlope = 0, double maxSlope = 0)
        {
            RedRange = ParameterRange.Range(min, max, minSlope, maxSlope);
            return this;
        }
        
        public ColorRange Red4P(double minStart, double minEnd, double maxStart, double maxEnd)
        {
            RedRange = ParameterRange.Range(minStart, minEnd, maxStart, maxEnd);
            return this;
        }
        
        public ColorRange Green(double min, double max, double minSlope = 0, double maxSlope = 0)
        {
            GreenRange = ParameterRange.Range(min, max, minSlope, maxSlope);
            return this;
        }
        
        public ColorRange Green4P(double minStart, double minEnd, double maxStart, double maxEnd)
        {
            GreenRange = ParameterRange.Range(minStart, minEnd, maxStart, maxEnd);
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