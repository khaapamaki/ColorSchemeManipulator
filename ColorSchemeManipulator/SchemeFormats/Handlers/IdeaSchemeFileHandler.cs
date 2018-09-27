using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Xml.Linq;
using ColorSchemeManipulator.Colors;

namespace ColorSchemeManipulator.SchemeFormats.Handlers
{
    public class IdeaSchemeFileHandler : SchemeFileHandler
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

        protected override PaddableHexFormat[] InputHexFormats => _inputHexFormats;

        protected override string RegexPattern =>
            "<option name=\".+\" value=\"(?<hex>[0-9abcdefABCDEF]{2,6})\"\\s?\\/>";

        protected override string MatchGroupName => "hex";

        protected override string OutputHexFormat => "rrggbb";

        public override string ReplaceColors(string xml, IEnumerable<Color> colors)
        {
            string convertedText = base.ReplaceColors(xml, colors);
            return SetParentScheme(convertedText);
        }

        private static string SetParentScheme(string xml)
        {
            var root = XElement.Parse(xml);
            var textOption = root.Elements("attributes").Descendants("option").First(x => x.Attribute("name")?.Value == "TEXT");
            string textFG = textOption.Descendants("value").Descendants("option").First(x => x.Attribute("name")?.Value == "FOREGROUND").Attribute("value")?.Value;
            string textBG = textOption.Descendants("value").Descendants("option").First(x => x.Attribute("name")?.Value == "BACKGROUND").Attribute("value")?.Value;
            
            if (!string.IsNullOrEmpty(textFG) && !string.IsNullOrEmpty(textBG) && HexRgb.IsValidHexString(textFG) && HexRgb.IsValidHexString(textBG))
            {
                Color fg = HexRgb.FromRgbString(textFG);
                Color bg = HexRgb.FromRgbString(textBG);
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