using System;
using ColorSchemeManipulator.Common;

// Todo Thread safety

namespace ColorSchemeManipulator.Colors
{
    public enum ColorFormat
    {
        Rgb,
        Hsl,
        Hsv,
        None
    }

    public enum Clamping
    {
        LowHigh,
        Low,
        None
    }

    public class Color
    {
        // Clamping automation is off by default. In derived subclasses you may want to set it on.
        protected Clamping InputInputClamping = Clamping.None;
        
        private int _calcLock = 0;

        private bool _rgbValid = false;
        private bool _hslValid = false;
        private bool _hsvValid = false;

        private double _red;
        private double _green;
        private double _blue;
        private double _hue;
        private double _saturation;
        private double _lightness;
        private double _hueHsv;
        private double _saturationHsv;
        private double _value;
        private double _alpha = 1.0;


        public Color()
        {
            
        }

        public Color(Color color)
        {
            CopyFrom(color);
        }
        
        public double Red
        {
            get
            {
                CalcRgb();
                return _red;
            }
            set
            {
                CalcRgb();
                _calcLock++;
                _red = ClampInput(value);
                ResetFlags(ColorFormat.Rgb);
                _calcLock--;
            }
        }

        public double Green
        {
            get
            {
                CalcRgb();
                return _green;
            }
            set
            {
                CalcRgb();
                _calcLock++;
                _green = ClampInput(value);
                ResetFlags(ColorFormat.Rgb);
                _calcLock--;
            }
        }

        public double Blue
        {
            get
            {
                CalcRgb();
                return _blue;
            }
            set
            {
                CalcRgb();
                _calcLock++;
                _blue = ClampInput(value);
                ResetFlags(ColorFormat.Rgb);
                _calcLock--;
            }
        }

        public double Hue
        {
            get
            {
                CalcHsl();
                return _hue;
            }
            set
            {
                CalcHsl();
                _calcLock++;
                _hue = value.NormalizeLoopingValue(360);
                ResetFlags(ColorFormat.Hsl);
                _calcLock--;
            }
        }

        public double HueHsv
        {
            get
            {
                CalcHsv();
                return _hueHsv;
            }
            set
            {
                CalcHsv();
                _calcLock++;
                _hueHsv = value.NormalizeLoopingValue(360);
                ResetFlags(ColorFormat.Hsv);
                _calcLock--;
            }
        }

        public double Saturation
        {
            get
            {
                CalcHsl();
                return _saturation;
            }
            set
            {
                CalcHsl();
                _calcLock++;
                _saturation = ClampInput(value);
                ResetFlags(ColorFormat.Hsl);
                _calcLock--;
            }
        }

        public double Lightness
        {
            get
            {
                CalcHsl();
                return _lightness;
            }
            set
            {
                CalcHsl();
                _calcLock++;
                _lightness = ClampInput(value);
                ResetFlags(ColorFormat.Hsl);
                _calcLock--;
            }
        }

        public double SaturationHsv
        {
            get
            {
                CalcHsv();
                return _saturationHsv;
            }
            set
            {
                CalcHsv();
                _calcLock++;
                _saturationHsv = ClampInput(value);
                ResetFlags(ColorFormat.Hsv);
                _calcLock--;
            }
        }

        public double Value
        {
            get
            {
                CalcHsv();
                return _value;
            }
            set
            {
                CalcHsv();
                _calcLock++;
                _value = ClampInput(value);
                ResetFlags(ColorFormat.Hsv);
                _calcLock--;
            }
        }

        public double Alpha
        {
            get { return _alpha; }
            set { _alpha = value.Clamp(0, 1); }
        }

        
        protected void CopyFrom(Color color)
        {
            CopyFrom(color, InputInputClamping);
        }
        
        protected void CopyFrom(Color color, Clamping clamping)
        {
            _calcLock++;

            _hue = clamping != Clamping.None ? color._hue.NormalizeLoopingValue(360) : color._hue;
            _saturation = ClampValue(color._saturation,clamping);
            _lightness = ClampValue(color._lightness, clamping);
            _red = ClampValue(color._red, clamping);
            _green = ClampValue(color._green, clamping);
            _blue = ClampValue(color._blue, clamping);
            _hueHsv = clamping != Clamping.None ? color._hueHsv.NormalizeLoopingValue(360) : color._hueHsv;
            _saturationHsv = ClampValue(color._saturationHsv, clamping);
            _value = ClampValue(color._value, clamping);
            _rgbValid = color._rgbValid;
            _hslValid = color._hslValid;
            _hsvValid = color._hsvValid;
                
            _alpha = color._alpha;

            _calcLock--;
        }

        public static Color FromRgb(double r, double g, double b, double a = 1.0)
        {
            Color color = new Color();
            color.SetRgb(r, g, b, a);
            
            return color;
        }

