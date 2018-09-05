using System;
using System.Collections.Concurrent;
using System.IO;
using ColorSchemeInverter.Filters;
using ColorSchemeInverter.SchemeFormat;

// Todo: Add more filters, at least for adjusting saturation and gamma 

// Todo: Change RGB presentation and Alpha values from byte to double to make conversions virtually lossless

// Todo: Color superclass for RGB and HSL so it's possible to build generic filters that accept any color type and automatically make conversion if needed

// Todo: Add support for CSS

// Todo: Filters for levels (gamma, black, white) adjustements, gamma adjustment for saturation, contrast

// Issues: HSV ValueInversion produces bad results. HSV could be dropped out


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
                Path.GetFileNameWithoutExtension(sourceFile) + "_converted"
                                                             + Path.GetExtension(sourceFile)));
            
            if (args.Length == 2) {
                sourceFile = args[0];
                targetFile = args[1];
            }

            SchemeFormat.SchemeFormat schemeFormat = SchemeFormatUtil.GetFormatFromExtension(Path.GetExtension(sourceFileName));

            if (schemeFormat == SchemeFormat.SchemeFormat.Idea || schemeFormat == SchemeFormat.SchemeFormat.VisualStudio) {
                if (File.Exists(sourceFile)) {

                    var filters = new FilterSet()
                        .Add(FilterBundle.InvertLightness)
                        .Add(FilterBundle.MultiplySaturation, 1.5)
                        .Add(FilterBundle.Invert);
   
                    ColorSchemeProcessor processor = new ColorSchemeProcessor(schemeFormat);
                    processor.ProcessFile(sourceFile, targetFile, filters);
                }
                else {
                    Console.Error.WriteLine(sourceFileName + " does not exist");
                }
            } else {
                Console.Error.WriteLine(sourceFileName + " is not supported color scheme format");
            }

        }
    }
}