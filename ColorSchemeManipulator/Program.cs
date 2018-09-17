using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ColorSchemeManipulator.CLI;
using ColorSchemeManipulator.Filters;
using ColorSchemeManipulator.SchemeFileSupport;

// 2nd best dark to light inversion so far.. last parts is the neon yellow/green fix
// -ib -lel=0.1,0.9,1,0.1,1 -gas=1.15 -ga(s:0.04-0.3/0.1,l:0-0.2/0.1)=2 -ga(h:40/2-56/20,l:0.04-0.6/0.2)=2 -s(h:40/2-56/20,l:0.04-0.6/0.2)=2 

//  best dark to light inversion so far.. with pre neon adjust -  last parts is the neon yellow/green fix
// -l(b:0.7/0.15-1,s:076/0.1-1)=0.6 -ib -lel=0.1,0.9,1,0.1,1 -ga(h:40/2-56/20,l:0.04-0.6/0.2)=1.7 -s(h:40/2-56/20,l:0.04-0.6/0.2)=1.7 -s=1.1
// -l(b:0.7/0.15-1,s:076/0.1-1)=0.6 -ib -lel=0.1,0.9,1,0.1,1 -ga(h:37/6-56/20,l:0.04-0.6/0.2)=1.7 -s(h:37/6-56/20,l:0.04-0.6/0.2)=1.7 -gas(s:0.1-0.6/0.2)=1.4

namespace ColorSchemeManipulator
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
            int filterCount = CliArgs.GetItems().Count();
            ExperimentalBundle.RegisterCliOptions();
            int experimFilterCount = CliArgs.GetItems().Count() - filterCount;


            Console.WriteLine(
                "Color Scheme Manipulator " + Assembly.GetExecutingAssembly().GetName().Version.ToString());
            // print help
            if (args.Length == 0 || (args.Length == 1 && args[0].ToLower() == "--help")) {
                PrintHelp(filterCount, experimFilterCount);

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

            string sourceFile, targetFile;


            // Test files for debugging
            string sourceFileName = @"HappyDays_Complete.icls";
            // sourceFileName = "darcula-vs-2017.vstheme";
            sourceFileName = "HappyDays.png";
            // sourceFileName = "photo.png";
            string baseDir = System.AppDomain.CurrentDomain.BaseDirectory;
            sourceFile = Path.GetFullPath(Path.Combine(baseDir, sourceFileName));
            targetFile = Path.GetFullPath(Path.Combine(baseDir,
                Path.GetFileNameWithoutExtension(sourceFile) + "_converted"
                                                             + Path.GetExtension(sourceFile)));

            // get source and target from CLI args
            if (remainingArgs.Length == 1) {
                sourceFile = remainingArgs[0];
                targetFile = Path.GetFileNameWithoutExtension(sourceFile) + "_converted"
                                                                          + Path.GetExtension(sourceFile);
            } else if (remainingArgs.Length == 2) {
                sourceFile = remainingArgs[0];
                targetFile = remainingArgs[1];
            } else {
                // Console.WriteLine("Both source and target files must be specified");
                // return;
            }

            SchemeFormat schemeFormat = SchemeFormatUtil.GetFormatFromExtension(Path.GetExtension(sourceFile));

            if (schemeFormat == SchemeFormat.Idea || schemeFormat == SchemeFormat.VisualStudio) {
                if (File.Exists(sourceFile)) {
                    ColorSchemeProcessor processor = new ColorSchemeProcessor(schemeFormat);
                    processor.ProcessFile(sourceFile, targetFile, filterSet);
                    Console.WriteLine("Done.");
                } else {
                    Console.Error.WriteLine(sourceFileName + " does not exist");
                }
            } else if (schemeFormat == SchemeFormat.Image) {
                ImageProcessor processor = new ImageProcessor();
                processor.ProcessFile(sourceFile, targetFile, filterSet);
                Console.WriteLine("Done.");
            } else {
                Console.Error.WriteLine(sourceFile + " is not supported color scheme format");
            }
        }

        private static void PrintHelp(int filterCount = -1, int expermFilterCount = -1)
        {
            Console.WriteLine("Available Filters:");
            if (filterCount == -1 || expermFilterCount == -1) {
                Console.WriteLine(CliArgs.ToString());
                return;
            }
                
            for (int i = 0; i < filterCount; i++) {
                Console.WriteLine("  " + CliArgs.GetItem(i).ToString());
            }

            if (expermFilterCount > 0) {
                Console.WriteLine("Experimental Filters:");
                for (int i = filterCount; i < filterCount + expermFilterCount; i++) {
                    Console.WriteLine("  " + CliArgs.GetItem(i).ToString());
                }
            }
        }

        private static void TestTempStuff() { }
    }
}