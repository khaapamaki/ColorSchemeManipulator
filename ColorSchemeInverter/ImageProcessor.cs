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
            Image image = Image.FromFile(sourceFile);
            Bitmap bitmap = new Bitmap(image);
            Bitmap convertedImage;
            try {
                convertedImage = ApplyFilters(bitmap, filters);
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
                    Color pixel = image.GetPixel(x, y);
                    image.SetPixel(x, y, ApplyFilters(pixel, filters));
                } 
            }
            
            return image;
        }

        private Color ApplyFilters(Color color, FilterSet filters)
        {
            return ColorConversions.RGBToSystemColor(
                ColorConversions.SystemColorToRGB(color).ApplyFilterSet(filters));
        }
        
    }
}