using System;
using System.Collections.Generic;
using ColorSchemeManipulator.Common;

namespace ColorSchemeManipulator.Colors
{
    public class Color
    {
        private const Clamping InputClamping = Clamping.LowHigh;

        private bool _hasRgb = false;
        private bool _hasHsl = false;
        private bool _hasHsv = false;

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
                _red = ClampInput(value);
                ResetFlags(ColorFormat.Rgb);
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
                _green = ClampInput(value);
                ResetFlags(ColorFormat.Rgb);
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
                _blue = ClampInput(value);
                ResetFlags(ColorFormat.Rgb);
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
                _hue = value.NormalizeLoopingValue(360);
                ResetFlags(ColorFormat.Hsl);
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
                _hueHsv = value.NormalizeLoopingValue(360);
                ResetFlags(ColorFormat.Hsv);
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
                _saturation = ClampInput(value);
                ResetFlags(ColorFormat.Hsl);
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
                _lightness = ClampInput(value);
                ResetFlags(ColorFormat.Hsl);
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
                _saturationHsv = ClampInput(value);
                ResetFlags(ColorFormat.Hsv);
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
                _value = ClampInput(value);
                ResetFlags(ColorFormat.Hsv);
            }
        }

        public double Alpha
        {
            get { return _alpha; }
            set { _alpha = value.Clamp(0, 1); }
        }

        public Color() { }

        public Color(Color color)
        {
            CopyFrom(color);
        }

        private void CopyFrom(Color color)
        {
            _hue = color._hue;
            _saturation = color._saturation;
            _lightness = color._lightness;
            _red = color._red;
            _green = color._green;
            _blue = color._blue;
            _hueHsv = color._hueHsv;
            _saturationHsv = color._saturationHsv;
            _value = color._value;
            _hasRgb = color._hasRgb;
            _hasHsl = color._hasHsl;
            _hasHsv = color._hasHsv;
            _alpha = color._alpha;
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
            _red = r;
            _green = g;
            _blue = b;
            _alpha = a;
            if (resetFlags) ResetFlags(ColorFormat.Rgb);
        }

        public void SetHsl((double, double, double) hsl, double a = 1.0, bool resetFlags = true)
        {
            _hue = hsl.Item1.NormalizeLoopingValue(360);
            _saturation = hsl.Item2;
            _lightness = hsl.Item3;
            _alpha = a;
            if (resetFlags) ResetFlags(ColorFormat.Hsl);
        }

        public void SetHsv((double, double, double) hsv, double a = 1.0, bool resetFlags = true)
        {
            _hueHsv = hsv.Item1.NormalizeLoopingValue(360);
            _saturationHsv = hsv.Item2;
            _value = hsv.Item3;
            _alpha = a;
            if (resetFlags) ResetFlags(ColorFormat.Hsv);
        }

        public void SetRgb((double, double, double) rgb, double a = 1.0, bool resetFlags = true)
        {
            _red = rgb.Item1;
            _green = rgb.Item2;
            _blue = rgb.Item3;
            _alpha = a;
            if (resetFlags) ResetFlags(ColorFormat.Rgb);
        }

        public void SetHsl(double h, double s, double l, double a = 1.0, bool resetFlags = true)
        {
            _hue = h.NormalizeLoopingValue(360);
            _saturation = s;
            _lightness = l;
            _alpha = a;
            if (resetFlags) ResetFlags(ColorFormat.Hsl);
        }

        public void SetHsv(double h, double s, double v, double a = 1.0, bool resetFlags = true)
        {
            _hueHsv = h.NormalizeLoopingValue(360);
            _saturationHsv = s;
            _value = v;
            _alpha = a;
            if (resetFlags) ResetFlags(ColorFormat.Hsv);
        }

        // INTERFACE

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
                $"R:{Red:F3}, G:{Green:F3}, B:{Blue:F3}, H:{Hue:F1}, S:{Saturation:F3}, L:{Lightness:F3}, H2:{HueHsv:F1} S2:{SaturationHsv:F3}, V:{Value:F3}, A:{Alpha:F2}");
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

        private void ResetFlags(ColorFormat cf)
        {
            _hasRgb = cf == ColorFormat.Rgb;
            _hasHsl = cf == ColorFormat.Hsl;
            _hasHsv = cf == ColorFormat.Hsv;
        }

        private bool HasValue()
        {
            return _hasRgb || _hasHsl || _hasHsv;
        }

        public (double, double, double) GetRgbComponents()
        {
            if (!_hasRgb) {
                if (!CalcRgb()) {
                    throw new Exception("Color has no value");
                }
            }

            return (_red, _green, _blue);
        }

        public (double, double, double) GetHslComponents()
        {
            if (!_hasHsl) {
                if (!CalcHsl()) {
                    throw new Exception("Color has no value");
                }
            }

            return (_hue, _saturation, _lightness);
        }

        public (double, double, double) GetHsvComponents()
        {
            if (!_hasHsv) {
                if (!CalcHsv()) {
                    throw new Exception("Color has no value");
                }
            }

            return (_hueHsv, _saturationHsv, _value);
        }

        private bool CalcRgb()
        {
            if (!HasValue())
                throw new Exception("Color has no value");
            if (!_hasRgb) {
                if (_hasHsl) {
                    SetRgb(ColorConversions.HslToRgb(_hue, _saturation, _lightness), _alpha, false);
                } else if (_hasHsv) {
                    SetRgb(ColorConversions.HsvToRgb(_hueHsv, _saturationHsv, _value), _alpha, false);
                }
            }

            return _hasRgb;
        }

        private bool CalcHsl()
        {
            if (!HasValue())
                throw new Exception("Color has no value");
            if (!_hasHsl) {
                if (_hasRgb) {
                    SetHsl(ColorConversions.RgbToHsl(_red, _green, _blue), _alpha, false);
                } else if (_hasHsv) {
                    SetHsl(ColorConversions.HsvToHsl(_hueHsv, _saturationHsv, _value), _alpha, false);
                }
            }

            return _hasHsl;
        }

        private bool CalcHsv()
        {
            if (!HasValue())
                throw new Exception("Color has no value");
            if (!_hasHsv) {
                if (_hasRgb) {
                    SetHsv(ColorConversions.RgbToHsv(_red, _green, _blue), _alpha, false);
                } else if (_hasHsl) {
                    SetHsv(ColorConversions.HslToHsv(_hue, _saturation, _lightness), _alpha, false);
                }
            }

            return _hasHsv;
        }

        public bool EqualTo(Color color)
        {
            if (!_alpha.AboutEqual(color._alpha))
                return false;

            if (_hasRgb) {
                return _red.AboutEqual(color.Red)
                       && _green.AboutEqual(color.Green)
                       && _blue.AboutEqual(color.Blue);
            }

            if (_hasHsl) {
                return _lightness.AboutEqual(color.Lightness)
                       && _saturation.AboutEqual(color.Saturation)
                       && _hue.AboutEqual(color.Hue);
            }

            if (_hasHsv) {
                return _hueHsv.AboutEqual(color.HueHsv)
                       && _saturationHsv.AboutEqual(color.SaturationHsv)
                       && _value.AboutEqual(color.Value);
            }

            return false;
        }

        private double ClampInput(double input)
        {
            if (InputClamping == Clamping.Low)
                return input.Clamp(0, 1);

            if (InputClamping == Clamping.LowHigh)
                return input.LimitLow(0);

            return input;
        }
    }


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
}