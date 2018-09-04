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
        public ColorSchemeProcessor(SchemeFormat schemeFormat)
        {
            _schemeFormat = schemeFormat;
        }

        private SchemeFormat _schemeFormat;
        
        public void Process(string sourceFile, string targetFile)
        {
            string text = File.ReadAllText(sourceFile);
            string convertedText;
            try {
                convertedText = ApplyFilters(text);

            } catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
            
            File.WriteAllText(targetFile, convertedText, Encoding.Default);
        }

        private string ApplyFilters(string text)
        {
            string regExPattern = SchemeFormatUtil.GetRegEx(_schemeFormat);
            text = Regex.Replace(text, regExPattern, new MatchEvaluator(MatchReplace));
            return text;
        }

        private string MatchReplace(Match m)
        {
            if (m.Groups.Count == 4) {
                return  m.Groups[1] 
                        + InvertLightness(m.Groups[2].ToString())
                        + m.Groups[3];
            }
            throw new Exception("Regular Expression Mismatch");
        }
        
        private string InvertLightness(string colorString)
        {
            string rgbStringFormat = SchemeFormatUtil.GetRGBStringFromat(_schemeFormat);
            if (IsValidHexString(colorString) && colorString.Length == rgbStringFormat.Length) {
                string converterColorString;
                switch (rgbStringFormat.ToUpper()) {
                    case "RRGGBB":
                        converterColorString = RGB.FromRGBString(colorString).InvertInHSL().ToRGBString();
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