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

            (Delegate filterDelegate, List<object> paramList) = CliArgs.GetDelegateAndParameters(arg);

            object[] filterParams = paramList.ToArray();
            if (filterDelegate is Func<Hsl, object[], Hsl>) {
                filters.Add((Func<Hsl, object[], Hsl>) filterDelegate, filterParams);
            } else if (filterDelegate is Func<Rgb, object[], Rgb>) {
                filters.Add((Func<Rgb, object[], Rgb>) filterDelegate, filterParams);
            } else {
                remainingArgs.Add(arg);
            }

            (filters, remainingArgs) = RecursiveParseFilterArgs(args, index, filters, remainingArgs); // recurse
            return (filters, remainingArgs);
        }

        // todo test method CliUtils.SplitArgIntoPieces
        public static (string, string, string) SplitArgIntoPieces(string arg)
        {
            string option = null;
            string filterParams = null;
            string rangeString = null;
            const string pattern = @"(\-[a-zA-Z]|\-\-[a-zA-Z]{2,})(\((.*)\))?\s*=\s*(.*)";
            Match m = Regex.Match(arg, pattern);
            if (m.Groups.Count == 5) {
                option = m.Groups[1].ToString();
                filterParams = m.Groups[4].ToString();
                rangeString = m.Groups[3].ToString();
            }

            return (option, filterParams, rangeString);
        }

        public static List<object> ExtractArgs(string argString)
        {
            var args = new List<object>();
            foreach (var s in argString.Trim('"').Split(',')) {
                args.Add(s.Trim());
            }

            return args;
        }


        public static ColorRange ParseRange(string rangeString)
        {
            if (string.IsNullOrEmpty(rangeString)) return null;
            
            ColorRange range = new ColorRange();
            double max, min;
            bool succeeded;

            (succeeded, min, max) = GetRange(rangeString, "h|hue");
            if (succeeded) {
                range.Hue(min, max);
            }

            (succeeded, min, max) = GetRange(rangeString, "s|sat|saturation");
            if (succeeded) {
                range.Saturation(min, max);
            }

            (succeeded, min, max) = GetRange(rangeString, "l|light|lightness");
            if (succeeded) {
                range.Lightness(min, max);
            }

            (succeeded, min, max) = GetRange(rangeString, "r|red");
            if (succeeded) {
                range.Red(min, max);
            }

            (succeeded, min, max) = GetRange(rangeString, "g|green");
            if (succeeded) {
                range.Green(min, max);
            }

            (succeeded, min, max) = GetRange(rangeString, "b|blue");
            if (succeeded) {
                range.Blue(min, max);
            }

            (succeeded, min, max) = GetRange(rangeString, "v|value");
            if (succeeded) {
                range.Value(min, max);
            }

            return range;
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