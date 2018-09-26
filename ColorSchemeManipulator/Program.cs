using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using ColorSchemeManipulator.CLI;
using ColorSchemeManipulator.Common;
using ColorSchemeManipulator.Filters;
using ColorSchemeManipulator.SchemeFormats;
using ColorSchemeManipulator.SchemeFormats.Handlers;

namespace ColorSchemeManipulator
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            //--------------------------------------------------------------------------
            //    Command line parsing
            //--------------------------------------------------------------------------

            // Register Filters
            FilterBundle.RegisterCliOptions();
            int filterCount = CliArgs.GetItems().Count();

            // Register Experimental Filters
            ExperimentalBundle.RegisterCliOptions();
            int experimFilterCount = CliArgs.GetItems().Count() - filterCount;

            // Parse CLI args and generate FilterSet of them
            (var filterSet, string[] remainingArgs) = CliArgs.ParseFilterArgs(args);

            // Extract non-option and remaining option arguments
            string[] remainingOptArgs;
            (remainingArgs, remainingOptArgs) = CliArgs.ExtractOptionArguments(remainingArgs);

            //
            //  PARSE other than filter options here, and remove them from remainingOptArgs array 
            //       

            //--------------------------------------------------------------------------
            //    Print help
            //--------------------------------------------------------------------------

            Console.WriteLine("Color Scheme Manipulator "
                              + Assembly.GetExecutingAssembly().GetName().Version);
            if (args.Length == 0 || (args.Length == 1 && args[0].ToLower() == "--help")) {
                Utils.PrintHelp(filterCount, experimFilterCount);

                return;
            }

            // All remaining option arguments are considered illegal
            if (remainingOptArgs.Length > 0) {
                Console.WriteLine("Illegal argument: " + remainingOptArgs[0]);
                return;
            }

            //--------------------------------------------------------------------------
            //    Get source and target paths
            //--------------------------------------------------------------------------

            string sourceFile, targetFile;
            // get source and target from CLI args

            if (remainingArgs.Length == 1) {
                sourceFile = remainingArgs[0];
                targetFile = Path.GetFileNameWithoutExtension(sourceFile)
                             + "_converted" + Path.GetExtension(sourceFile);
            } else if (remainingArgs.Length == 2) {
                sourceFile = remainingArgs[0];
                targetFile = remainingArgs[1];
            } else {
#if DEBUG
                //--------------------------------------------------------------------------
                //    Debug version auto file choosing, remove when merging to stage/prod
                //-------------------------------------------------------------------------- 
                
                string sourceFileName = @"HapdpyDays_Complete.icls";
                // sourceFileName = "darcula.vstheme";
                // sourceFileName = "HappyDays.png";
                // sourceFileName = "photo.png";
                string baseDir = System.AppDomain.CurrentDomain.BaseDirectory;
                sourceFile = Path.GetFullPath(Path.Combine(baseDir, sourceFileName));
                targetFile = Path.GetFileNameWithoutExtension(sourceFile)
                             + "_converted" + Path.GetExtension(sourceFile);
#else
                Console.WriteLine("No source file specified");
                return;
#endif
            }
            
            //--------------------------------------------------------------------------
            //    Get scheme format by file extension
            //--------------------------------------------------------------------------

            SchemeFormat schemeFormat = SchemeUtils.GetFormatFromExtension(Path.GetExtension(sourceFile));
            if (schemeFormat == SchemeFormat.Unknown) {
                Console.Error.WriteLine(sourceFile + " is not supported color scheme format");
                return;
            }

            //--------------------------------------------------------------------------
            //    Process file
            //--------------------------------------------------------------------------    

            if (File.Exists(sourceFile)) {
                if (schemeFormat == SchemeFormat.Image) {
                    var processor = new ColorFileProcessor<Bitmap>(new ImageFileHandler());

                    Console.WriteLine("Applying filters:");
                    Console.WriteLine(filterSet.ToString());

                    processor.ProcessFile(sourceFile, targetFile, filterSet);
                    Console.WriteLine("Done.");
                } else {
                    var handler = SchemeUtils.GetSchemeHandlerByFormat(schemeFormat);
                    if (handler != null) {
                        var processor = new ColorFileProcessor<string>(handler);

                        Console.WriteLine("Applying filters:");
                        Console.WriteLine(filterSet.ToString());

                        processor.ProcessFile(sourceFile, targetFile, filterSet);
                        Console.WriteLine("Done.");
                    } else {
                        Console.Error.WriteLine(sourceFile + " is not supported color scheme format");
                    }
                }
            } else {
                Console.Error.WriteLine(sourceFile + " does not exist");
            }
        }
    }
}