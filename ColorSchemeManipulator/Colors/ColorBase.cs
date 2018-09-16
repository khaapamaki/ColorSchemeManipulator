using System;
using System.Drawing;
using ColorSchemeManipulator.Filters;

namespace ColorSchemeManipulator.Colors
{
    public abstract class ColorBase
    {
        public Rgb ToRgb()
        {
            if (this is Rgb) {
                return (Rgb) this;
                // return new Rgb((Rgb) this);
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
                // return new Hsl((Hsl) this);
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
                //return new Hsv((Hsv) this);
            } else if (this is Rgb) {
                return ColorConversions.RgbToHsv((Rgb) this);
            } else if (this is Hsl) {
                return ColorConversions.HslToHsv((Hsl) this);
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
        
    }
}