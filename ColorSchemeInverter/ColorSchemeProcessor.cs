using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace ColorSchemeInverter
{
    public class ColorSchemeProcessor
    {
        public static void InvertColors(string sourceFile, string targetFile, SchemeFormat schemeFormat)
        {
            string text = File.ReadAllText(sourceFile);
            text = ReplaceColorsWithInvertedOnes(text, schemeFormat);
            File.WriteAllText(targetFile, text, Encoding.Default);
        }

        private static string ReplaceColorsWithInvertedOnes(string text, SchemeFormat schemeFormat)
        {
            
            MatchEvaluator matchEvaluator;
            switch (schemeFormat) {
                    case SchemeFormat.Idea:
                        matchEvaluator = new MatchEvaluator(MatchReplaceRGBLowercase);
                        break;
                    case SchemeFormat.VisualStudio:
                        matchEvaluator = new MatchEvaluator(MatchReplaceARGBUppercase);
                        break;
                    default:
                        throw new NotImplementedException();
            }
                
                
            string regExPattern = SchemeFormatUtil.GetRegEx(schemeFormat);
            string rgbStringFormat = SchemeFormatUtil.GetRGBStringFromat(schemeFormat);

            text = Regex.Replace(text, regExPattern, matchEvaluator);

            Console.WriteLine(text);
            return text;
        }

        private static string MatchReplaceRGBLowercase(Match m)
        {
            if (m.Groups.Count == 4) {
                return  m.Groups[1] 
                    + ProcessColorString(m.Groups[2].ToString(), "rrggbb")
                    + m.Groups[3];
            }
            throw new Exception("Regular Expression Mismatch");
        }
        
        private static string MatchReplaceARGBUppercase(Match m)
        {
            if (m.Groups.Count == 4) {
                return  m.Groups[1] 
                        + ProcessColorString(m.Groups[2].ToString(), "AARRGGBB")
                        + m.Groups[3];
            }
            throw new Exception("Regular Expression Mismatch");
        }
        
        private static string ProcessColorString(string colorString, string rgbStringFormat)
        {
            if (IsValidHexString(colorString) && colorString.Length == rgbStringFormat.Length) {
                string converterColorString;
                switch (rgbStringFormat.ToUpper()) {
                    case "RRGGBB":
                        converterColorString = RGB.FromRGBString(colorString).InvertInHSV().ToRGBString();
                        break;
                    case "AARRGGBB":
                        converterColorString = RGB.FromARGBString(colorString).InvertInHSL().ToARGBString();
                        break;
                    case "RRGGBBAA":
                        converterColorString = RGB.FromRGBAString(colorString).InvertInHSL().ToRGBAString();
                        break;
                    default:
                        converterColorString = colorString;
                        break;
                }

                return IsUppercase(rgbStringFormat) ? converterColorString.ToUpper() : converterColorString.ToLower();
            }

            // something went wrong, just return input untouched
            return colorString;
        }

        private static bool IsUppercase(string str)
        {
            return str.ToUpper() == str;
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