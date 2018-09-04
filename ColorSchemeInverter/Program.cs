using System;
using System.IO;
using System.Net;

// Todo: Add more filters, at least for adjusting saturation and gamma 
// Todo: Implement RGBA, RGB, argb MatchEvaluator delegates for future needs
// Todo: Delegate functions for filters. Filters for RGB only or also for HSL? or both?
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
                Path.GetFileNameWithoutExtension(sourceFile) + "_inverted"
                                                             + Path.GetExtension(sourceFile)));
            
            if (args.Length == 2) {
                sourceFile = args[0];
                targetFile = args[1];
            }

            SchemeFormat schemeFormat = SchemeFormatUtil.GetFormatFromExtension(Path.GetExtension(sourceFileName));

            if (schemeFormat == SchemeFormat.Idea || schemeFormat == SchemeFormat.VisualStudio) {
                if (File.Exists(sourceFile)) {
                    ColorSchemeProcessor processor = new ColorSchemeProcessor(schemeFormat);
                    processor.Process(sourceFile, targetFile);
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