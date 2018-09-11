using System;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Security.Permissions;
using ColorSchemeInverter.Colors;
using ColorSchemeInverter.Common;

namespace ColorSchemeInverter.Filters
{
    public class ColorRange
    {
        // Todo constructors
        // Todo CLI system for ranges
        // Todo implement slopes value

        public LinearRange SaturationRange { get; set; } = null;
        public LinearRange LightnessRange { get; set; } = null;
        public LinearRange ValueRange { get; set; } = null;
        public LinearRange RedRange { get; set; } = null;
        public LinearRange GreenRange { get; set; } = null;
        public LinearRange BlueRange { get; set; } = null;
        public LoopingRange HueRange { get; set; } = null;

        private double _minHue = 0.0;
        private double _maxHue = 360.0;
        private double _minHueSlope = 0.0;
        private double _maxHueSlope = 0.0;
        private bool _hueRange = false;


        public ColorRange Hue(double min, double max, double minSlope = 0.0, double maxSlope = 0.0)
        {
            _minHue = min.NormalizeLoopingValue(360.0);
            _maxHue = max.NormalizeLoopingValue(360.0);
            _minHueSlope = minSlope;
            _maxHueSlope = maxSlope;
            _hueRange = true;
            return this;
        }


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
            result *= RedRange?.InRangeFactor(rgb.Red) ?? 0;
            result *= GreenRange?.InRangeFactor(rgb.Green) ?? 0;
            result *= BlueRange?.InRangeFactor(rgb.Blue) ?? 0;
            return result;
        }

        private double HsvFactors(Hsv hsv, double result = 1.0)
        {
            result *= ValueRange?.InRangeFactor(hsv.Value) ?? 0;
            return result;
        }

        private double HslFactors(Hsl hsl, double result = 1.0)
        {
            result *= HueRange?.InRangeFactor(hsl.Hue) ?? 0;
            result *= SaturationRange?.InRangeFactor(hsl.Saturation) ?? 0;
            result *= LightnessRange?.InRangeFactor(hsl.Lightness) ?? 0;
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
            return RedRange != null || GreenRange != null || BlueRange != null;
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
    }
}