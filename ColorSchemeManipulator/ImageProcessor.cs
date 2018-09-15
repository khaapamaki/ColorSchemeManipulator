using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using ColorSchemeManipulator.Colors;
using ColorSchemeManipulator.Filters;

namespace ColorSchemeManipulator
{
    public class ImageProcessor
    {
        public ImageProcessor() { }

        public void ProcessFile(string sourceFile, string targetFile, BatchFilterSet filters)
        {
            var image = Image.FromFile(sourceFile);
            var bitmap = new Bitmap(image);
            Console.WriteLine($"{bitmap.Width},{bitmap.Height}");
            Console.WriteLine("Pixels: " +
                              Enumerate(bitmap).Count());
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

        public IEnumerable<ColorBase> Enumerate(Bitmap bitmap)
        {
            for (int y = 0; y < bitmap.Height; y++) {
                for (int x = 0; x < bitmap.Width; x++) {
                    yield return ColorConversions.SystemColorToRgb(bitmap.GetPixel(x, y));
                }
            }
        }

        public Bitmap SetPixels(Bitmap original, IEnumerable<ColorBase> colors)
        {
            int x = 0;
            int y = 0;
            foreach (ColorBase color in colors) {
                if (y >= original.Height || x >= original.Width)
                    break;
                original.SetPixel(x, y, color.ToSystemColor());
                x++;
                if (x >= original.Width) {
                    x = 0;
                    y++;
                }
            }

            Console.WriteLine($"{x},{y}");
            return original;
        }


        //        private IEnumerable<ColorBase> ApplyFilters(IEnumerable<ColorBase> colorSet, BatchFilterSet filters)
        //        {
        //            foreach (var color in colorSet) {
        //                color.ApplyFilterSet(filters);
        //                yield return color;
        //            }
        //        }

        // private  IEnumerable<ColorBase> ApplyFilters(Bitmap image, BatchFilterSet filters)
        // {
        //     
        //     foreach (var color in filters.ApplyTo(Enumerate(image))) {
        //         yield return color;
        //     }
        //
        // }

        public Bitmap ApplyFilters(Bitmap bitmap, BatchFilterSet filters)
        {
            SetPixels(bitmap, filters.ApplyTo(Enumerate(bitmap)));
            return bitmap;
        }

        // todo THIS CURRENTLY DOES NOTHING
        private Color ApplyFilters(Color color, BatchFilterSet filters)
        {
            return ColorConversions.RgbToSystemColor(
                ColorConversions.SystemColorToRgb(color)); // .ApplyFilterSet(filters));
        }
    }
}