using System;
using System.Collections.Generic;
using ColorSchemeManipulator.Colors;
using ColorSchemeManipulator.Common;

namespace ColorSchemeManipulator.SchemeFormats
{
    public enum PaddingDirection
    {
        Left,
        Right,
        None
    }

    public class PaddableHexFormat
    {
        public string HexFormat { get; set; }
        public string Padding { get; set; }
        public PaddingDirection PaddingDirection { get; set; }
        
        /// <summary>
        /// Converts RGB hex string to Color. Applies padding to short strings by the color scheme padding rules.
        /// </summary>
        /// <param name="rgbString"></param>
        /// <param name="formats"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static Color PaddableHexStringToColor(string rgbString, IEnumerable<PaddableHexFormat> formats)
        {

            foreach (var format in formats) {
                string rgbHexFormat = format.HexFormat;
                if (format.Padding != null && format.Padding.Length != format.HexFormat.Length) {
                    throw new Exception("RGB hex string misconfiguration: " + format.HexFormat + " with padding " +
                                        format.Padding);
                }

                if (HexRgbUtil.IsValidHexString(rgbString) && rgbString.Length <= rgbHexFormat.Length) {
                    if (rgbString.Length < rgbHexFormat.Length) {
                        if (format.PaddingDirection == PaddingDirection.Left
                            || format.PaddingDirection == PaddingDirection.Right) {
                            switch (format.PaddingDirection) {
                                case PaddingDirection.Left:
                                    rgbString = rgbString.PadLeft(format.Padding);
                                    break;
                                case PaddingDirection.Right:
                                    rgbString = rgbString.PadRight(format.Padding);
                                    break;
                            }
                        }
                    }

                    if (rgbString.Length == rgbHexFormat.Length) {
                        return HexRgbUtil.HexStringToColor(rgbString, rgbHexFormat);
                    }
                }
            }

            throw new Exception("Invalid color string: " + rgbString);
        }

    }
}