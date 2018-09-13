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

            object[] filterParams = TryParseDoubles(paramList?.ToArray());
            //object[] filterParams =  paramList?.ToArray();

            if (filterDelegate is Func<Hsl, object[], Hsl>) {
                filters.Add((Func<Hsl, object[], Hsl>) filterDelegate, filterParams);
            } else if (filterDelegate is Func<Rgb, object[], Rgb>) {
                filters.Add((Func<Rgb, object[], Rgb>) filterDelegate, filterParams);
            } else if (filterDelegate is Func<Hsv, object[], Hsv>) {
                filters.Add((Func<Hsv, object[], Hsv>) filterDelegate, filterParams);
            } else {
                remainingArgs.Add(arg);
            }

            (filters, remainingArgs) = RecursiveParseFilterArgs(args, index, filters, remainingArgs); // recurse
            return (filters, remainingArgs);
        }

        public static object[] TryParseDoubles(object[] filterParams)
        {
            if (filterParams == null)
                return null;
            List<object> newList = new List<object>();
            foreach (var param in filterParams) {
                object newParam = param;
                if (param is string) {
                    double? d = FilterUtils.TryParseDouble(param);
                    if (d != null) {
                        newParam = d;
                    }
                }

                newList.Add(newParam);
            }

            return newList.ToArray();
        }

        public static (string, string, string) SplitArgIntoPieces(string arg)
        {
            string option = null;
            string filterParams = null;
            string rangeString = null;
            const string pattern = @"(^\-[a-zA-Z]{1,3}|^\-\-[a-zA-Z\-]{3,})(\((.*)\))?(\s*=\s*(.*))?";
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

        public static List<object> ExtractParams(string paramString)
        {
            var args = new List<object>();

            if (string.IsNullOrEmpty(paramString)) {
                return args;
            }

            foreach (var s in paramString.Trim('"').Split(',')) {
                args.Add(s.Trim());
            }

            return args;
        }

        public static ColorRange ParseRange(string rangeString)
        {
            if (string.IsNullOrEmpty(rangeString)) return null;

            ColorRange colorRange = new ColorRange();
            double max, min, minSlope, maxSlope;
            bool succeeded;

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
            range = TryParseRangeForRangeParam(rangeString, "b|bri|brightness");
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

                return  ParameterRange.FourPointRange(minStart, minEnd, maxStart, maxEnd);
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