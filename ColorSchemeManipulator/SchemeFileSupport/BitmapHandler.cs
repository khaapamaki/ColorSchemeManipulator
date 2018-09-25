using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using ColorSchemeManipulator.Colors;
using Color = ColorSchemeManipulator.Colors.Color;


namespace ColorSchemeManipulator.SchemeFileSupport
{
    public class BitmapHandler : IColorFileHandler<Bitmap>
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
            // List<Color> colors = new List<Color>();
            
            for (int y = 0; y < source.Height; y++) {
                for (int x = 0; x < source.Width; x++) {
                    yield return ColorConversions.SystemColorToColor(source.GetPixel(x, y));
                }
            }

            // for (int y = 0; y < source.Height; y++) {
            //     for (int x = 0; x < source.Width; x++) {
            //         colors.Add(ColorConversions.SystemColorToColor(source.GetPixel(x, y)));
            //     }
            // }
            //
            // return colors;
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