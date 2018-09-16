using System;

namespace ColorSchemeManipulator.Colors
{
    public abstract class ColorBase
    {
        public Rgb ToRgb()
        {
            if (this is Rgb) {
                return (Rgb) this;
            } else if (this is Hsl) {
                return ColorConversions.HslToRgb((Hsl) this);
            } else if (this is Hsv) {
                return ColorConversions.HsvToRgb((Hsv) this);
            }

            throw new NotImplementedException();
        }

        public Hsl ToHsl()
        {
            if (this is Hsl) {
                return (Hsl) this;
            } else if (this is Rgb) {
                return ColorConversions.RgbToHsl((Rgb) this);
            } else if (this is Hsv) {
                return ColorConversions.HsvToHsl((Hsv) this);
            }

            throw new NotImplementedException();
        }

        public Hsv ToHsv()
        {
            if (this is Hsv) {
                return (Hsv) this;
            } else if (this is Rgb) {
                return ColorConversions.RgbToHsv((Rgb) this);
            } else if (this is Hsl) {
                return ColorConversions.HslToHsv((Hsl) this);
            }

            throw new NotImplementedException();
        }
    }
}