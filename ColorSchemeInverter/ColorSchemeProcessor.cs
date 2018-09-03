using System;
using System.Linq;
using System.Xml.Linq;

namespace ColorSchemeInverter
{
    public class ColorSchemeProcessor
    {
        public static void InvertColors(string sourceFile, string targetFile)
        {
            DigIntoXML(sourceFile);
        }

        // temp method for testing stuff
        private static bool DigIntoXML(string sourceFile)
        {
            bool succeeded = false;
            try {
                var xData = XDocument.Load(sourceFile);
                var xRoot = xData.Root;
                // string colorString = get_from_xml...
                // colorString = InvertColor(colorString);
                succeeded = true;
            } catch (Exception e) {
                Console.Error.WriteLine(e.ToString());
            }

            return succeeded;
        }

        private static string InvertColor(string colorString, bool alphaFirst = true)
        {            
            string rgbString = colorString.ToUpper();
            bool uppercaseHex = rgbString == colorString;
            string prefix = "";
            if (rgbString.StartsWith("#")) {
                rgbString = rgbString.Substring(1);
                prefix = colorString.Substring(0, 1);
            } else if (colorString.StartsWith("0x")) {
                rgbString = rgbString.Substring(2);
                prefix = colorString.Substring(0, 2);
            }

            if (IsValidHexString(rgbString)) {      
                if (rgbString.Length == 6) {
                   rgbString = RGB.FromRGB(rgbString).InvertInHSL().ToRGBString();
                } else if (rgbString.Length == 8) {
                    if (alphaFirst) {
                        rgbString = RGB.FromARGB(rgbString).InvertInHSL().ToARGBString();
                    } else {
                        rgbString = RGB.FromRGBA(rgbString).InvertInHSL().ToRGBAString();
                    }
                } else {
                    return colorString;
                }
                colorString = prefix + (uppercaseHex ? rgbString.ToUpper() : rgbString.ToLower());
            }

            return colorString;
        }

        private static bool IsValidHexString(string str)
        {
            const string validHex = "0123456789abcdefABCDEF";
            foreach (var c in str) {
                if (!validHex.Contains(c))
                    return false;
            }
            return true;
        } 
    }
}