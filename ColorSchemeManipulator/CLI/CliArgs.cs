using System;
using System.Collections.Generic;
using System.Text;
using ColorSchemeManipulator.Colors;
using ColorSchemeManipulator.Filters;

namespace ColorSchemeManipulator.CLI
{
    /// <summary>
    /// A singleton class to store and handle command line arguments relating to filters and filter paramters
    /// </summary>
    public sealed class CliArgs
    {
        private static CliArgs _instance;
        private static readonly object Padlock = new object();

        private CliArgs() { }

        private static CliArgs GetInstance()
        {
            lock (Padlock) {
                return _instance ?? (_instance = new CliArgs());
            }
        }

        private List<CliArg> Items { get; } = new List<CliArg>();

        // ---- API Methods --------

        public static CliArg GetItem(int index)
        {
            return GetInstance().Items[index];
        }

        public static List<CliArg> GetItems()
        {
            return GetInstance().Items;
        }

        public static void Register(
            string option,
            Func<IEnumerable<Color>, ColorRange, double[], IEnumerable<Color>> filter,
            byte minParams,
            byte maxParams = 0,
            string paramList = "",
            string desc = "",
            string paramDesc = "")
        {
            GetInstance().Items.Add(new CliArg(option, filter, minParams, maxParams, paramList, desc, paramDesc));
        }

        public static void Register(
            List<string> options,
            Func<IEnumerable<Color>, ColorRange, double[], IEnumerable<Color>> filter, 
            byte minParams,
            byte maxParams = 0, 
            string paramList = "", 
            string desc = "", 
            string paramDesc = "")
        {
            GetInstance().Items.Add(new CliArg(options, filter, minParams, maxParams, paramList, desc, paramDesc));
        }

        /// <summary>
        /// Parses command line arguments, creates a FilterSet from them and returns it together with
        /// remaining arguments that should include source and target files
        /// </summary>
        /// <param name="args"></param>
        /// <returns>FilterSet with delegate and parameters, Remaining arguments</returns>
        public static (FilterSet, string[]) ParseFilterArgs(params string[] args)
        {
            return CliUtils.ParseFilterArgs(args);
        }

        /// <summary>
        /// Gets matching filter delegate function and given arguments for given command line options
        /// Filter must be registered in CliArgs class.
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public static (Func<IEnumerable<Color>, ColorRange, double[], IEnumerable<Color>>, 
            ColorRange, double[]) GetDelegateAndData(string option)
        {
            string paramString;
            string rangeString;

            (option, paramString, rangeString) = CliUtils.SplitArgIntoPieces(option);

            var range = CliUtils.ParseRange(rangeString);

            foreach (var batchCliArg in GetInstance().Items) {
                if (batchCliArg.OptionArgs.Contains(option)) {
                    double[] filterParams = CliUtils.ExtractAndParseDoubleParams(paramString);
                    if (filterParams.Length >= batchCliArg.MinParams) {
                        return (batchCliArg.FilterDelegate, range, filterParams);
                    }
                }
            }

            return (null, null, null);
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

        public static string ToString(string delimiter = "\n", string prefix = "")
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