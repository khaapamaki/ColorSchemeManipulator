using System;
using System.Collections.Generic;
using System.Drawing;
using ColorSchemeManipulator.Colors;
using ColorSchemeManipulator.Filters;
<<<<<<< HEAD
=======
using Color = ColorSchemeManipulator.Colors.Color;
>>>>>>> staging

namespace ColorSchemeManipulator
{
    public class ImageProcessor
    {
        public ImageProcessor() { }

        public void ProcessFile(string sourceFile, string targetFile, FilterSet filters)
        {
            var image = Image.FromFile(sourceFile);
            var bitmap = new Bitmap(image);
            Bitmap convertedImage;
            try {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                convertedImage = ApplyFilters(bitmap, filters);
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine($"Image converted in {elapsedMs} ms");
            } catch (Exception ex) {
                Console.WriteLine(GetType().FullName + " : " + ex.Message);
                throw;
            }

            convertedImage.Save(targetFile);
        }

        public static IEnumerable<Color> Enumerate(Bitmap bitmap)
        {
            for (int y = 0; y < bitmap.Height; y++) {
                for (int x = 0; x < bitmap.Width; x++) {
                    yield return ColorConversions.SystemColorToColor(bitmap.GetPixel(x, y));
                }
            }
        }

        public static void SetPixels(Bitmap original, IEnumerable<Color> colors)
        {
            int x = 0;
            int y = 0;
            foreach (Color color in colors) {
                if (y >= original.Height || x >= original.Width)
                    break;
                original.SetPixel(x, y, ColorConversions.ColorToSystemColor(color));
                x++;
                if (x >= original.Width) {
                    x = 0;
                    y++;
                }
            }
        }

        public Bitmap ApplyFilters(Bitmap bitmap, FilterSet filters)
        {
            SetPixels(bitmap, filters.ApplyTo(Enumerate(bitmap)));
            return bitmap;
        }
    }
}