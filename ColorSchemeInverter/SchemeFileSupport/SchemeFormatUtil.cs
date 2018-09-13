namespace ColorSchemeInverter.SchemeFileSupport
{
    public static class SchemeFormatUtil
    {
        private const string PatternHex8 = "([0-9abcdefABCDEF]{8})";
        private const string PatternHex6 = "([0-9abcdefABCDEF]{6})";

        public static SchemeFormat GetFormatFromExtension(string extension)
        {
            if (extension.StartsWith(".")) {
                extension = extension.Substring(1);
            }

            switch (extension) {
                case "icls":
                    return SchemeFormat.Idea;
                case "vstheme":
                    return SchemeFormat.VisualStudio;
                case "png":
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
                    return "(<option name=\".+\" value=\")" + PatternHex6 + "(\"\\s?\\/>)";
                case SchemeFormat.VisualStudio:
                    return "(ground Type=\".+\" Source=\")" + PatternHex8 + "(\" *\\/>)";
                case SchemeFormat.Generic:
                default:
                    return "(#)" + PatternHex6 + "(.*)";
            }
        }

        public static string GetRgbHexFormat(SchemeFormat schemeFormat)
        {
            switch (schemeFormat) {
                case SchemeFormat.Idea:
                    return "rrggbb";
                case SchemeFormat.VisualStudio:
                    return "AARRGGBB";
                default:
                    return "RRGGBB";
            }
        }
    }
}