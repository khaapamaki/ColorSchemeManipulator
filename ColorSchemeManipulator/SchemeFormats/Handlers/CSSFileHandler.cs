using System.Diagnostics.CodeAnalysis;

namespace ColorSchemeManipulator.SchemeFormats.Handlers
{
    // Todo subclassing HexRgbFileHandler is not good enough -> make proper css handler
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class CSSFileHandler : HexRgbFileHandler
    {
        private readonly PaddableHexFormat[] _inputHexFormats =
        {
            new PaddableHexFormat()
            {
                HexFormat = "rgb",
                Padding = null,
                PaddingDirection = PaddingDirection.None
            },
            
            new PaddableHexFormat()
            {
                HexFormat = "rrggbb",
                Padding = null,
                PaddingDirection = PaddingDirection.None
            }
        };
        
        protected override PaddableHexFormat[] InputHexFormats => _inputHexFormats;
        
        protected override string RegexPattern => "color:\\s*#(?<hex>[0-9abcdefABCDEF]{6}|[0-9abcdefABCDEF]{3}).*";

        protected override string MatchGroupName => "hex";
        
        protected override string OutputHexFormat => "rrggbb";
    }
}