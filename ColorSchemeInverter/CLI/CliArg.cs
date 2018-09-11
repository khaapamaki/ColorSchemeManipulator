using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using ColorSchemeInverter.Colors;

namespace ColorSchemeInverter.CLI
{
    public class CliArg
    {
        public List<string> OptionArgs { get; set; }
        public Delegate FilterDelegate { get; set; }
        public byte MinParams { get; set; }
        public byte MaxParams { get; set; }
        public string Description { get; set; }

        // private CliArg() { }

        public CliArg(string option, Func<Hsl, object[], Hsl> filterDelegate, byte minParams, byte maxParams = 0,
            string desc = "")
        {
            OptionArgs = new List<string> {option};
            FilterDelegate = filterDelegate;
            MinParams = minParams;
            MaxParams = minParams < maxParams ? maxParams : minParams;
            Description = desc;
        }

        public CliArg(List<string> options, Func<Hsl, object[], Hsl> filterDelegate, byte minParams, byte maxParams = 0,
            string desc = "")
        {
            OptionArgs = new List<string>(options);
            FilterDelegate = filterDelegate;
            MinParams = minParams;
            MaxParams = minParams < maxParams ? maxParams : minParams;
            Description = desc;
        }

        public CliArg(string option, Func<Rgb, object[], Rgb> filterDelegate, byte minParams, byte maxParams = 0,
            string desc = "")
        {
            OptionArgs = new List<string> {option};
            FilterDelegate = filterDelegate;
            MinParams = minParams;
            MaxParams = minParams < maxParams ? maxParams : minParams;
            Description = desc;
        }

        public CliArg(IEnumerable<string> options, Func<Rgb, object[], Rgb> filterDelegate, byte minParams,
            byte maxParams = 0, string desc = "")
        {
            OptionArgs = new List<string>(options);
            FilterDelegate = filterDelegate;
            MinParams = minParams;
            MaxParams = minParams < maxParams ? maxParams : minParams;
            Description = desc;
        }

        public CliArg(string option, Func<Hsv, object[], Hsv> filterDelegate, byte minParams, byte maxParams = 0,
            string desc = "")
        {
            OptionArgs = new List<string> {option};
            FilterDelegate = filterDelegate;
            MinParams = minParams;
            MaxParams = minParams < maxParams ? maxParams : minParams;
            Description = desc;
        }

        public CliArg(List<string> options, Func<Hsv, object[], Hsv> filterDelegate, byte minParams, byte maxParams = 0,
            string desc = "")
        {
            OptionArgs = new List<string>(options);
            FilterDelegate = filterDelegate;
            MinParams = minParams;
            MaxParams = minParams < maxParams ? maxParams : minParams;
            Description = desc;
        }


        public new string ToString()
        {
            // todo formatted output that can be used in cmd line help
            var opts = new StringBuilder();
            OptionArgs.ForEach(c => opts.Append(c + "  "));
            string opt1 = "", opt2 = "";
            if (OptionArgs.Count == 2) {
                opt1 = OptionArgs[0];
                opt2 = OptionArgs[1];
            } else if (OptionArgs.Count == 1) {
                if (OptionArgs[0].StartsWith("--")) {
                    opt2 = OptionArgs[0];
                } else {
                    opt1 = OptionArgs[0];
                }
            }

            string desc = Description == "" ? "++" + FilterDelegate.Method.Name : Description;
            return $"{opt1,-5} {opt2,-26} {(desc)}";
            return opts + "(" + MinParams + ")";


            // with string format
            var columnHeaders1 = string.Format($"|{0,-30}|{1,-4}|{2,-15}|{3,-30}|{4,-30}|{5,-30}|{6,-30}", "ColumnA",
                "ColumnB", "ColumnC", "ColumnD", "ColumnE", "ColumnF", "ColumnG");

            // with string interpolation
            var columnHeaders2 =
                $"|{"ColumnA",-30}|{"ColumnB",-4}|{"ColumnC",-15}|{"ColumnD",-30}|{"ColumnE",-30}|{"ColumnF",-30}|{"ColumnG",-30}";
        }
    }
}