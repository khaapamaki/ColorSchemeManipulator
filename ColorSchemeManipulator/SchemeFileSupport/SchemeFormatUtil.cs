namespace ColorSchemeManipulator.SchemeFileSupport
{
    public static class SchemeFormatUtil
    {
        private const string PatternHex8 = "(?<hex>[0-9abcdefABCDEF]{8})";
        private const string PatternHex6 = "(?<hex>[0-9abcdefABCDEF]{6})";
        private const string PatternHex6or8= "(?<hex>[0-9abcdefABCDEF]{8}|[0-9abcdefABCDEF]{6})";
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
                    return "(<option name=\".+\" value=\")" + PatternHex2to6 + "(\"\\s?\\/>)";
                case SchemeFormat.VisualStudio:
                    return "(ground Type=\".+\" Source=\")" + PatternHex8 + "(\" *\\/>)";
                case SchemeFormat.VSCode:
                    return "ground\":\\s*\"#" + PatternHex6or8 + "\"";
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
                case SchemeFormat.VSCode:
                    return "rrggbb"; // todo change to rrggbbaa when padding system implemented
                default:
                    return "RRGGBB";
            }
        }
    }
}