using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using ColorSchemeInverter.Colors;

namespace ColorSchemeInverter.CLI
{
    public class CliArg
    {
        public List<string> OptionArgs { get; set; } = new List<string>();
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
            return opts + "(" + MinNumberOfParams + ")";
        }
    }
}