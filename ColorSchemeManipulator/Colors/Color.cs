using System;
using System.Runtime.InteropServices;
using ColorSchemeManipulator.Common;

namespace ColorSchemeManipulator.Colors
{
    public class Color
    {
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
        }

        public double Green
        {
            get
            {
                CalcRgb();
                return _green;
            }
        }

        public double Blue
        {
            get
            {
                CalcRgb();
                return _blue;
            }
        }

        public double Hue
        {
            get
            {
                CalcHsl();
                return _hue;
            }
        }

        public double HueHsv
        {
            get
            {
                CalcHsv();
                return _hueHsv;
            }
        }
        // {
        //     get => _hue.NormalizeLoopingValue(360.0);
        //     set => _hue = value.NormalizeLoopingValue(360.0);
        // }

        public double Saturation
        {
            get
            {
                CalcHsl();
                return _saturation;
            }
        }

        public double Lightness
        {
            get
            {
                CalcHsl();
                return _lightness;
            }
        }

        public double SaturationHsv
        {
            get
            {
                CalcHsv();
                return _saturationHsv;
            }
        }

        public double Value
        {
            get
            {
                CalcHsv();
                return _value;
            }
        }

        public double Alpha => _alpha;

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
            _alpha = color._alpha;
        }

        public static Color FromRgb(double r, double g, double b, double a = 1.0)
        {
            Color color = new Color();
            color.SetRgb(r, g, b, a);
            return color;
        }

        public static Color FromRgb8(byte r, byte g, byte b, byte a = 0xFF)
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

        public void SetRgb(double r, double g, double b, double a = 1.0)
        {
            _red = r;
            _green = g;
            _blue = b;
            _alpha = a;
            _hasRgb = true;
            _hasHsv = false;
            _hasHsl = false;
        }

        public void SetHsl((double, double, double) hsl, double a = 1.0)
        {
            _hue = hsl.Item1.NormalizeLoopingValue(360);
            _saturation = hsl.Item2;
            _lightness = hsl.Item3;
            _alpha = a;
            _hasHsl = true;
            _hasRgb = false;
            _hasHsv = false;
        }

        public void SetHsv((double, double, double) hsv, double a = 1.0)
        {
            _hueHsv = hsv.Item1.NormalizeLoopingValue(360);
            _saturationHsv = hsv.Item2;
            _value = hsv.Item3;
            _alpha = a;
            _hasHsv = true;
            _hasHsl = false;
            _hasRgb = false;
        }

        public void SetRgb((double, double, double) rgb, double a = 1.0)
        {
            _red = rgb.Item1;
            _green = rgb.Item2;
            _blue = rgb.Item3;
            _alpha = a;
            _hasRgb = true;
            _hasHsv = false;
            _hasHsl = false;
        }

        public void SetHsl(double h, double s, double l, double a = 1.0)
        {
            _hue = h.NormalizeLoopingValue(360);
            _saturation = s;
            _lightness = l;
            _alpha = a;
            _hasHsl = true;
            _hasRgb = false;
            _hasHsv = false;
        }

        public void SetHsv(double h, double s, double v, double a = 1.0)
        {
            _hueHsv = h.NormalizeLoopingValue(360);
            _saturationHsv = s;
            _value = v;
            _alpha = a;
            _hasHsv = true;
            _hasHsl = false;
            _hasRgb = false;
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

        // public string ToRgbString(string rgbHexFormat)
        // {
        //     return ToRgb8Bit().ToRgbString(rgbHexFormat);
        // }


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
                    SetRgb(ColorConversions.HslToRgb(_hue, _saturation, _lightness), _alpha);
                } else if (_hasHsv) {
                    SetRgb(ColorConversions.HslToRgb(_hueHsv, _saturationHsv, _value), _alpha);
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
                    SetHsl(ColorConversions.RgbToHsl(_red, _green, _blue), _alpha);
                } else if (_hasHsv) {
                    SetHsl(ColorConversions.HsvToHsl(_hueHsv, _saturationHsv, _value), _alpha);
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
                    SetHsv(ColorConversions.RgbToHsv(_red, _green, _blue), _alpha);
                } else if (_hasHsl) {
                    SetHsv(ColorConversions.HslToHsv(_hue, _saturation, _lightness), _alpha);
                }
            }

            return _hasHsv;
        }
    }
}