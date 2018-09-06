using System;
using System.Collections.Generic;
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

        public static void Register(string option, Func<RGB, object[], RGB> filterDelegate, byte minArguments)
        {
            GetInstance().Items.Add(new CliArg(option, filterDelegate, minArguments));
        }
        
        public static void Register(List<string> option, Func<RGB, object[], RGB> filterDelegate, byte minArguments)
        {
            GetInstance().Items.Add(new CliArg(option, filterDelegate, minArguments));
        }    

        public static void Register(string option, Func<HSL, object[], HSL> filterDelegate, byte minArguments)
        {
            GetInstance().Items.Add(new CliArg(option, filterDelegate, minArguments));
        }
        
        public static void Register(List<string> option, Func<HSL, object[], HSL> filterDelegate, byte minArguments)
        {
            GetInstance().Items.Add(new CliArg(option, filterDelegate, minArguments));
        }

        /// <summary>
        /// Parses command line arguments, creates a FilterSet from them and returns it together with
        /// remaining arguments that should include source and target files
        /// </summary>
        /// <param name="args"></param>
        /// <returns>FilterSet with delegate and parameters, Remaining arguments</returns>
        public static (FilterSet, string[]) ParseFilterArgs(string[] args)
        {
            return CliUtils.ParseFilterArgs(args);
        }
       
        /// <summary>
        /// Gets matching filterDelegate delegate function and given arguments for given command line optionArg
        /// Filter must be registered in CliArgs class.
        /// </summary>
        /// <param name="optionArg"></param>
        /// <returns></returns>
        public static (Delegate, string[]) GetDelegateAndParameters(string optionArg)
        {
            string argString;
            (optionArg, argString) = CliUtils.SplitIntoCommandAndArguments(optionArg);
            
            foreach (var cliArg in GetInstance().Items) {
                if (cliArg.OptionArgs.Contains(optionArg)) {
                    string[] argList = CliUtils.ExtractArgs(argString);
                    if (argList.Length >= cliArg.MinNumberOfParams)
                        return (cliArg.FilterDelegate, argList);
                }
            }

            return (null, null);
        }

            
        public static string ToString(string delimiter = "\n", string prefix = "   ")
        {
            StringBuilder sb = new StringBuilder();
            
            for (var i = 0; i < GetInstance().Items.Count; i++) {
                sb.Append(prefix + GetInstance().Items[i].ToString());
                if (i != GetInstance().Items.Count - 1)
                    sb.Append(delimiter);
            }
            return sb.ToString();
        }
        

    }
}