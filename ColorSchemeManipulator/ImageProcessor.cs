using System;
using System.Drawing;
using ColorSchemeManipulator.Colors;
using ColorSchemeManipulator.Filters;

namespace ColorSchemeManipulator
{
    public class ImageProcessor
    {

        public ImageProcessor()
        {
        }
        
        public void ProcessFile(string sourceFile, string targetFile, BatchFilterSet filters)
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
             
        private Bitmap ApplyFilters(Bitmap image, BatchFilterSet filters)
        {    
 
            for (int x = 0; x < image.Width; x++) {
                for (int y = 0; y < image.Height; y++) {
                    var pixel = image.GetPixel(x, y);
                    image.SetPixel(x, y, ApplyFilters(pixel, filters));
                } 
            }
            
            return image;
        }
        // todo THIS CURRENTLY DOES NOTHING
        private Color ApplyFilters(Color color, BatchFilterSet filters)
        {
            return ColorConversions.RgbToSystemColor(
                ColorConversions.SystemColorToRgb(color)); // .ApplyFilterSet(filters));
        }
        
    }
}