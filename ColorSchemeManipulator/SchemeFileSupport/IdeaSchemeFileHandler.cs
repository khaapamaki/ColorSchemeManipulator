namespace ColorSchemeManipulator.SchemeFileSupport
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
        
        protected override string RegexPattern => "<option name=\".+\" value=\"(?<hex>[0-9abcdefABCDEF]{2,6})\"\\s?\\/>";

        protected override string MatchGroupName => "hex";
        
        protected override string OutputHexFormat => "rrggbb";
    }
}