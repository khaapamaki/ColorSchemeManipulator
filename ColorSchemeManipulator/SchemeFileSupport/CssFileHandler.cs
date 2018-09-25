namespace ColorSchemeManipulator.SchemeFileSupport
{
    // Todo subclassing SchemeFileHandler is not good enough -> make proper css handler
    public class CssFileHandler : SchemeFileHandler
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