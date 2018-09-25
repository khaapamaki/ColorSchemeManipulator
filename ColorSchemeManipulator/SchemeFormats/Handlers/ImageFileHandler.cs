using System.Collections.Generic;
using System.Drawing;
using ColorSchemeManipulator.Colors;
using Color = ColorSchemeManipulator.Colors.Color;


namespace ColorSchemeManipulator.SchemeFormats.Handlers
{
    public class ImageFileHandler : IColorFileHandler<Bitmap>
    {
        public Bitmap ReadFile(string sourceFile)
        {
            return new Bitmap(Image.FromFile(sourceFile));
        }

        public void WriteFile(Bitmap image, string targetFile)
        {
            image.Save(targetFile);
        }
        
        public IEnumerable<Color> GetColors(Bitmap source)
        {                       
            for (int y = 0; y < source.Height; y++) {
                for (int x = 0; x < source.Width; x++) {
                    yield return ColorConversions.SystemColorToColor(source.GetPixel(x, y));
                }
            }
        }
        

        public Bitmap ReplaceColors(Bitmap source, IEnumerable<Color> colors)
        {        
            int x = 0;
            int y = 0;
            foreach (var color in colors) {
                if (y >= source.Height || x >= source.Width)
                    break;
                source.SetPixel(x, y, ColorConversions.ColorToSystemColor(color));
                x++;
                if (x >= source.Width) {
                    x = 0;
                    y++;
                }
            }

            return source;
        }
    }
}