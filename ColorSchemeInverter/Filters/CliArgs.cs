using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using ColorSchemeInverter.Colors;

namespace ColorSchemeInverter.Filters
{
    public sealed class CliArgs
    {
        private static CliArgs _instance;
        private static readonly object Padlock = new object();
        
        public List<CliArg> Items { get; set; }
  
        private CliArgs()
        {
            Items = new List<CliArg>();
        }
       
        private static CliArgs GetInstance()
        {
            lock (Padlock) {
                return _instance ?? (_instance = new CliArgs());
            }
        }

        public static CliArg GetItem(int index)
        {
            return GetInstance().Items[index];
        }

        public static void Register(string option, Func<RGB, object[], RGB> filter, byte minArguments)
        {
            GetInstance().Items.Add(new CliArg(option, filter, minArguments));
        }
        
        public static void Register(List<string> option, Func<RGB, object[], RGB> filter, byte minArguments)
        {
            GetInstance().Items.Add(new CliArg(option, filter, minArguments));
        }    

        public static void Register(string option, Func<HSL, object[], HSL> filter, byte minArguments)
        {
            GetInstance().Items.Add(new CliArg(option, filter, minArguments));
        }
        
        public static void Register(List<string> option, Func<HSL, object[], HSL> filter, byte minArguments)
        {
            GetInstance().Items.Add(new CliArg(option, filter, minArguments));
        }  
        
        /// <summary>
        /// Parses command line arguments, creates a FilterSet from them and returns it together with
        /// remaining arguments that should include source and target files
        /// Note: Run this with only paramter.
        /// </summary>
        /// <param name="args">command line arguments as provided in Main()</param>
        /// <param name="index">This is used only in recursion, leave out</param>
        /// <param name="filters">This is used only in recursion, leave out</param>
        /// <param name="remainingArgs">This is used only in recursion, leave out</param>
        /// <returns>FilterSet with delegate and parameters, Remaining arguments as List<string></returns>
        public static (FilterSet, List<string>) ParseArgs(string[] args, int index = 0, FilterSet filters = null, List<string> remainingArgs = null)
        { 
            filters = filters ?? new FilterSet();
            remainingArgs = remainingArgs ?? new List<string>();
            if (args.Length < index + 1)
                return (filters, remainingArgs);

            string arg = args[index++];
            
            (Delegate filter, string[] argStrings) = GetDelegateAndParameters(arg);

            if (filter is Func<HSL, object[], HSL>) {
                filters.Add((Func<HSL, object[], HSL>) filter, argStrings);
            } else if (filter is Func<RGB, object[], RGB>) {
                filters.Add((Func<RGB, object[], RGB>) filter, argStrings);
            } else {
                remainingArgs.Add(arg);
            }
             
            (filters,remainingArgs) = ParseArgs(args, index, filters, remainingArgs); // recurse
            return (filters, remainingArgs);
        }

        
        /// <summary>
        /// Gets matching filter delegate function and given arguments for given command line option
        /// Filter must be registered in CliArgs class.
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public static (Delegate, string[]) GetDelegateAndParameters(string option)
        {
            string argString;
            (option, argString) = Utils.SplitIntoCommandAndArguments(option);
            
            foreach (var cliArg in GetInstance().Items) {
                if (cliArg.Commands.Contains(option)) {
                    string[] argList = Utils.ExtractArgs(argString);
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