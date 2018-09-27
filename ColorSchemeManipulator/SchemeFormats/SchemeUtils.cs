using System;
using System.Collections.Generic;
using System.Linq;
using ColorSchemeManipulator.Colors;
using ColorSchemeManipulator.Common;
using ColorSchemeManipulator.SchemeFormats.Handlers;

namespace ColorSchemeManipulator.SchemeFormats
{
    public static class SchemeUtils
    {

        public static SchemeFormat GetFormatFromExtension(string extension)
        {
            if (extension.StartsWith(".")) {
                extension = extension.Substring(1);
            }

            switch (extension.ToLower()) {
                case "icls":
                    return SchemeFormat.IDEA;
                case "vstheme":
                    return SchemeFormat.VisualStudio;
                case "json":
                    return SchemeFormat.VSCode;
                case "css":
                    return SchemeFormat.CSS;
                case "png":
                    return SchemeFormat.Image;
                case "jpg":
                    return SchemeFormat.Image;
                case "jpeg":
                    return SchemeFormat.Image;
                default:
                    return SchemeFormat.Unknown;
            }
        }

        public static IColorFileHandler<string> GetSchemeHandlerByFormat(SchemeFormat schemeFormat)
        {
            switch (schemeFormat) {
                case SchemeFormat.IDEA:
                    return new IDEAFileHandler();
                case SchemeFormat.VisualStudio:
                    return new VisualStudioFileHandler();
                case SchemeFormat.VSCode:
                    return new VSCodeFileHandler();
                case SchemeFormat.CSS:
                    return new CSSFileHandler();                
                default:
                    return null;
            }
        }
       

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
        
        /// <summary>
        /// Performs manual replacement for matches with processed color replacement strings
        /// </summary>
        /// <param name="text"></param>
        /// <param name="colorMatches"></param>
        /// <returns></returns>
        public static string BatchReplace(string text, List<RegexReplacement> colorMatches)
        {
            // matches must be in reverse order by indexes, otherwise replacing with strings
            // of which lengths differ from original's will make latter indexes invalid
            colorMatches = colorMatches.OrderByDescending(m => m.Index).ToList();
            
            foreach (var match in colorMatches) {
                text = text.ReplaceWithin(match.Index, match.Length, match.ReplacementString);
                //Console.WriteLine(match.MatchingString + " -> " +  match.ReplacementString);
            }

            return text;
        }

    }
}