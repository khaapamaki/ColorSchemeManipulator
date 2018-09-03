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
            
            RGB rgb = RGB.FromRGBAString("EB996380");
            Console.WriteLine(rgb.ToString());
            Console.WriteLine(rgb.ToHSL().ToString());
            Console.WriteLine(rgb.ToHSV().ToString());
            Console.WriteLine(rgb.ToHSL().ToRGB().ToString());
            Console.WriteLine(rgb.ToHSV().ToRGB().ToString());
            Console.WriteLine(rgb.InvertInHSL().ToString());
            Console.WriteLine(rgb.InvertInHSV().ToString());
            
            //ColorSchemeProcessor.InvertColors(sourceFile, targetFile);
            
        }
    }
}