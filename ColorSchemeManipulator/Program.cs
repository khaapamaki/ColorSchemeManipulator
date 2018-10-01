using System;
using System.Drawing;
using System.Drawing.Text;
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
            //    Register handlers
            //--------------------------------------------------------------------------    

            HandlerRegister<string> _schemeHandlerRegister = new HandlerRegister<string>();
            HandlerRegister<Bitmap> _imageHandlerRegister = new HandlerRegister<Bitmap>();
            _schemeHandlerRegister.Register(new IDEAFileHandler());
            _schemeHandlerRegister.Register(new VisualStudioFileHandler());
            _schemeHandlerRegister.Register(new VSCodeFileHandler());
            _imageHandlerRegister.Register(new ImageFileHandler());

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

            if (args.Length == 0) {
                Utils.PrintHelp(filterCount, experimFilterCount, verbose: false);
                return;
            } else if (args.Length == 1 && args[0].ToLower() == "--help") {
                Utils.PrintHelp(filterCount, experimFilterCount, verbose: true);
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

                string sourceFileName = @"HappyDays_Complete.icls";
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

            if (File.Exists(sourceFile)) {
                //--------------------------------------------------------------------------
                //    Get handler for the file type
                //--------------------------------------------------------------------------

                IColorFileHandler<string> schemeHandler = null;
                IColorFileHandler<Bitmap> imageHandler = null;
                try {
                    schemeHandler = _schemeHandlerRegister.GetHandlerForFile(sourceFile);
                    imageHandler = _imageHandlerRegister.GetHandlerForFile(sourceFile);
                } catch (Exception e) {
                    Console.WriteLine($"Multiple file processing units found for: {sourceFile}");
                    return;
                }

                if (schemeHandler == null && imageHandler == null) {
                    Console.WriteLine($"{sourceFile} is not supported color scheme format");
                    return;
                }

                if (schemeHandler != null && imageHandler != null) {
                    Console.WriteLine($"Multiple file processing units found for: {sourceFile}");
                    return;
                }

                //--------------------------------------------------------------------------
                //    Process file
                //--------------------------------------------------------------------------    

                if (imageHandler != null) {
                    var processor = new ColorFileProcessor<Bitmap>(imageHandler);
                    processor.ProcessFile(sourceFile, targetFile, filterSet);
                } else if (schemeHandler != null) {
                    var processor = new ColorFileProcessor<string>(schemeHandler);
                    processor.ProcessFile(sourceFile, targetFile, filterSet);
                } else {
                    Console.WriteLine($"{sourceFile} is not supported color scheme format");
                    return;
                }
            } else {
                Console.Error.WriteLine(sourceFile + " does not exist");
            }
        }
    }
}