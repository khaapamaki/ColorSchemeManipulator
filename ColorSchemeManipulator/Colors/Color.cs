using System;
using System.Drawing;
using ColorSchemeManipulator.Filters;

namespace ColorSchemeManipulator.Colors
{
    public abstract class Color
    {
        public Rgb ToRgb()
        {
            if (this is Rgb) {
                return (Rgb) this;
            } else if (this is Hsl) {
                return ColorConversions.HsltoRgb((Hsl) this);
            } else if (this is Hsv) {
                return ColorConversions.HsvtoRgb((Hsv) this);
            }

            throw new NotImplementedException();
        }

        public Hsl ToHsl()
        {
            if (this is Hsl) {
                return (Hsl) this;
            } else if (this is Rgb) {
                return ColorConversions.RgbtoHsl((Rgb) this);
            } else if (this is Hsv) {
                return ColorConversions.HsvtoHsl((Hsv) this);
            }

            throw new NotImplementedException();
        }

        public Hsv ToHsv()
        {
            if (this is Hsv) {
                return (Hsv) this;
            } else if (this is Rgb) {
                return ColorConversions.RgbtoHsv((Rgb) this);
            } else if (this is Hsl) {
                return ColorConversions.HsltoHsv((Hsl) this);
            }

            throw new NotImplementedException();
        }
        
        public System.Drawing.Color ToSystemColor()
        {
            if (this is Hsl) {
                return ColorConversions.RgbToSystemColor(((Hsl) this).ToRgb());
            } else if (this is Rgb) {
                return ColorConversions.RgbToSystemColor((Rgb) this);
            } else if (this is Hsv) {
                return ColorConversions.RgbToSystemColor(((Hsv) this).ToRgb());
            }

            throw new NotImplementedException();
        }
        
//        public Rgb ApplyFilterSet(FilterSet filters)
//        {
//            return filters.ApplyTo(this);
//        }
    }
}