using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ColorSchemeManipulator.Colors;
using ColorSchemeManipulator.Filters;
using ColorSchemeManipulator.Ranges;

namespace ColorSchemeManipulator.CLI
{
    public static class CliUtils
    {
        /// <summary>
        /// Parses command line arguments, creates a FilterSet from them and returns it together with
        /// remaining arguments that should include source and target files
        /// </summary>
        /// <param name="args"></param>
        /// <returns>FilterSet with delegate and parameters, Remaining arguments</returns>
        public static (FilterChain, string[]) ParseFilterArgs(string[] args)
        {
            (FilterChain filters, List<string> remainingArgs) = RecursiveParseFilterArgs(args);
            return (filters, remainingArgs.ToArray());
        }

        private static (FilterChain, List<string>) RecursiveParseFilterArgs(string[] args, int index = 0,
            FilterChain filters = null, List<string> remainingArgs = null)
        {
            filters = filters ?? new FilterChain();
            remainingArgs = remainingArgs ?? new List<string>();
            if (args.Length < index + 1)
                return (filters, remainingArgs);

            string arg = args[index++];

            (FilterDelegate filterDelegate, var colorRange, double[] filterParams) = CliArgs.GetDelegateAndData(arg);

            if (filterDelegate is FilterDelegate func) {
                filters.Add(func, colorRange, filterParams);
            } else {
                remainingArgs.Add(arg);
            }

            (filters, remainingArgs) = RecursiveParseFilterArgs(args, index, filters, remainingArgs); // recurse
            return (filters, remainingArgs);
        }

        public static (string, string, string) SplitArgIntoPieces(string arg)
        {
            string option = null;
            string filterParams = null;
            string rangeString = null;
            const string pattern = @"(^\-[a-zA-Z0-9]{1,3}|^\-\-[a-zA-Z0-9\-]{3,})(\((.*)\))?(\s*=\s*(.*))?";
            Match m = Regex.Match(arg, pattern);
            if (m.Groups.Count == 6) {
                option = m.Groups[1].ToString();
                filterParams = m.Groups[5].ToString();
                rangeString = m.Groups[3].ToString();
            }

            return (option == "" ? null : option,
                filterParams == "" ? null : filterParams,
                rangeString == "" ? null : rangeString);
        }

        public static double[] ExtractAndParseDoubleParams(string paramString)
        {
            var args = new List<double>();

            if (string.IsNullOrEmpty(paramString)) {
                return args.ToArray();
            }

            foreach (var s in paramString.Trim('"').Split(',')) {
                string str = s.Trim();
                if (double.TryParse(s.Trim(), out double val)) {
                    args.Add(val);
                } else 
                    args.Add(0);
            }

            return args.ToArray();
        }   
        
        public static ColorRange ParseRange(string rangeString)
        {
            if (string.IsNullOrEmpty(rangeString)) return null;

            var colorRange = new ColorRange();

            ParameterRange range = TryParseRangeForRangeParam(rangeString, "h|hue");
            if (range != null) {
                range.LoopMax = 360;
                colorRange.HueRange = range.Copy();
            }

            range = TryParseRangeForRangeParam(rangeString, "s|sat|saturation");
            if (range != null) {
                colorRange.SaturationRange = range.Copy();
            }

            range = TryParseRangeForRangeParam(rangeString, "l|lig|light|lightness");
            if (range != null) {
                colorRange.LightnessRange = range.Copy();
            }

            range = TryParseRangeForRangeParam(rangeString, "r|red");
            if (range != null) {
                colorRange.RedRange = range.Copy();
            }

            range = TryParseRangeForRangeParam(rangeString, "g|green");
            if (range != null) {
                colorRange.GreenRange = range.Copy();
            }

            range = TryParseRangeForRangeParam(rangeString, "b|blue");
            if (range != null) {
                colorRange.BlueRange = range.Copy();
            }

            range = TryParseRangeForRangeParam(rangeString, "v|value");
            if (range != null) {
                colorRange.ValueRange = range.Copy();
            }

            range = TryParseRangeForRangeParam(rangeString, "bri|bright|brightness");
            if (range != null) {
                colorRange.BrightnessRange = range.Copy();
            }

            return colorRange;
        }

        public static ParameterRange TryParseRangeForRangeParam(string rangeString,
            string rangeParam)
        {
            Match m = Regex.Match(rangeString, GetRangePattern(rangeParam));
            if (m.Success) {
                double min = double.Parse(m.Groups["min"].Value);
                double max = double.Parse(m.Groups["max"].Value);
                double.TryParse(m.Groups["minslope"].Value, out var minSlope);
                double.TryParse(m.Groups["maxslope"].Value, out var maxSlope);

                return ParameterRange.Range(min, max, minSlope, maxSlope);
            }

            m = Regex.Match(rangeString, GetFourPointRangePattern(rangeParam));
            if (m.Success) {
                double minStart = double.Parse(m.Groups["minS"].Value);
                double minEnd = double.Parse(m.Groups["minE"].Value);
                double maxStart = double.Parse(m.Groups["maxS"].Value);
                double maxEnd = double.Parse(m.Groups["maxE"].Value);

                return ParameterRange.FourPointRange(minStart, minEnd, maxStart, maxEnd);
            }

            return null;
        }

        private static string GetRangePattern(string options)
        {
            return
                @"(?i)(?<attr>"
                + options
                + @"):\s*(?<min>[\-]?[0-9]*[\.]?[0-9]+)(\/(?<minslope>[0-9]*[\.]?[0-9]+))?\s*\-\s*(?<max>[\-]?[0-9]*[\.]?[0-9]+)(\/(?<maxslope>[0-9]*[\.]?[0-9]+))?";
        }

        /// <summary>
        /// Pattern that matches eg. this: s:0,0.1,0.5,0.6 -or- S: 0, 0.1, 0.5, 0.6
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static string GetFourPointRangePattern(string options)
        {
            return
                @"(?i)(?<attr>"
                + options
                + @"):\s*(?<minS>[\-]?[0-9]*[\.]?[0-9]+),\s*(?<minE>[\-]?[0-9]*[\.]?[0-9]+),\s*(?<maxS>[\-]?[0-9]*[\.]?[0-9]+),\s*(?<maxE>[\-]?[0-9]*[\.]?[0-9]+)?";
        }
    }
}