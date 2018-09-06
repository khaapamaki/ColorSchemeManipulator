using System;
using System.Collections.Generic;
using System.IO;
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
            RegisterCliCommands();
            (FilterSet cliFilters, List<string> remainingArgs) = CliArgs.ParseArgs(args);
            args = remainingArgs.ToArray();
            
            Console.WriteLine("Available Filters:");
            Console.WriteLine(CliArgs.ToString());
            
            Console.WriteLine("Asked Filters:");
            Console.WriteLine(cliFilters.ToString());
            
            
            string sourceFileName = @"HappyDays.icls";
            // sourceFileName = "darcula-vs-2017.vstheme";
            
            string baseDir = System.AppDomain.CurrentDomain.BaseDirectory;
            string sourceFile = Path.GetFullPath(Path.Combine(baseDir, sourceFileName));
            string targetFile = Path.GetFullPath(Path.Combine(baseDir,
                Path.GetFileNameWithoutExtension(sourceFile) + "_converted"
                                                             + Path.GetExtension(sourceFile)));
            
            
            // get source and target, if not available use built-in ones for debugging
            // todo: show error if source or target is missing
            if (args.Length == 2) {
                sourceFile = args[0];
                targetFile = args[1];
            }
            
            if (args.Length == 1) {
                targetFile = args[0];
            }

            SchemeFormat schemeFormat = SchemeFormatUtil.GetFormatFromExtension(Path.GetExtension(sourceFileName));


            FilterSet test = FilterBundle.TestGetFilterSet();
 
            if (schemeFormat == SchemeFormat.Idea || schemeFormat == SchemeFormat.VisualStudio) {
                if (File.Exists(sourceFile)) {

                    // var filters = new FilterSet()
                    //     .Add(FilterBundle.LightnessInvert)
                    //     .Add(FilterBundle.SaturationContrast, 0.3)
                    //     .Add(FilterBundle.SaturationGain, 1.2)
                    //     .Add(FilterBundle.Gain, 1.1)
                    //     .Add(FilterBundle.Contrast, 0.3);
   
                    ColorSchemeProcessor processor = new ColorSchemeProcessor(schemeFormat);
                    processor.ProcessFile(sourceFile, targetFile, cliFilters);
                }
                else {
                    Console.Error.WriteLine(sourceFileName + " does not exist");
                }
            } else {
                Console.Error.WriteLine(sourceFileName + " is not supported color scheme format");
            }
            
        }
        
        private bool _isRegisterd = false;
        
        public static void RegisterCliCommands()
        {
            // if (_isRegisterd)
            //     return;

            CliArgs.Register(new List<string> { "-il", "--invert-lightness"}, FilterBundle.LightnessInvert, 0);
            CliArgs.Register(new List<string> { "-s", "--saturation"}, FilterBundle.SaturationGain, 1);
            CliArgs.Register(new List<string> { "-i", "--invert"}, FilterBundle.Invert, 0);
        }
    }
    
    
}