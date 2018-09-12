using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using ColorSchemeInverter.CLI;
using ColorSchemeInverter.Colors;
using ColorSchemeInverter.Common;
using ColorSchemeInverter.Filters;
using ColorSchemeInverter.SchemeFileSupport;
    
    // best dark to light inversion so far.. last parts is the neon yellow/green fix
    // -ib -lel=0.1,0.9,1,0.1,1 -gas=1.15 -ga(s:0.04-0.3/0.1,l:0-0.2/0.1)=2 -ga(h:40/2-56/20,l:0.04-0.6/0.2)=2 -s(h:40/2-56/20,l:0.04-0.6/0.2)=2 

namespace ColorSchemeInverter
{
    internal class Program
    {
        public static void Main(string[] args)
        {
#if DEBUG
            // TestTempStuff();
#endif
            // Make FilterBundle filters available for CLI
            FilterBundle.RegisterCliOptions();

            // print help
            if (args.Length == 0 || (args.Length == 1 && args[0].ToLower() == "--help")) {
                Console.WriteLine("Available Filters:");
                Console.WriteLine(CliArgs.ToString());
                return;
            }

            // Parse CLI args and generate FilterSet of them
            (FilterSet filterSet, string[] remainingArgs) = CliArgs.ParseFilterArgs(args);

            // Extract non-option and remaining option arguments
            string[] remainingOptArgs;
            (remainingArgs, remainingOptArgs) = CliArgs.ExtractOptionArguments(remainingArgs);

            // PARSE other than filter options here, and remove them from remainingOptArgs array

            // All remaining option arguments are considered illegal
            if (remainingOptArgs.Length > 0) {
                Console.WriteLine("Illegal argument: " + remainingOptArgs[0]);
                return;
            }


            Console.WriteLine("Applying filters:");
            Console.WriteLine(filterSet.ToString());

            // Test files for debugging
            string sourceFileName = @"HappyDays.icls";
            // sourceFileName = "darcula-vs-2017.vstheme";
            sourceFileName = "HappyDays.png";
            // sourceFileName = "photo.png";

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
                    ColorSchemeProcessor processor = new ColorSchemeProcessor(schemeFormat);
                    processor.ProcessFile(sourceFile, targetFile, filterSet);
                } else {
                    Console.Error.WriteLine(sourceFileName + " does not exist");
                }
            } else if (schemeFormat == SchemeFormat.Image) {
                ImageProcessor processor = new ImageProcessor();
                processor.ProcessFile(sourceFile, targetFile, filterSet);
            } else {
                Console.Error.WriteLine(sourceFileName + " is not supported color scheme format");
            }

            IEnumerable<string> testEnumerable;
            
        }

        private static void TestTempStuff()
        {
            Hsl hsl = new Hsl(180, 1, 0.99, 0.5);
            Hsv hsv = new Hsv(hsl);

            Console.WriteLine(hsl);
            Console.WriteLine(hsv);
            hsl = hsv.ToHsl();
            Console.WriteLine(hsl);

            Rgb rgb = new Rgb(0.5, 0.5, 0.25, 0.5);
            hsl = rgb.ToHsl();
            Console.WriteLine(hsl);
            hsv = rgb.ToHsv();
            Console.WriteLine(hsv);
        }
    }
}