using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using ColorSchemeManipulator.Colors;
using ColorSchemeManipulator.Common;

namespace ColorSchemeManipulator.SchemeFileSupport
{
    public static class SchemeFormatUtils
    {
        [Obsolete] // copy stuff before deleting
        private const string PatternHex8 = "(?<hex>[0-9abcdefABCDEF]{8})";
        private const string PatternHex6 = "(?<hex>[0-9abcdefABCDEF]{6})";
        private const string PatternHex3or6 = "(?<hex>[0-9abcdefABCDEF]{6}|[0-9abcdefABCDEF]{3})";
        private const string PatternHex6or8 = "(?<hex>[0-9abcdefABCDEF]{8}|[0-9abcdefABCDEF]{6})";
        private const string PatternHex2to6 = "(?<hex>[0-9abcdefABCDEF]{2,6})";

        public static SchemeFormat GetFormatFromExtension(string extension)
        {
            if (extension.StartsWith(".")) {
                extension = extension.Substring(1);
            }

            switch (extension.ToLower()) {
                case "icls":
                    return SchemeFormat.Idea;
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

        public static IColorFileHandler<string> GetHandlerByFormat(SchemeFormat schemeFormat)
        {
            switch (schemeFormat) {
                case SchemeFormat.Idea:
                    return new IdeaSchemeFileHandler();
                case SchemeFormat.VisualStudio:
                    return new VisualStudioSchemeFileHandler();
                case SchemeFormat.VSCode:
                    return new VsCodeSchemeFileHandler();
                case SchemeFormat.CSS:
                    return new CssFileHandler();                
                default:
                    return null;
            }
        }
        
        [Obsolete] // copy stuff before deleting
        public static string GetRegEx(SchemeFormat schemeFormat)
        {
            // Group that matches actual rgb hex pattern must be named as "hex"
            switch (schemeFormat) {
                case SchemeFormat.Idea:
                    return "<option name=\".+\" value=\"" + PatternHex2to6 + "\"\\s?\\/>";
                case SchemeFormat.VisualStudio:
                    return "ground Type=\".+\" Source=\"" + PatternHex8 + "\" *\\/>";
                case SchemeFormat.VSCode:
                    return "ground\":\\s*\"#" + PatternHex6or8 + "\"";
                case SchemeFormat.CSS:
                    return "color:\\s*#" + PatternHex3or6 + ".*";
                default:
                    return "#" + PatternHex3or6 + ".*";
            }
        }

        [Obsolete] // copy stuff before deleting
        private static PaddableHexFormat[] GetRgbHexFormats(SchemeFormat schemeFormat)
        {
            // Note: if multiple formats are used at the same time and padding is in use,
            // shorter formats must be defined first
            switch (schemeFormat) {
                case SchemeFormat.Idea:
                {
                    return new[]
                    {
                        new PaddableHexFormat()
                        {
                            HexFormat = "rrggbb",
                            Padding = "000000",
                            PaddingDirection = PaddingDirection.Left
                        }
                    };
                }
                case SchemeFormat.VisualStudio:
                {
                    return new[]
                    {
                        new PaddableHexFormat()
                        {
                            HexFormat = "AARRGGBB",
                            Padding = null,
                            PaddingDirection = PaddingDirection.None
                        }
                    };
                }
                case SchemeFormat.VSCode:
                {
                    return new[]
                    {
                        new PaddableHexFormat()
                        {
                            HexFormat = "rrggbbaa",
                            Padding = "000000ff",
                            PaddingDirection = PaddingDirection.Right
                        }
                    };
                }
                case SchemeFormat.CSS:
                default:
                {
                    return new[]
                    {
                        new PaddableHexFormat()
                        {
                            HexFormat = "rgb",
                            Padding = null,
                            PaddingDirection = PaddingDirection.None
                        },
                        new PaddableHexFormat()
                        {
                            HexFormat = "rrggbb",
                            Padding = null,
                            PaddingDirection = PaddingDirection.None
                        }
                    };
                }
            }
        }

        /// <summary>
        /// Converts RGB hex string to Color. Applies padding to short strings by the color scheme padding rules.
        /// </summary>
        /// <param name="rgbString"></param>
        /// <param name="formats"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static Color FromHexString(string rgbString, IEnumerable<PaddableHexFormat> formats)
        {

            foreach (var format in formats) {
                string rgbHexFormat = format.HexFormat;
                if (format.Padding != null && format.Padding.Length != format.HexFormat.Length) {
                    throw new Exception("RGB hex string misconfiguration: " + format.HexFormat + " with padding " +
                                        format.Padding);
                }

                if (HexRgb.IsValidHexString(rgbString) && rgbString.Length <= rgbHexFormat.Length) {
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
                        return HexRgb.FromRgbString(rgbString, rgbHexFormat);
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


    public class PaddableHexFormat
    {
        public string HexFormat { get; set; }
        public string Padding { get; set; }
        public PaddingDirection PaddingDirection { get; set; }
    }

    public enum PaddingDirection
    {
        Left,
        Right,
        None
    }
}