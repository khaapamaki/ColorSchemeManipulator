namespace ColorSchemeManipulator.SchemeFileSupport
{
    /// <summary>
    /// Holds information of regex match to make replacements later
    /// </summary>
    public class ColorMatch
    {
        public int Index;
        public int Length;
        public string MatchingString;
        public string ReplacementString;
    }
}