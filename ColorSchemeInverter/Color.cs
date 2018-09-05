using System;

namespace ColorSchemeInverter
{
    public abstract class Color
    {
        
        public RGB ToRGB()
        {
            if (this is RGB) {
                return (RGB) this;
            } else if (this is HSL) {
                return ((HSL) this).ToRGB();
            }
            throw new NotImplementedException();
        }
        
        public HSL ToHSL()
        {
            if (this is HSL) {
                return (HSL) this;
            } else if (this is RGB) {
                return ((RGB) this).ToHSL();
            }
            throw new NotImplementedException();
        }
    }
}