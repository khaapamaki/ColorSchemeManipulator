using System;
using System.Collections.Generic;
using System.Text;
using ColorSchemeManipulator.Colors;
using ColorSchemeManipulator.Common;
using ColorSchemeManipulator.Filters;

namespace ColorSchemeManipulator.CLI
{
    public class CliArg
    {
        public List<string> OptionArgs { get; set; }
        
        public FilterDelegate FilterDelegate { get; set; }
        //public Func<IEnumerable<Color>, ColorRange, double[], IEnumerable<Color>> SingleFilter { get; set; }
        public byte MinParams { get; set; }
        public byte MaxParams { get; set; }
        public string Description { get; set; }
        public string ParamList { get; set; }
        public string ParamDesc { get; set; }

        public CliArg(IEnumerable<string> options,
            FilterDelegate filterDelegate, 
            byte minParams,
            byte maxParams = 0,
            string paramList = "", 
            string desc = "", 
            string paramDesc = "")
        {
            OptionArgs = new List<string>(options);
            FilterDelegate = filterDelegate;
            MinParams = minParams;
            MaxParams = minParams < maxParams ? maxParams : minParams;
            Description = desc;
            ParamList = paramList;
            ParamDesc = paramDesc;
        }
        
        
        public CliArg(IEnumerable<string> options,
            Func<Color, ColorRange, double[], Color> singleFilter, 
            byte minParams,
            byte maxParams = 0,
            string paramList = "", 
            string desc = "", 
            string paramDesc = "")
        {
            OptionArgs = new List<string>(options);
            FilterDelegate = new FilterDelegate(singleFilter);
            MinParams = minParams;
            MaxParams = minParams < maxParams ? maxParams : minParams;
            Description = desc;
            ParamList = paramList;
            ParamDesc = paramDesc;
        }
        
        public CliArg(IEnumerable<string> options,
            Func<IEnumerable<Color>, ColorRange, double[], IEnumerable<Color>> multiFilter, 
            byte minParams,
            byte maxParams = 0,
            string paramList = "", 
            string desc = "", 
            string paramDesc = "")
        {
            OptionArgs = new List<string>(options);
            FilterDelegate = new FilterDelegate(multiFilter);
            MinParams = minParams;
            MaxParams = minParams < maxParams ? maxParams : minParams;
            Description = desc;
            ParamList = paramList;
            ParamDesc = paramDesc;
        }
        
        public string GetDescription(bool verbose)
        {
            return verbose ? GetVerboseDescription() : GetBriefDescription();
        }

        public string GetVerboseDescription()
        {
            string description = Description == "" ? "@" + FilterDelegate.FilterName() : Description;

            List<string> optLines = new List<string>(4);
            for (var i = 0; i < OptionArgs.Count; i++) {
                string option = OptionArgs[i] + ParamList;
                if (i != 0 && option.Length > 28 && ParamList != null) {
                    option = OptionArgs[i] + "=...";
                }

                List<string> argParts = Utils.ParamsWrap(option, 28);
                for (var index = 0; index < argParts.Count; index++) {
                    var argPart = argParts[index];
                    optLines.Add(index > 0 ? "    " + argPart : argPart);
                }
            }

            List<string> descLines = new List<string>();
            Utils.WordWrap(description, 70).ForEach(l => descLines.Add(l));
            Utils.WordWrap(ParamDesc, 70).ForEach(l => descLines.Add(l));

            var sb = new StringBuilder();
            int lineMaxCount = descLines.Count.Max(optLines.Count);
            for (var index = 0; index < lineMaxCount; index++) {
                var opt = index < optLines.Count ? optLines[index].TrimEnd() : "";
                var desc = index < descLines.Count ? descLines[index].Trim() : "";
                sb.Append($"  {opt,-31} {desc}");
                if (index < lineMaxCount - 1)
                    sb.AppendLine();
            }

            return sb.ToString();
        }

        public string GetBriefDescription()
        {
            string description = Description == "" ? "@" + FilterDelegate.FilterName() : Description;

            List<string> options = new List<string> {"", ""};

            foreach (var arg in OptionArgs) {
                if (arg.StartsWith("-") && arg[1] != '-') {
                    options[0] = arg + ",";
                    break;
                }
            }

            foreach (var arg in OptionArgs) {
                if (arg.StartsWith("--")) {
                    options[1] = arg;
                    break;
                }
            }

            List<string> descLines = new List<string>();
            Utils.WordWrap(description, 70).ForEach(l => descLines.Add(l));
            //Utils.WordWrap(ParamDesc, 70).ForEach(l => descLines.Add(l));

            var sb = new StringBuilder();

            for (var index = 0; index < descLines.Count; index++) {
                var desc = index < descLines.Count ? descLines[index].Trim() : "";
                if (index == 0)
                    sb.Append($"  {options[0],-5} {options[1],-26} {desc}");
                else
                    sb.Append($"  {"",-5} {"",-26} {desc}");
                if (index < descLines.Count - 1)
                    sb.AppendLine();
            }

            return sb.ToString();
        }
        
    }

}