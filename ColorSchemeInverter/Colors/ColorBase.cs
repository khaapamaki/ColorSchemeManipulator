using System;

namespace ColorSchemeInverter.Colors
{
    public abstract class ColorBase
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
    }
}