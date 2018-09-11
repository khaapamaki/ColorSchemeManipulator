using System;
using System.Drawing;
using ColorSchemeInverter.Colors;
using ColorSchemeInverter.Filters;
using ColorSchemeInverter.SchemeFileSupport;

namespace ColorSchemeInverter
{
    public class ImageProcessor
    {

        public ImageProcessor()
        {
        }
        
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
             
        private Bitmap ApplyFilters(Bitmap image, FilterSet filters)
        {    
 
            for (int x = 0; x < image.Width; x++) {
                for (int y = 0; y < image.Height; y++) {
                    var pixel = image.GetPixel(x, y);
                    image.SetPixel(x, y, ApplyFilters(pixel, filters));
                } 
            }
            
            return image;
        }

        private Color ApplyFilters(Color color, FilterSet filters)
        {
            return ColorConversions.RgbToSystemColor(
                ColorConversions.SystemColorToRgb(color).ApplyFilterSet(filters));
        }
        
    }
}