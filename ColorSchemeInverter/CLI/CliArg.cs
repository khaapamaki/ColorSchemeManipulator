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
        public byte MinNumberOfParams { get; set; }
        public Delegate FilterDelegate { get; set; }

        // private CliArg() { }
        
        public CliArg(string option, Func<Hsl, object[], Hsl> filterDelegate, byte minParams)
        {
            OptionArgs = new List<string> {option};
            FilterDelegate = filterDelegate;
            MinNumberOfParams = minParams;
        }
        
        public CliArg(List<string> options, Func<Hsl, object[], Hsl> filterDelegate, byte minParams)
        {
            OptionArgs = new List<string>(options);
            FilterDelegate = filterDelegate;
            MinNumberOfParams = minParams;     
        }   
        
        public CliArg(string option, Func<Rgb, object[], Rgb> filterDelegate, byte minParams)
        {
            OptionArgs = new List<string> {option};
            FilterDelegate = filterDelegate;
            MinNumberOfParams = minParams;
        }
        
        public CliArg(IEnumerable<string> options, Func<Rgb, object[], Rgb> filterDelegate, byte minParams)
        {
            OptionArgs = new List<string>(options);
            FilterDelegate = filterDelegate;
            MinNumberOfParams = minParams;     
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

            return $"   {opt1,-5} {opt2,-30} ";
            return opts + "(" + MinNumberOfParams + ")";
            
            
            // with string format
            var columnHeaders1 = string.Format($"|{0,-30}|{1,-4}|{2,-15}|{3,-30}|{4,-30}|{5,-30}|{6,-30}", "ColumnA", "ColumnB", "ColumnC", "ColumnD", "ColumnE", "ColumnF", "ColumnG");

            // with string interpolation
            var columnHeaders2 = $"|{"ColumnA",-30}|{"ColumnB",-4}|{"ColumnC",-15}|{"ColumnD",-30}|{"ColumnE",-30}|{"ColumnF",-30}|{"ColumnG",-30}";
        }
    }
}