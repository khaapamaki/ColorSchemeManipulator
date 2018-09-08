using System;
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
        private bool _hueRange = false;
        private double _minSaturation  = 0.0;
        private double _maxSaturation  = 1.0;
        private bool _saturationRange = false;
        private double _minLightness  = 0.0;
        private double _maxLightness  = 1.0;
        private bool _lightnessRange = false;
        private double _minValue  = 0.0;
        private double _maxValue  = 1.0;
        private bool _valueRange = false;
        private double _minRed = 0.0;
        private double _maxRed  = 1.0;
        private bool _redRange = false;
        private double _minGreen  = 0.0;
        private double _maxGreen  = 1.0;
        private bool _greenRange = false;
        private double _minBlue  = 0.0;
        private double _maxBlue  = 1.0;
        private bool _blueRange = false;
        
        public void SetHueRange(double min, double max)
        {
            _minHue = min.NormalizeLoopingValue(360.0);
            _maxHue = max.NormalizeLoopingValue(360.0);
            _hueRange = true;
        }
        
        public void SetSaturationRange(double min, double max)
        {
            _minSaturation = min;
            _maxSaturation = max;
            _saturationRange = true;
        }
        
        public void SetLightnessRange(double min, double max)
        {
            _minLightness = min;
            _maxLightness = max;
            _lightnessRange = true;
        }      
       
        public void SetRedRange(double min, double max)
        {
            _minRed = min;
            _maxRed = max;
            _redRange = true;
        } 
        
        
        public void SetGreenRange(double min, double max)
        {
            _minGreen = min;
            _maxGreen = max;
            _greenRange = true;
        } 
        
        public void SetBlueRange(double min, double max)
        {
            _minBlue = min;
            _maxBlue = max;
            _blueRange = true;
        } 
        
        public void SetValueRange(double min, double max)
        {
            _minValue = min;
            _maxValue = max;
            _valueRange = true;
        } 
        
        public double InRange(RGB rgb)
        {
            double result = 0.0;
            if (HSLVProcessingNeeded()) {
                HSL hsl = new HSL(rgb);
                result *= InHueRange(hsl.Hue) * InSaturationRange(hsl.Saturation) * InLightnessRange(hsl.Lightness);
            }
            if (HsvProcessingNeeded()) {
                throw new NotImplementedException();
                //result *= InValueRange(hsl.ToHSV().Value);
            }
            if (RgbProcessingNeeded()) {
                result *= InRedRange(rgb.Red) * InGreenRange(rgb.Green) * InBlueRange(rgb.Blue);
            }

            return result;
        }
        
        public double InRange(HSL hsl)
        {
            double result = 0.0;
            if (HSLVProcessingNeeded()) {
                result *= InHueRange(hsl.Hue) * InSaturationRange(hsl.Saturation) * InLightnessRange(hsl.Lightness);
            }
            if (HsvProcessingNeeded()) {
                throw new NotImplementedException();
                //result *= InValueRange(hsl.ToHSV().Value);
            }
            if (RgbProcessingNeeded()) {
                RGB rgb = new RGB(hsl);
                result *= InRedRange(rgb.Red) * InGreenRange(rgb.Green) * InBlueRange(rgb.Blue);
            }

            return result;

        }

        private bool RgbProcessingNeeded()
        {
            return _redRange || _greenRange || _blueRange;
        }
        
        private bool HSLVProcessingNeeded()
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
                return hue >= _minSaturation && hue <= _maxSaturation ? 1.0 : 0.0;
            } else {
                return hue >= _minSaturation || hue <= _maxSaturation ? 1.0 : 0.0;
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