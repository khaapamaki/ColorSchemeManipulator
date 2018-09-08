using System;

namespace ColorSchemeInverter.Colors
{
    public abstract class ColorBase
    {
        public RGB ToRGB()
        {
            if (this is RGB) {
                return (RGB) this;
            } else if (this is HSL) {
                return ColorConversions.HSLToRGB((HSL) this);
            }

            throw new NotImplementedException();
        }

        public HSL ToHSL()
        {
            if (this is HSL) {
                return (HSL) this;
            } else if (this is RGB) {
                return ColorConversions.RGBToHSL((RGB) this);
            }

            throw new NotImplementedException();
        }
    }
}