        public static Color FromRgb(byte r, byte g, byte b, byte a = 0xFF)
        {
            Color color = new Color();
            color.SetRgb(r / 255.0, g / 255.0, b / 255.0, a / 255.0);
            
            return color;
        }

        public static Color FromHsl(double h, double s, double l, double a = 1.0)
        {
            Color color = new Color();
            color.SetHsl(h, s, l, a);
            
            return color;
        }

        public static Color FromHsv(double h, double s, double v, double a = 1.0)
        {
            Color color = new Color();
            color.SetHsv(h, s, v, a);
            
            return color;
        }
        
        public void SetRgb(double r, double g, double b, double a = 1.0, bool resetFlags = true)
        {
            _calcLock++;

            _red = r;
            _green = g;
            _blue = b;
            _alpha = a;
            if (resetFlags) ResetFlags(ColorFormat.Rgb);

            _calcLock--;
        }

        public void SetHsl((double, double, double) hsl, double a = 1.0, bool resetFlags = true)
        {
            _calcLock++;

            _hue = hsl.Item1.NormalizeLoopingValue(360);
            _saturation = hsl.Item2;
            _lightness = hsl.Item3;
            _alpha = a;
            if (resetFlags) ResetFlags(ColorFormat.Hsl);

            _calcLock--;
        }

        public void SetHsv((double, double, double) hsv, double a = 1.0, bool resetFlags = true)
        {
            _calcLock++;

            _hueHsv = hsv.Item1.NormalizeLoopingValue(360);
            _saturationHsv = hsv.Item2;
            _value = hsv.Item3;
            _alpha = a;
            if (resetFlags) ResetFlags(ColorFormat.Hsv);

            _calcLock--;
        }

        public void SetRgb((double, double, double) rgb, double a = 1.0, bool resetFlags = true)
        {
            _calcLock++;

            _red = rgb.Item1;
            _green = rgb.Item2;
            _blue = rgb.Item3;
            _alpha = a;
            if (resetFlags) ResetFlags(ColorFormat.Rgb);

            _calcLock--;
        }

        public void SetHsl(double h, double s, double l, double a = 1.0, bool resetFlags = true)
        {
            _calcLock++;

            _hue = h.NormalizeLoopingValue(360);
            _saturation = s;
            _lightness = l;
            _alpha = a;
            if (resetFlags) ResetFlags(ColorFormat.Hsl);

            _calcLock--;
        }

        public void SetHsv(double h, double s, double v, double a = 1.0, bool resetFlags = true)
        {
            _calcLock++;

            _hueHsv = h.NormalizeLoopingValue(360);
            _saturationHsv = s;
            _value = v;
            _alpha = a;
            if (resetFlags) ResetFlags(ColorFormat.Hsv);

            _calcLock--;
        }

        // INTERFACE

        public void ClampExceedingColors()
        {
            CalcHsl();
            if (_saturation > 1 || _saturation < 0 || _lightness < 0 || _lightness > 1) {
                _calcLock++;

                _hue = _hue.NormalizeLoopingValue(360);
                _saturation = _saturation.Clamp(0, 1);
                _lightness = _lightness.Clamp(0, 1);

                ResetFlags(ColorFormat.Hsl);
                _calcLock--;
            }

            CalcHsv();

            if (_saturationHsv > 1 || _value > 1 || _saturationHsv < 0 || _value < 0) {
                _calcLock++;
                
                _saturationHsv = _saturationHsv.Clamp(0, 1);
                _value = _value.Clamp(0, 1);
                ResetFlags(ColorFormat.Hsv);
                
                _calcLock--;
            }

            CalcRgb();
            
            if (_red < 0 || _red > 1 || _green < 0 || _green > 1 || _blue < 0 || _blue > 1) {
                // this probably never occurs. but if so.. this produces hue shifts.
                _calcLock++;
                
                _red = _red.Clamp(0, 1);
                _green = _green.Clamp(0, 1);
                _blue = _blue.Clamp(0, 1);
                ResetFlags(ColorFormat.Rgb);
                
                _calcLock--;
            }
        }

        public static Color Interpolate(Color color1, Color color2, double factor)
        {
            factor = factor.Clamp(0, 1);
            double r = ColorMath.LinearInterpolation(factor, color1.Red, color2.Red);
            double g = ColorMath.LinearInterpolation(factor, color1.Green, color2.Green);
            double b = ColorMath.LinearInterpolation(factor, color1.Blue, color2.Blue);
            double a = ColorMath.LinearInterpolation(factor, color1.Alpha, color2.Alpha);
            return FromRgb(r, g, b, a);
        }

        public Color InterpolateWith(Color color, double factor)
        {
            factor = factor.Clamp(0, 1);
            double r = ColorMath.LinearInterpolation(factor, Red, color.Red);
            double g = ColorMath.LinearInterpolation(factor, Green, color.Green);
            double b = ColorMath.LinearInterpolation(factor, Blue, color.Blue);
            double a = ColorMath.LinearInterpolation(factor, Alpha, color.Alpha);
            SetRgb(r, g, b, a);
            return this;
        }

