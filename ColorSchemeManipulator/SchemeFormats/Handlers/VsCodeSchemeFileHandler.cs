namespace ColorSchemeManipulator.SchemeFormats.Handlers
{
    public class VsCodeSchemeFileHandler : SchemeFileHandler
    {   
        private readonly PaddableHexFormat[] _inputHexFormats =
        {
            new PaddableHexFormat()
            {
                HexFormat = "rrggbbaa",
                Padding = "000000ff",
                PaddingDirection = PaddingDirection.Right
            }
        };
        
        protected override string RegexPattern => "ground\":\\s*\"#(?<hex>[0-9abcdefABCDEF]{8}|[0-9abcdefABCDEF]{6})\"";

        protected override PaddableHexFormat[] InputHexFormats => _inputHexFormats;

        protected override string MatchGroupName => "hex";
        
        protected override string OutputHexFormat => "rrggbbaa";
        
    }
}