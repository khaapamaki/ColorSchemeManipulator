using System;
using System.Collections.Generic;
using System.Text;
using ColorSchemeManipulator.Colors;
using ColorSchemeManipulator.Filters;

namespace ColorSchemeManipulator.CLI
{
    /// <summary>
    /// A singleton class to store and handle command line arguments relating to filters and filterDelegate paramters
    /// </summary>
    public sealed class BatchCliArgs
    {
        private static BatchCliArgs _instance;
        private static readonly object Padlock = new object();

        private BatchCliArgs() { }

        private static BatchCliArgs GetInstance()
        {
            lock (Padlock) {
                return _instance ?? (_instance = new BatchCliArgs());
            }
        }

        private List<BatchCliArg> Items { get; set; } = new List<BatchCliArg>();

        // ---- API Methods --------

        public static BatchCliArg GetItem(int index)
        {
            return GetInstance().Items[index];
        }

        public static void Register(string option, Func<IEnumerable<ColorBase>, object[], IEnumerable<ColorBase>> filterDelegate, byte minParams,
            byte maxParams = 0, string desc = "")
        {
            GetInstance().Items.Add(new BatchCliArg(option, filterDelegate, minParams, maxParams, desc));
        }

        public static void Register(List<string> option, Func<IEnumerable<ColorBase>, object[], IEnumerable<ColorBase>> filterDelegate, byte minParams,
            byte maxParams = 0, string desc = "")
        {
            GetInstance().Items.Add(new BatchCliArg(option, filterDelegate, minParams, maxParams, desc));
        }
 
        /// <summary>
        /// Parses command line arguments, creates a BatchFilterSet from them and returns it together with
        /// remaining arguments that should include source and target files
        /// </summary>
        /// <param name="args"></param>
        /// <returns>BatchFilterSet with delegate and parameters, Remaining arguments</returns>
        public static (BatchFilterSet, string[]) ParseFilterArgs(params string[] args)
        {
            return BatchCliUtils.ParseFilterArgs(args);
        }

        /// <summary>
        /// Gets matching filterDelegate delegate function and given arguments for given command line option
        /// Filter must be registered in BatchCliArgs class.
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public static (Delegate, List<object>) GetDelegateAndParameters(string option)
        {
            string paramString;
            string rangeString;
            (option, paramString, rangeString) = BatchCliUtils.SplitArgIntoPieces(option);
            ColorRange range = BatchCliUtils.ParseRange(rangeString);
            foreach (var BatchCliArg in GetInstance().Items) {
                if (BatchCliArg.OptionArgs.Contains(option)) {
                    List<object> filterParams = BatchCliUtils.ExtractParams(paramString);
                    if (filterParams.Count >= BatchCliArg.MinParams) {
                        if (range != null)
                            filterParams.Add(range);
                        return (BatchCliArg.FilterDelegate, filterParams);
                    }
                }
            }

            return (null, null);
        }

        public static (string[], string[]) ExtractOptionArguments(string[] args)
        {
            List<string> optList = new List<string>();
            List<string> otherArgList = new List<string>();
            foreach (var arg in args) {
                if (arg.StartsWith("-")) {
                    optList.Add(arg);
                } else {
                    otherArgList.Add(arg);
                }
            }

            return (otherArgList.ToArray(), optList.ToArray());
        }

        public static string ToString(string delimiter = "\n", string prefix = "  ")
        {
            var sb = new StringBuilder();

            for (var i = 0; i < GetInstance().Items.Count; i++) {
                string line = GetInstance().Items[i].ToString();
                sb.Append(prefix + line);
                if (i != GetInstance().Items.Count - 1)
                    sb.Append(delimiter);
            }

            return sb.ToString();
        }
    }
}