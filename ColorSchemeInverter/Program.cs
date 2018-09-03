using System;
using System.IO;
using System.Net;

namespace ColorSchemeInverter
{
    internal class Program
    {
        public static void Main(string[] args)
        {           
            string sourceFileName = @"HappyDays.icls";
            // sourceFileName = "darcula-vs-2017.vstheme";

            string baseDir = System.AppDomain.CurrentDomain.BaseDirectory;
            string sourceFile = Path.GetFullPath(Path.Combine(baseDir, sourceFileName));
            string targetFile = Path.GetFullPath(Path.Combine(baseDir,
                Path.GetFileNameWithoutExtension(sourceFile) + "_inverted"
                                                             + Path.GetExtension(sourceFile)
            ));
            
            if (args.Length == 2) {
                sourceFile = args[0];
                targetFile = args[1];
            }

            /*
            Console.WriteLine("Source path: " + sourceFile);
            Console.WriteLine("Target path: " + targetFile);
            RGB rgb = RGB.FromRGBAString("EB996380");
            Console.WriteLine(rgb.ToString());
            Console.WriteLine(rgb.ToHSL().ToString());
            Console.WriteLine(rgb.ToHSV().ToString());
            Console.WriteLine(rgb.ToHSL().ToRGB().ToString());
            Console.WriteLine(rgb.ToHSV().ToRGB().ToString());
            Console.WriteLine(rgb.InvertInHSL().ToString());
            Console.WriteLine(rgb.InvertInHSV().ToString());
            */

            SchemeFormat schemeFormat = SchemeFormatUtil.GetFormatFromExtension(Path.GetExtension(sourceFileName));

            if (schemeFormat == SchemeFormat.Idea || schemeFormat == SchemeFormat.VisualStudio) {
                if (File.Exists(sourceFile))
                    ColorSchemeProcessor.InvertColors(sourceFile, targetFile, schemeFormat);
                else {
                    Console.Error.WriteLine(sourceFileName + " does not exist");
                }
            } else {
                Console.Error.WriteLine(sourceFileName + " is not supported color scheme format");
            }

        }
    }
}