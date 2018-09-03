using System;

namespace ColorSchemeInverter
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            string sourceFile = @"";
            string targetFile = @"";

            if (args.Length == 2) {
                sourceFile = args[0];
                targetFile = args[1];
            }
            
            RGB rgb = RGB.FromRGB("fa0000");
            Console.WriteLine(rgb.ToARGBString());
            Console.WriteLine(rgb.InvertInHSL().ToRGBString());
            
            //ColorSchemeProcessor.InvertColors(sourceFile, targetFile);
            
        }
    }
}