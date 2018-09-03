namespace ColorSchemeInverter {
    
    public class SchemeFormatUtil
    {
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
                default:
                    return SchemeFormat.Generic;
            }  
        }

        
        public static string GetRegEx(SchemeFormat schemeFormat)
        {
            // Patterns must have three groups, where 2nd must pure hex RGB without any prefixes!
            switch (schemeFormat) {
                case SchemeFormat.Idea:
                    return "(<option name=\".+\" value=\")([0-9abcdef]{6})(\"\\s?\\/>)";
                case SchemeFormat.VisualStudio:
                    return "(ground Type=\".+\" Source=\")([0-9ABCDEF]{8})(\" *\\/>)";
                case SchemeFormat.Generic:
                default:
                    return "(#)([0-9abcdefABCDEF]{6})(.*)";
            }
        }
        
        public static string GetRGBStringFromat(SchemeFormat schemeFormat)
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