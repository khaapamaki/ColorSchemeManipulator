using System;
using System.Collections.Generic;
using ColorSchemeInverter.Colors;
using ColorSchemeInverter.Filters;

namespace ColorSchemeInverter.CLI
{
    public static class CliUtils
    {
        /// <summary>
        /// Parses command line arguments, creates a FilterSet from them and returns it together with
        /// remaining arguments that should include source and target files
        /// </summary>
        /// <param name="args"></param>
        /// <returns>FilterSet with delegate and parameters, Remaining arguments</returns>
        public static (FilterSet, string[]) ParseArgs(string[] args)
        {
            (FilterSet cliFilters, List<string> remainingArgs) = CliUtils.RecursiveParseArgs(args);
            return (cliFilters, remainingArgs.ToArray());
        }
        
        
        private static (FilterSet, List<string>) RecursiveParseArgs(string[] args, int index = 0,
            FilterSet filters = null, List<string> remainingArgs = null)
        { 
            filters = filters ?? new FilterSet();
            remainingArgs = remainingArgs ?? new List<string>();
            if (args.Length < index + 1)
                return (filters, remainingArgs);

            string arg = args[index++];
            
            (Delegate filter, string[] argStrings) = CliArgs.GetDelegateAndParameters(arg);

            if (filter is Func<HSL, object[], HSL>) {
                filters.Add((Func<HSL, object[], HSL>) filter, argStrings);
            } else if (filter is Func<RGB, object[], RGB>) {
                filters.Add((Func<RGB, object[], RGB>) filter, argStrings);
            } else {
                remainingArgs.Add(arg);
            }
             
            (filters,remainingArgs) = RecursiveParseArgs(args, index, filters, remainingArgs); // recurse
            return (filters, remainingArgs);
        }

        
        public static (string, string) SplitIntoCommandAndArguments(string option)
        {
            string cmd = option;
            string argString = "";
            int splitPos = option.IndexOf('=');
            if (splitPos > 0) {
                cmd = option.Substring(0, splitPos);
                if (splitPos < option.Length) {
                    argString = option.Substring(splitPos + 1);
                }
            }

            return (cmd, argString);
        }

        public static string[] ExtractArgs(string argString)
        {
            var args = new List<string>();
            foreach (var s in argString.Trim('"').Split(',')) {
                args.Add(s.Trim());
            }

            return args.ToArray();
        }
    }
}