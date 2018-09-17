using System;
using System.Collections.Generic;
using System.Linq;
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
            StringBuilder sb = new StringBuilder();
            List<string> lines = WordWrap(desc, 64);
            foreach (var line in lines) {
                //Console.WriteLine("/"+line.Trim()+"/");
            }
            for (var index = 0; index < lines.Count; index++) {
                var line = lines[index].Trim();
                sb.Append($"  {opt1,-5} {opt2,-30} {line}");
                opt1 = "";
                opt2 = "";
                if (index < lines.Count-1)
                    sb.AppendLine();
            }

            return sb.ToString();
        }

        private static List<string> WordWrap(string sentence, int columnWidth)
        {
            List<string> lines = new List<string>(3);
            string[] words = sentence.Split(' ');

            StringBuilder newSentence = new StringBuilder();

            string line = "";
            foreach (string word in words)
            {
                if ((line + word).Length > columnWidth)
                {
                    lines.Add(line);
                    line = "";
                }

                line += $"{word} ";
            }

            if (line.Length > 0)
                lines.Add(line);

            return lines;
        }
    }
}