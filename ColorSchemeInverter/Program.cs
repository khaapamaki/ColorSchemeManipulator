using System;
using System.IO;
using ColorSchemeInverter.Filters;
using ColorSchemeInverter.SchemeFileSupport;

// Todo: Filters for levels (gamma, black, white), gamma, gamma for saturation, contrast, gain
// Todo: CLI implementation
// Todo: Add support for CSS
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

            SchemeFormat schemeFormat = SchemeFormatUtil.GetFormatFromExtension(Path.GetExtension(sourceFileName));

            if (schemeFormat == SchemeFormat.Idea || schemeFormat == SchemeFormat.VisualStudio) {
                if (File.Exists(sourceFile)) {

                    var filters = new FilterSet()
                        .Add(FilterBundle.LightnessInvert)
                        .Add(FilterBundle.SaturationGain, 1.5);
                        //.Add(FilterBundle.Invert);
   
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