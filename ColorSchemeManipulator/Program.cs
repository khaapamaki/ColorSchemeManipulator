using System;
using System.IO;
using ColorSchemeManipulator.CLI;
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

            // print help
            if (args.Length == 0 || (args.Length == 1 && args[0].ToLower() == "--help")) {
                Console.WriteLine("Available Filters:");
                Console.WriteLine(CliArgs.ToString());
                // todo print usage examples
                return;
            }

            // Parse CLI args and generate FilterSet of them
            (var filterSet, string[] remainingArgs) = CliArgs.ParseFilterArgs(args);

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
            
            // get source and target from CLI args

            if (remainingArgs.Length == 2) {
                sourceFile = remainingArgs[0];
                targetFile = remainingArgs[1];
            } else {
                Console.WriteLine("Both source and target files must be specified");
                return;
            }

            var schemeFormat = SchemeFormatUtil.GetFormatFromExtension(Path.GetExtension(sourceFile));

            if (schemeFormat == SchemeFormat.Idea || schemeFormat == SchemeFormat.VisualStudio) {
                if (File.Exists(sourceFile)) {
                    var processor = new ColorSchemeProcessor(schemeFormat);
                    processor.ProcessFile(sourceFile, targetFile, filterSet);
                } else {
                    Console.Error.WriteLine(sourceFile + " does not exist");
                }
            } else if (schemeFormat == SchemeFormat.Image) {
                var processor = new ImageProcessor();
                processor.ProcessFile(sourceFile, targetFile, filterSet);
            } else {
                Console.Error.WriteLine(sourceFile + " is not supported color scheme format");
            }
        }

    }
}