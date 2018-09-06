using System;
using System.Collections.Generic;
using System.IO;
using ColorSchemeInverter.CLI;
using ColorSchemeInverter.Filters;
using ColorSchemeInverter.SchemeFileSupport;

// Todo: Filters for levels (gamma, black, white), gain
// Todo: CLI implementation
// Todo: Add support for CSS
// Issues: HSV ValueInversion produces bad results. HSV could be dropped out

namespace ColorSchemeInverter
{
    internal class Program
    {
        public static void Main(string[] args)
        {           
            // Make FilterBundle filters available for CLI
            FilterBundle.RegisterCliOptions();
            
            // Parse CLI args and generate FilterSet of them
            (FilterSet filterSet, string[] remainingArgs) = CliArgs.ParseArgs(args);
            
            Console.WriteLine("Available Filters:");
            Console.WriteLine(CliArgs.ToString());
            
            Console.WriteLine("Filters to be applied:");
            Console.WriteLine(filterSet.ToString());
            
            // Test files for debugging
            string sourceFileName = @"HappyDays.icls";
            // sourceFileName = "darcula-vs-2017.vstheme";
            
            string baseDir = System.AppDomain.CurrentDomain.BaseDirectory;
            string sourceFile = Path.GetFullPath(Path.Combine(baseDir, sourceFileName));
            string targetFile = Path.GetFullPath(Path.Combine(baseDir,
                Path.GetFileNameWithoutExtension(sourceFile) + "_converted"
                                                             + Path.GetExtension(sourceFile)));
            
            // get source and target from CLI args, if not available use built-in ones for debugging
            // todo: show error if source or target is missing
            if (remainingArgs.Length == 2) {
                sourceFile = args[0];
                targetFile = args[1];
            }
            
            if (remainingArgs.Length == 1) {
                targetFile = args[0];
            }

            SchemeFormat schemeFormat = SchemeFormatUtil.GetFormatFromExtension(Path.GetExtension(sourceFileName));
 
            if (schemeFormat == SchemeFormat.Idea || schemeFormat == SchemeFormat.VisualStudio) {
                if (File.Exists(sourceFile)) {

                    // old testing, now ClI parsing is in use:
                    // var filters = new FilterSet()
                    //     .Add(FilterBundle.LightnessInvert)
                    //     .Add(FilterBundle.SaturationContrast, 0.3)
                    //     .Add(FilterBundle.SaturationGain, 1.2)
                    //     .Add(FilterBundle.Gain, 1.1)
                    //     .Add(FilterBundle.Contrast, 0.3);
   
                    ColorSchemeProcessor processor = new ColorSchemeProcessor(schemeFormat);
                    processor.ProcessFile(sourceFile, targetFile, filterSet);
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