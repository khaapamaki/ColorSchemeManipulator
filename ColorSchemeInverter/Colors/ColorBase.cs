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
            } else if (this is HSV) {
                return ColorConversions.HSVToRGB((HSV) this);
            }

            throw new NotImplementedException();
        }

        public HSL ToHSL()
        {
            if (this is HSL) {
                return (HSL) this;
            } else if (this is RGB) {
                return ColorConversions.RGBToHSL((RGB) this);
            } else if (this is HSV) {
                return ColorConversions.HSVToHSL((HSV) this);
            }

            throw new NotImplementedException();
        }
        
        public HSV ToHSV()
        {
            if (this is HSV) {
                return (HSV) this;
            } else if (this is RGB) {
                return ColorConversions.RGBToHSV((RGB) this);
            } else if (this is HSL) {
                return ColorConversions.HSLToHSV((HSL) this);
            }

            throw new NotImplementedException();
        }
    }
}