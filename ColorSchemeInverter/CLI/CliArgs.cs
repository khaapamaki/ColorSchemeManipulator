using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColorSchemeInverter.Colors;
using ColorSchemeInverter.Filters;

namespace ColorSchemeInverter.CLI
{
    /// <summary>
    /// A singleton class to store and handle command line arguments relating to filters and filterDelegate paramters
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

        private List<CliArg> Items { get; set; } = new List<CliArg>();

        // ---- API Methods --------

        public static CliArg GetItem(int index)
        {
            return GetInstance().Items[index];
        }

        public static void Register(string option, Func<Rgb, object[], Rgb> filterDelegate, byte minArguments)
        {
            GetInstance().Items.Add(new CliArg(option, filterDelegate, minArguments));
        }

        public static void Register(List<string> option, Func<Rgb, object[], Rgb> filterDelegate, byte minArguments)
        {
            GetInstance().Items.Add(new CliArg(option, filterDelegate, minArguments));
        }

        public static void Register(string option, Func<Hsl, object[], Hsl> filterDelegate, byte minArguments)
        {
            GetInstance().Items.Add(new CliArg(option, filterDelegate, minArguments));
        }

        public static void Register(List<string> option, Func<Hsl, object[], Hsl> filterDelegate, byte minArguments)
        {
            GetInstance().Items.Add(new CliArg(option, filterDelegate, minArguments));
        }

        public static void Register(string option, Func<Hsv, object[], Hsv> filterDelegate, byte minArguments)
        {
            GetInstance().Items.Add(new CliArg(option, filterDelegate, minArguments));
        }

        public static void Register(List<string> option, Func<Hsv, object[], Hsv> filterDelegate, byte minArguments)
        {
            GetInstance().Items.Add(new CliArg(option, filterDelegate, minArguments));
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
        /// Gets matching filterDelegate delegate function and given arguments for given command line option
        /// Filter must be registered in CliArgs class.
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public static (Delegate, List<object>) GetDelegateAndParameters(string option)
        {
            string paramString;
            string rangeString;
            (option, paramString, rangeString) = CliUtils.SplitArgIntoPieces(option);
            ColorRange range = CliUtils.ParseRange(rangeString);
            foreach (var cliArg in GetInstance().Items) {
                if (cliArg.OptionArgs.Contains(option)) {
                    List<object> filterParams = CliUtils.ExtractParams(paramString);
                    if (filterParams.Count >= cliArg.MinNumberOfParams) {
                        if (range != null)
                            filterParams.Add(range);
                        return (cliArg.FilterDelegate, filterParams);
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
        public static string ToString(string delimiter = "\n", string prefix = "   ")
        {
            var sb = new StringBuilder();

            for (var i = 0; i < GetInstance().Items.Count; i++) {
                sb.Append(prefix + GetInstance().Items[i].ToString());
                if (i != GetInstance().Items.Count - 1)
                    sb.Append(delimiter);
            }

            return sb.ToString();
        }

    }
}