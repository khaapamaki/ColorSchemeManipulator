using System;
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

        private double _minHue = 0.0;
        private double _maxHue = 360.0;
        private double _minHueSlope = 0.0;
        private double _maxHueSlope = 0.0;
        private bool _hueRange = false;
        private double _minSaturation  = 0.0;
        private double _maxSaturation  = 1.0;
        private double _minSaturationSlope  = 0.0;
        private double _maxSaturationSlope  = 0.0;
        private bool _saturationRange = false;
        private double _minLightness  = 0.0;
        private double _maxLightness  = 1.0;
        private double _minLightnessSlope  = 0.0;
        private double _maxLightnessSlope  = 0.0;
        private bool _lightnessRange = false;
        private double _minValue  = 0.0;
        private double _maxValue  = 1.0;
        private double _minValueSlope  = 0.0;
        private double _maxValueSlope  = 0.0;
        private bool _valueRange = false;
        private double _minRed = 0.0;
        private double _maxRed  = 1.0;
        private double _minRedSlope = 0.0;
        private double _maxRedSlope  = 0.0;
        private bool _redRange = false;
        private double _minGreen  = 0.0;
        private double _maxGreen  = 1.0;
        private double _minGreenSlope  = 0.0;
        private double _maxGreenSlope  = 0.0;
        private bool _greenRange = false;
        private double _minBlue  = 0.0;
        private double _maxBlue  = 1.0;
        private double _minBlueSlope  = 0.0;
        private double _maxBlueSlope  = 0.0;        
        private bool _blueRange = false;
        
        public ColorRange Hue(double min, double max, double minSlope = 0.0, double maxSlope = 0.0)
        {
            _minHue = min.NormalizeLoopingValue(360.0);
            _maxHue = max.NormalizeLoopingValue(360.0);
            _minHueSlope = minSlope;
            _maxHueSlope = maxSlope;
            _hueRange = true;
            return this;
        }
        
        public ColorRange Saturation(double min, double max, double minSlope = 0.0, double maxSlope = 0.0)
        {
            _minSaturation = min;
            _maxSaturation = max;
            _minSaturationSlope = minSlope;
            _maxSaturationSlope = maxSlope;
            _saturationRange = true;
            return this;
        }
        
        public ColorRange Lightness(double min, double max, double minSlope = 0.0, double maxSlope = 0.0)
        {
            _minLightness = min;
            _maxLightness = max;
            _minLightnessSlope = minSlope;
            _maxLightnessSlope = maxSlope;
            _lightnessRange = true;
            return this;
        }      
       
        public ColorRange Red(double min, double max, double minSlope = 0.0, double maxSlope = 0.0)
        {
            _minRed = min;
            _maxRed = max;
            _minRedSlope = minSlope;
            _maxRedSlope = maxSlope;
            _redRange = true;
            return this;
        } 
         
        public ColorRange Green(double min, double max, double minSlope = 0.0, double maxSlope = 0.0)
        {
            _minGreen = min;
            _maxGreen = max;
            _minGreenSlope = minSlope;
            _maxGreenSlope = maxSlope;
            _greenRange = true;
            return this;
        } 
        
        public ColorRange Blue(double min, double max, double minSlope = 0.0, double maxSlope = 0.0)
        {
            _minBlue = min;
            _maxBlue = max;
            _minBlueSlope = minSlope;
            _maxBlueSlope = maxSlope;
            _blueRange = true;
            return this;
        } 
        
        public ColorRange Value(double min, double max, double minSlope = 0.0, double maxSlope = 0.0)
        {
            _minValue = min;
            _maxValue = max;
            _minValueSlope = minSlope;
            _maxValueSlope = maxSlope;
            _valueRange = true;
            return this;
        } 
        
        public double InRangeFactor(Rgb rgb)
        {
            
            Hsl __hsl = rgb.ToHsl();
            double h = InHueRange(__hsl.Hue);
            double s = InSaturationRange(__hsl.Saturation);
            double l = InLightnessRange(__hsl.Lightness);
            double r = InRedRange(rgb.Red);
            double g = InGreenRange(rgb.Green);
            double b = InBlueRange(rgb.Blue);
            double v = InValueRange(rgb.ToHsv().Value);
            
            
            
            double result = 1.0;
            if (HslOrHsvProcessingNeeded()) {
                var hsl = new Hsl(rgb);
                result *= InHueRange(hsl.Hue) * InSaturationRange(hsl.Saturation) * InLightnessRange(hsl.Lightness);
            }
            if (HsvProcessingNeeded()) {
                result *= InValueRange(rgb.ToHsv().Value);
            }
            if (RgbProcessingNeeded()) {
                result *= InRedRange(rgb.Red) * InGreenRange(rgb.Green) * InBlueRange(rgb.Blue);
            }

            return result;
        }
        
        public double InRangeFactor(Hsl hsl)
        {
            double result = 1.0;
            if (HslOrHsvProcessingNeeded()) {
                result *= InHueRange(hsl.Hue) * InSaturationRange(hsl.Saturation) * InLightnessRange(hsl.Lightness);
            }
            if (HsvProcessingNeeded()) {
                result *= InValueRange(hsl.ToHsv().Value);
            }
            if (RgbProcessingNeeded()) {
                var rgb = new Rgb(hsl);
                result *= InRedRange(rgb.Red) * InGreenRange(rgb.Green) * InBlueRange(rgb.Blue);
            }

            return result;
        }

                
        public double InRangeFactor(Hsv hsv)
        {
            double result = 1.0;
            if (HslOrHsvProcessingNeeded()) {
                result *= InHueRange(hsv.Hue) * InSaturationRange(hsv.Saturation) * InLightnessRange(hsv.Value);
            }
            if (HslProcessingNeeded()) {
                result *= InLightnessRange(hsv.ToHsl().Lightness);
            }
            if (RgbProcessingNeeded()) {
                var rgb = new Rgb(hsv);
                result *= InRedRange(rgb.Red) * InGreenRange(rgb.Green) * InBlueRange(rgb.Blue);
            }

            return result;
        }
        
        private bool RgbProcessingNeeded()
        {
            return _redRange || _greenRange || _blueRange;
        }
        
        private bool HslOrHsvProcessingNeeded()
        {
            return _saturationRange || _hueRange || _lightnessRange || _valueRange;
        }
        
        private bool HslProcessingNeeded()
        {
            return _lightnessRange;
        }
        
        private bool HsvProcessingNeeded()
        {
            return _valueRange;
        }
        
        private double InHueRange(double hue)
        {
            if (_minHue <= _maxHue) {
                return hue >= _minHue && hue <= _maxHue ? 1.0 : 0.0;
            } else {
                return hue >= _minHue || hue <= _maxHue ? 1.0 : 0.0;
            }
        }
        
        private double InSaturationRange(double saturation)
        {
            if (_minSaturation <= _maxSaturation) {
                return saturation >= _minSaturation && saturation <= _maxSaturation ? 1.0 : 0.0;
            }

            return 0.0;
        }
        
        private double InLightnessRange(double lightness)
        {
            if (_minLightness <= _maxLightness) {
                return lightness >= _minLightness && lightness <= _maxLightness ? 1.0 : 0.0;
            }

            return 0.0;
        }
        
        private double InRedRange(double value)
        {
            if (_minRed <= _maxRed) {
                return value >= _minRed && value <= _maxRed ? 1.0 : 0.0;
            }

            return 0.0;
        }
        
        private double InGreenRange(double value)
        {
            if (_minGreen <= _maxGreen) {
                return value >= _minGreen && value <= _maxGreen ? 1.0 : 0.0;
            }

            return 0.0;
        }
        
        private double InBlueRange(double value)
        {
            if (_minBlue <= _maxBlue) {
                return value >= _minBlue && value <= _maxBlue ? 1.0 : 0.0;
            }

            return 0.0;
        }
        
        private double InValueRange(double value)
        {
            if (_minValue <= _maxValue) {
                return value >= _minValue && value <= _maxValue ? 1.0 : 0.0;
            }

            return 0.0;
        }


    }
}