        public override string ToString()
        {
            return string.Format(
                $"R:{Red:F3}, G:{Green:F3}, B:{Blue:F3}, H:{Hue:F1}, S:{Saturation:F3}, L:{Lightness:F3}, S2:{SaturationHsv:F3}, V:{Value:F3}, A:{Alpha:F2}");
        }

        public string ToString(string format)
        {
            if (format.ToUpper() == "X2") {
                return string.Format(
                    $"R: 0x{Red * 255:X2}, G: 0x{Green * 255:X2}, B: 0x{Blue * 255:X2}  A: 0x{Alpha * 255:X2}");
            } else {
                throw new FormatException("Invalid Format String: " + format);
            }
        }

        public double GetBrightness()
        {
            (double r, double g, double b) = GetRgbComponents();
            return ColorMath.RgbPerceivedBrightness(r, g, b);
        }

        public double GetLightness()
        {
            // todo faster formula to calc lightness only
            return Lightness;
        }

        // INTERNAL PROCESSING

        private double ClampInput(double input)
        {
            return ClampValue(input, InputInputClamping);
        }

        private double ClampValue(double input, Clamping clamping)
        {
            if (clamping == Clamping.LowHigh)
                return input.Clamp(0, 1);

            if (clamping == Clamping.Low)
                return input.LimitLow(0);

            return input;
        }
        
        private void ResetFlags(ColorFormat cf)
        {
            _rgbValid = cf == ColorFormat.Rgb;
            _hslValid = cf == ColorFormat.Hsl;
            _hsvValid = cf == ColorFormat.Hsv;
        }

        private bool HasValue()
        {
            return _rgbValid || _hslValid || _hsvValid;
        }

        public (double, double, double) GetRgbComponents()
        {
            if (!_rgbValid) {
                if (!CalcRgb()) {
                    throw new Exception("Color has no value");
                }
            }

            return (_red, _green, _blue);
        }

        public (double, double, double) GetHslComponents()
        {
            if (!_hslValid) {
                if (!CalcHsl()) {
                    throw new Exception("Color has no value");
                }
            }

            return (_hue, _saturation, _lightness);
        }

        public (double, double, double) GetHsvComponents()
        {
            if (!_hsvValid) {
                if (!CalcHsv()) {
                    throw new Exception("Color has no value");
                }
            }

            return (_hueHsv, _saturationHsv, _value);
        }

        private bool CalcRgb()
        {
            if (_calcLock > 0)
                return _rgbValid;

            if (!HasValue())
                throw new Exception("Color has no value");
            if (!_rgbValid) {
                if (_hslValid) {
                    SetRgb(ColorConversions.HslToRgb(_hue, _saturation, _lightness), _alpha, false);
                } else if (_hsvValid) {
                    SetRgb(ColorConversions.HsvToRgb(_hueHsv, _saturationHsv, _value), _alpha, false);
                }
            }

            return _rgbValid;
        }

        private bool CalcHsl()
        {
            if (_calcLock > 0)
                return _hslValid;

            if (!HasValue())
                throw new Exception("Color has no value");
            if (!_hslValid) {
                if (_rgbValid) {
                    SetHsl(ColorConversions.RgbToHsl(_red, _green, _blue), _alpha, false);
                } else if (_hsvValid) {
                    SetHsl(ColorConversions.HsvToHsl(_hueHsv, _saturationHsv, _value), _alpha, false);
                }
            }

            return _hslValid;
        }

        private bool CalcHsv()
        {
            if (_calcLock > 0)
                return _hsvValid;

            if (!HasValue())
                throw new Exception("Color has no value");
            if (!_hsvValid) {
                if (_rgbValid) {
                    SetHsv(ColorConversions.RgbToHsv(_red, _green, _blue), _alpha, false);
                } else if (_hslValid) {
                    SetHsv(ColorConversions.HslToHsv(_hue, _saturation, _lightness), _alpha, false);
                }
            }

            return _hsvValid;
        }

        public bool Equals(Color color)
        {
            if (!_alpha.AboutEqual(color._alpha))
                return false;

            if (_rgbValid) {
                return _red.AboutEqual(color.Red)
                       && _green.AboutEqual(color.Green)
                       && _blue.AboutEqual(color.Blue);
            }

            if (_hslValid) {
                return _lightness.AboutEqual(color.Lightness)
                       && _saturation.AboutEqual(color.Saturation)
                       && _hue.AboutEqual(color.Hue);
            }

            if (_hsvValid) {
                return _hueHsv.AboutEqual(color.HueHsv)
                       && _saturationHsv.AboutEqual(color.SaturationHsv)
                       && _value.AboutEqual(color.Value);
            }

            return false;
        }
    }
}