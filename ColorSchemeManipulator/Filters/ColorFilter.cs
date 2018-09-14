using ColorSchemeManipulator.Colors;

namespace ColorSchemeManipulator.Filters
{
    public abstract class ColorFilter
    {
        public abstract ColorBase ApplyTo(ColorBase color);
        public abstract override string ToString();
    }
}