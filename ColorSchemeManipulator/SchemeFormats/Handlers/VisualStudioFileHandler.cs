using System.IO;

namespace ColorSchemeManipulator.SchemeFormats.Handlers
{
    public class VisualStudioFileHandler : HexRgbFileHandler
    {   
        private readonly PaddableHexFormat[] _inputHexFormats =
        {
            new PaddableHexFormat()
            {
                HexFormat = "AARRGGBB",
                Padding = null,
                PaddingDirection = PaddingDirection.None
            }
        };
        
        public override bool Accepts(string sourceFile)
        {
            string ext= Path.GetExtension(sourceFile)?.ToLower() ?? "";
            return ext == ".vstheme";
        }
        
        protected override PaddableHexFormat[] InputHexFormats => _inputHexFormats;

        protected override string RegexPattern => "ground Type=\".+\" Source=\"(?<hex>[0-9abcdefABCDEF]{8})\" *\\/>";

        protected override string MatchGroupName => "hex";
        
        protected override string OutputHexFormat => "AARRGGBB";
        
    }
}