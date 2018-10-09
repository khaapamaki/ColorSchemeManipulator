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
    /// <summary>
    /// 
    /// </summary>
    public class CliAppRunner
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public void Run(string[] args)
        {
            //--------------------------------------------------------------------------
            //    Register handlers
            //--------------------------------------------------------------------------

            RegisterHandlers();

            //--------------------------------------------------------------------------
            //    Parse command line arguments
            //--------------------------------------------------------------------------

            (var filters, string[] nonOptionArgs, string[] remainingOptArgs) = RegisterCliOptions(args);


            //--------------------------------------------------------------------------
            //    Print help / Other options
            //--------------------------------------------------------------------------

            // todo refactor, extract to method for handing other options

            //
            //  PARSE other than filter options here from nonFilterOptArgs
            //       

            Console.WriteLine("Color Scheme Manipulator "
                              + Assembly.GetExecutingAssembly().GetName().Version);

            if (args.Length == 0) {
                Utils.PrintHelp(_filterCount, _experimentalFilterCount, verbose: false);
                return;
            } else if (args.Length == 1 && args[0].ToLower() == "--help") {
                Utils.PrintHelp(_filterCount, _experimentalFilterCount, verbose: true);
                return;
            }

            // All remaining option arguments are considered illegal
            if (remainingOptArgs.Length > 0) {
                Console.WriteLine("Illegal argument: " + remainingOptArgs[0]);
                return;
            }

            //--------------------------------------------------------------------------
            //    Extract source and target files
            //--------------------------------------------------------------------------

            string sourceFile, targetFile;

            if (nonOptionArgs.Length == 1) {
                sourceFile = nonOptionArgs[0];
                targetFile = Path.GetFileNameWithoutExtension(sourceFile)
                             + "_converted" + Path.GetExtension(sourceFile);
            } else if (nonOptionArgs.Length == 2) {
                sourceFile = nonOptionArgs[0];
                targetFile = nonOptionArgs[1];
            } else {
                Console.WriteLine("No source file specified");
                return;
            }

            //--------------------------------------------------------------------------
            //    Process file
            //--------------------------------------------------------------------------

            ProcessFile(sourceFile, targetFile, filters);
        }

        private HandlerRegister<string> _schemeHandlerRegister = new HandlerRegister<string>();
        private HandlerRegister<Bitmap> _bitmapHandlerRegister = new HandlerRegister<Bitmap>();

        /// <summary>
        /// 
        /// </summary>
        public void RegisterHandlers()
        {
            // scheme file handlers (text based)
            _schemeHandlerRegister.Register(new IDEAFileHandler());
            _schemeHandlerRegister.Register(new VisualStudioFileHandler());
            _schemeHandlerRegister.Register(new VSCodeFileHandler());
            // bitmap handlers
            _bitmapHandlerRegister.Register(new ImageFileHandler());
        }

        private int _filterCount;
        private int _experimentalFilterCount;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public (FilterChain, string[], string[]) RegisterCliOptions(string[] args)
        {
            // Register Filters
            FilterBundle.RegisterCliOptions();
            _filterCount = CliArgs.GetItems().Count();

            // Register Experimental Filters
            ExperimentalBundle.RegisterCliOptions();
            _experimentalFilterCount = CliArgs.GetItems().Count() - _filterCount;

            // Parse CLI args and generate FilterSet of them
            (var filterChain, string[] nonOptionArgs) = CliArgs.ParseFilterArgs(args);

            // Extract non-option and remaining option arguments
            string[] nonFilterOptionArgs;
            (nonOptionArgs, nonFilterOptionArgs) = CliArgs.ExtractOptionArguments(nonOptionArgs);

            return (filterChain, nonOptionArgs, nonFilterOptionArgs);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="targetFile"></param>
        /// <param name="filterChain"></param>
        public void ProcessFile(string sourceFile, string targetFile, FilterChain filterChain)
        {
            if (File.Exists(sourceFile)) {
                //--------------------------------------------------------------------------
                //    Get handler for the file type
                //--------------------------------------------------------------------------

                IColorFileHandler<string> schemeHandler;
                IColorFileHandler<Bitmap> bitmapHandler;
                try {
                    schemeHandler = _schemeHandlerRegister.GetHandlerForFile(sourceFile);
                    bitmapHandler = _bitmapHandlerRegister.GetHandlerForFile(sourceFile);
                } catch (Exception e) {
                    Console.WriteLine($"Multiple file processing units found for: {sourceFile}");
                    return;
                }

                if (schemeHandler == null && bitmapHandler == null) {
                    Console.WriteLine($"{sourceFile} is not supported color scheme format");
                    return;
                }

                if (schemeHandler != null && bitmapHandler != null) {
                    Console.WriteLine($"Multiple file processing units found for: {sourceFile}");
                    return;
                }

                //--------------------------------------------------------------------------
                //    Process file
                //--------------------------------------------------------------------------    

                if (bitmapHandler != null) {
                    var processor = new ColorFileProcessor<Bitmap>(bitmapHandler);
                    processor.ProcessFile(sourceFile, targetFile, filterChain);
                } else if (schemeHandler != null) {
                    var processor = new ColorFileProcessor<string>(schemeHandler);
                    processor.ProcessFile(sourceFile, targetFile, filterChain);
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