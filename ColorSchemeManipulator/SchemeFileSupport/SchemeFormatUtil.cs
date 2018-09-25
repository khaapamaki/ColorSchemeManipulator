using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace ColorSchemeManipulator.SchemeFileSupport
{
    public static class SchemeFormatUtil
    {
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
                    return SchemeFormat.Generic;
            }
        }

        public static string GetRegEx(SchemeFormat schemeFormat)
        {
            // Patterns must have three groups, where 2nd must pure hex RGB without any prefixes!
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

        [Obsolete]
        public static string GetRgbHexFormat(SchemeFormat schemeFormat)
        {
            switch (schemeFormat) {
                case SchemeFormat.Idea:
                    return "rrggbb";
                case SchemeFormat.VisualStudio:
                    return "AARRGGBB";
                case SchemeFormat.VSCode:
                    return "rrggbbaa";
                case SchemeFormat.CSS:
                    return "rrggbb";
                default:
                    return "rrggbb";
            }
        }

        public static RgbHexFormatSpecs[] GetRgbHexFormats(SchemeFormat schemeFormat)
        {
            // Note: if multiple formats are used at the same time and padding is in use,
            // shorter formats must be defined first
            switch (schemeFormat) {
                case SchemeFormat.Idea:
                {
                    return new[]
                    {
                        new RgbHexFormatSpecs()
                        {
                            RgbHexFormat = "rrggbb",
                            Padding = "000000",
                            PaddingDirection = PaddingDirection.Left
                        }
                    };
                }
                case SchemeFormat.VisualStudio:
                {
                    return new[]
                    {
                        new RgbHexFormatSpecs()
                        {
                            RgbHexFormat = "AARRGGBB",
                            Padding = null,
                            PaddingDirection = PaddingDirection.None
                        }
                    };
                }
                case SchemeFormat.VSCode:
                {
                    return new[]
                    {
                        new RgbHexFormatSpecs()
                        {
                            RgbHexFormat = "rrggbbaa",
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
                        new RgbHexFormatSpecs()
                        {
                            RgbHexFormat = "rgb",
                            Padding = null,
                            PaddingDirection = PaddingDirection.None
                        },
                        new RgbHexFormatSpecs()
                        {
                            RgbHexFormat = "rrggbb",
                            Padding = null,
                            PaddingDirection = PaddingDirection.None
                        }
                    };
                }
            }
        }
    }

    public class RgbHexFormatSpecs
    {
        public string RgbHexFormat { get; set; }
        public string Padding { get; set; }
        public PaddingDirection PaddingDirection { get; set; }
    }

    public enum PaddingDirection
    {
        Left,
        Right,

        // Middle,
        None
    }
}