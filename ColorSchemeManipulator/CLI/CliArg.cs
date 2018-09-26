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

            string description = Description == "" ? "@" + FilterDelegate.Method.Name : Description;
            
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
                if (index < lineMaxCount-1)
                    sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}