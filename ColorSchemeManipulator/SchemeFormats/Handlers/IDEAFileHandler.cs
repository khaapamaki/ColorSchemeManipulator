using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ColorSchemeManipulator.Colors;

namespace ColorSchemeManipulator.SchemeFormats.Handlers
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class IDEAFileHandler : HexRgbFileHandler
    {
        private readonly PaddableHexFormat[] _inputHexFormats =
        {
            new PaddableHexFormat()
            {
                HexFormat = "rrggbb",
                Padding = "000000",
                PaddingDirection = PaddingDirection.Left
            }
        };

        public override bool Accepts(string sourceFile)
        {
            string ext = Path.GetExtension(sourceFile)?.ToLower() ?? "";
            return ext == ".icls";
        }

        protected override PaddableHexFormat[] InputHexFormats => _inputHexFormats;

        protected override string RegexPattern =>
            "(?m)<option\\s+name=\"(?<attr>.*)\"\\s*>\\s*<value>\\s*<option\\s+name=\"(?<attr2>.*)\"\\s+value=\"(?<hex>[0-9abcdefABCDEF]{2,6})\"\\s?\\/>"
            + "|<option\\s+name=\"(?<attr2>.*)\"\\s+value=\"(?<hex>[0-9abcdefABCDEF]{2,6})\"\\s?\\/>";

        protected override string MatchGroupName => "hex";

        protected override string OutputHexFormat => "rrggbb";

        public override string ReplaceColors(string xml, IEnumerable<Color> colors)
        {
            string convertedText = base.ReplaceColors(xml, colors);
            Console.Write("SOURCE: ");
            SetParentScheme(xml);
            Console.Write("RESULT: ");
            return SetParentScheme(convertedText);
        }

        /// <summary>
        /// Sets builtin parent scheme to Default for light background schemes, and Darcula for dark
        /// background schemes.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        private static string SetParentScheme(string xml)
        {
            var root = XElement.Parse(xml);
            var textOption = root.Elements("attributes").Descendants("option")
                .First(x => x.Attribute("name")?.Value == "TEXT");
            string textFG = textOption.Descendants("value").Descendants("option")
                .First(x => x.Attribute("name")?.Value == "FOREGROUND").Attribute("value")?.Value;
            string textBG = textOption.Descendants("value").Descendants("option")
                .First(x => x.Attribute("name")?.Value == "BACKGROUND").Attribute("value")?.Value;

            if (!string.IsNullOrEmpty(textFG) && !string.IsNullOrEmpty(textBG) && HexRgbUtil.IsValidHexString(textFG) &&
                HexRgbUtil.IsValidHexString(textBG)) {
                var fg = HexRgbUtil.RGBStringToColor(textFG);
                var bg = HexRgbUtil.RGBStringToColor(textBG);
                Console.WriteLine(
                    $"Background #{textBG} ({bg.CompareValue():F3}), Foreground #{textFG} ({fg.CompareValue():F3})");
                var parentScheme = "Default";
                if (fg.GetBrightness() > bg.GetBrightness())
                    parentScheme = "Darcula";
                root.Attribute("parent_scheme")?.SetValue(parentScheme);
                return root.ToString();
            }

            return xml;
        }
    }
}