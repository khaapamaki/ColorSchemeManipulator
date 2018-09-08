using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
        public static (FilterSet, string[]) ParseFilterArgs(string[] args)
        {
            (FilterSet cliFilters, List<string> remainingArgs) = RecursiveParseFilterArgs(args);
            return (cliFilters, remainingArgs.ToArray());
        }


        private static (FilterSet, List<string>) RecursiveParseFilterArgs(string[] args, int index = 0,
            FilterSet filters = null, List<string> remainingArgs = null)
        {
            filters = filters ?? new FilterSet();
            remainingArgs = remainingArgs ?? new List<string>();
            if (args.Length < index + 1)
                return (filters, remainingArgs);

            string arg = args[index++];

            (Delegate filterDelegate, string[] argStrings) = CliArgs.GetDelegateAndParameters(arg);

            if (filterDelegate is Func<HSL, object[], HSL>) {
                filters.Add((Func<HSL, object[], HSL>) filterDelegate, argStrings);
            } else if (filterDelegate is Func<RGB, object[], RGB>) {
                filters.Add((Func<RGB, object[], RGB>) filterDelegate, argStrings);
            } else {
                remainingArgs.Add(arg);
            }

            (filters, remainingArgs) = RecursiveParseFilterArgs(args, index, filters, remainingArgs); // recurse
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

        public static ColorRange ParseRange(string rangeString)
        {

            ColorRange range = new ColorRange();
            double max, min;
            bool succeeded;
            
            (succeeded, min, max) = GetRange(rangeString, "h|hue");
            if (succeeded)
            {
                range.SetHueRange(min, max);
              
            }
            
            (succeeded, min, max) = GetRange(rangeString, "s|sat|saturation");
            if (succeeded)
            {
                range.SetSaturationRange(min, max);
            }

            (succeeded, min, max) = GetRange(rangeString, "l|light|lightness");
            if (succeeded)
            {
                range.SetLightnessRange(min, max);
            }
            
            (succeeded, min, max) = GetRange(rangeString, "r|red");
            if (succeeded)
            {
                range.SetRedRange(min, max);
            }
            
            (succeeded, min, max) = GetRange(rangeString, "g|green");
            if (succeeded)
            {
                range.SetGreenRange(min, max);
            }
            
            (succeeded, min, max) = GetRange(rangeString, "b|blue");
            if (succeeded)
            {
                range.SetBlueRange(min, max);
            }
            
            (succeeded, min, max) = GetRange(rangeString, "v|value");
            if (succeeded)
            {
                range.SetValueRange(min, max);
            }
            
            throw new NotImplementedException();
        }

        private static (bool, double, double) GetRange(string rangeString, string options)
        {
            Match m = Regex.Match(rangeString, GetRangePattern(options));
            if (m.Length == 1 && m.Groups.Count == 4) {
                double min = double.Parse(m.Groups[2].Value);
                double max = double.Parse(m.Groups[3].Value);
                return (true, min, max);
            }

            return (false, 0, 0);
        }
        
        private static string GetRangePattern(string options)
        {
            return @"(?i)" + options
                + @":\s*([\-]?[0-9]*[\.]?[0-9]+)\s*\-\s*([\-]?[0-9]*[\.]?[0-9]+)";
        }
    }
}