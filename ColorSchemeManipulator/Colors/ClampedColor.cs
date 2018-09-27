namespace ColorSchemeManipulator.Colors
{
    /// <summary>
    /// This is just an example of Color subclass with auto clamping turned on
    /// </summary>
    public class ClampedColor : Color
    {
        public ClampedColor()
        {
            InputInputClamping = Clamping.LowHigh;
        }

        public ClampedColor(Color color) : base(color)
        {
            InputInputClamping = Clamping.LowHigh;
        }

        public new static ClampedColor FromRgb(double r, double g, double b, double a = 1.0)
        {
            var color = new ClampedColor();
            color.SetRgb(r, g, b, a);

            return color;
        }

        public new static ClampedColor FromRgb(byte r, byte g, byte b, byte a = 0xFF)
        {
            var color = new ClampedColor();
            color.SetRgb(r / 255.0, g / 255.0, b / 255.0, a / 255.0);

            return color;
        }

        public new static ClampedColor FromHsl(double h, double s, double l, double a = 1.0)
        {
            var color = new ClampedColor();
            color.SetHsl(h, s, l, a);

            return color;
        }

        public new static ClampedColor FromHsv(double h, double s, double v, double a = 1.0)
        {
            var color = new ClampedColor();
            color.SetHsv(h, s, v, a);

            return color;
        }
    }
}