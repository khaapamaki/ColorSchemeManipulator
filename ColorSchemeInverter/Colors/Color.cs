using System;

namespace ColorSchemeInverter.Colors
{
    public abstract class Color
    {
        public RGB ToRGB()
        {
            if (this is RGB) {
                return (RGB) this;
            } else if (this is HSL) {
                return ColorConverter.HSLToRGB((HSL) this);
            }

            throw new NotImplementedException();
        }

        public HSL ToHSL()
        {
            if (this is HSL) {
                return (HSL) this;
            } else if (this is RGB) {
                return ColorConverter.RGBToHSL((RGB) this);
            }

            throw new NotImplementedException();
        }
    }
}