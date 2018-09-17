using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ColorSchemeManipulator.CLI;
using ColorSchemeManipulator.Common;
using ColorSchemeManipulator.Filters;
using ColorSchemeManipulator.SchemeFileSupport;

namespace ColorSchemeManipulator
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            // Make FilterBundle filters available for CLI
            FilterBundle.RegisterCliOptions();
            int filterCount = CliArgs.GetItems().Count();
            int experimFilterCount = 0;
            
            // Comment out these if you want to exclude experimental filters
            ExperimentalBundle.RegisterCliOptions();
            experimFilterCount = CliArgs.GetItems().Count() - filterCount;

            Console.WriteLine(
                "Color Scheme Manipulator " + Assembly.GetExecutingAssembly().GetName().Version.ToString());
            // print help
            if (args.Length == 0 || (args.Length == 1 && args[0].ToLower() == "--help")) {
                Utils.PrintHelp(filterCount, experimFilterCount);
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
                Console.WriteLine("No source file specified");
                return;
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

    }
}