using System;
using System.Collections.Generic;
using System.Text;
using ColorSchemeManipulator.Colors;
using ColorSchemeManipulator.Common;

namespace ColorSchemeManipulator.CLI
{
    public class CliArg
    {
        public List<string> OptionArgs { get; set; }
        public Delegate FilterDelegate { get; set; }
        public byte MinParams { get; set; }
        public byte MaxParams { get; set; }
        public string Description { get; set; }
        public string ParamList { get; set; }
        public string ParamDesc { get; set; }

        public CliArg(string option, Func<IEnumerable<Color>, object[], IEnumerable<Color>> filterDelegate, byte minParams, byte maxParams = 0,
            string paramList = "", string desc = "", string paramDesc = "" )
        {
            OptionArgs = new List<string> {option};
            FilterDelegate = filterDelegate;
            MinParams = minParams;
            MaxParams = minParams < maxParams ? maxParams : minParams;
            Description = desc;
            ParamList = paramList;
            ParamDesc = paramDesc;
        }

        public CliArg(IEnumerable<string> options, Func<IEnumerable<Color>, object[], IEnumerable<Color>> filterDelegate, byte minParams, byte maxParams = 0,
            string paramList = "", string desc = "", string paramDesc = "")
        {
            OptionArgs = new List<string>(options);
            FilterDelegate = filterDelegate;
            MinParams = minParams;
            MaxParams = minParams < maxParams ? maxParams : minParams;
            Description = desc;
            ParamList = paramList;
            ParamDesc = paramDesc;
        }

        public new string ToString()
        {
            // var opts = new StringBuilder();
            // OptionArgs.ForEach(c => opts.Append(c + "  "));
            // string opt1 = "", opt2 = "";
            // if (OptionArgs.Count == 2) {
            //     opt1 = OptionArgs[0];
            //     opt2 = OptionArgs[1];
            // } else if (OptionArgs.Count == 1) {
            //     if (OptionArgs[0].StartsWith("--")) {
            //         opt2 = OptionArgs[0];
            //     } else {
            //         opt1 = OptionArgs[0];
            //     }
            // }

            string desc = Description == "" ? "@" + FilterDelegate.Method.Name : Description;
            
            List<string> optLines = new List<string>(4);
            for (var i = 0; i < OptionArgs.Count; i++) {
                var option = i == 0 ? OptionArgs[i] + ParamList : OptionArgs[i] + (ParamList != "" ? "=..." : "") ;
                List<string> argParts = Utils.ParamsWrap(option, 32);
                for (var index = 0; index < argParts.Count; index++) {
                    var argPart = argParts[index];
                    optLines.Add(index > 0 ? "    " + argPart : argPart);
                }
            }

            var sb = new StringBuilder();
            List<string> lines = Utils.WordWrap(desc, 65);

            int lineMaxCount = lines.Count.Max(optLines.Count);
            for (var index = 0; index < lineMaxCount; index++) {
                var line = index < lines.Count ? lines[index].Trim() : "";
                string opt = index < optLines.Count ? optLines[index] : "";
                sb.Append($"  {opt,-33} {line}");
                if (index < lineMaxCount-1)
                    sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}