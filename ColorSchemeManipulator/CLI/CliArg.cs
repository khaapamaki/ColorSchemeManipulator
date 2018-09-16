using System;
using System.Collections.Generic;
using System.Text;
using ColorSchemeManipulator.Colors;

namespace ColorSchemeManipulator.CLI
{
    public class CliArg
    {
        public List<string> OptionArgs { get; set; }
        public Delegate FilterDelegate { get; set; }
        public byte MinParams { get; set; }
        public byte MaxParams { get; set; }
        public string Description { get; set; }

        public CliArg(string option, Func<IEnumerable<Color>, object[], IEnumerable<Color>> filterDelegate, byte minParams, byte maxParams = 0,
            string desc = "")
        {
            OptionArgs = new List<string> {option};
            FilterDelegate = filterDelegate;
            MinParams = minParams;
            MaxParams = minParams < maxParams ? maxParams : minParams;
            Description = desc;
        }

        public CliArg(List<string> options, Func<IEnumerable<Color>, object[], IEnumerable<Color>> filterDelegate, byte minParams, byte maxParams = 0,
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

            string desc = Description == "" ? "@" + FilterDelegate.Method.Name : Description;
            return $"{opt1,-5} {opt2,-26} {desc}";
        }
    }
}