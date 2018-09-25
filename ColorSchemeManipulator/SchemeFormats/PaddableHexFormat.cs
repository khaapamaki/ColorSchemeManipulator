namespace ColorSchemeManipulator.SchemeFormats
{
    public enum PaddingDirection
    {
        Left,
        Right,
        None
    }

    public class PaddableHexFormat
    {
        public string HexFormat { get; set; }
        public string Padding { get; set; }
        public PaddingDirection PaddingDirection { get; set; }
    }
}