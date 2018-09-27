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

        public override string ReplaceColors(string text, IEnumerable<Color> colors)
        {
            string converterdText = base.ReplaceColors(text, colors);
//            XElement xElement = XElement.Parse(text);
//            string schemeParent;
//
//            try {
//                var elements = xElement.Elements();
//                var test = xElement.Elements().Where(n => n.Name == "scheme").First();
//                //Console.WriteLine(schemeParent);
//            } catch (Exception e) {
//                Console.WriteLine("error");
//                throw;
//            }

            return converterdText;
        }
    }
}