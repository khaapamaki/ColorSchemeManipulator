using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Configuration;
using System.Reflection;
using System.Text;
using ColorSchemeInverter.Colors;

namespace ColorSchemeInverter.Filters
{
    public class CliArg
    {
        public List<string> Commands { get; set; } = new List<string>();
        public byte MinNumberOfParams { get; set; }
        public Delegate FilterDelegate { get; set; }

        private CliArg() { }
        
        public CliArg(string option, Func<HSL, object[], HSL> filterDelegate, byte minParams)
        {
            Commands = new List<string>() {option};
            FilterDelegate = filterDelegate;
            MinNumberOfParams = minParams;
        }
        
        public CliArg(List<string> options, Func<HSL, object[], HSL> filterDelegate, byte minParams)
        {
            Commands = new List<string>(options);
            FilterDelegate = filterDelegate;
            MinNumberOfParams = minParams;     
        }   
        
        public CliArg(string option, Func<RGB, object[], RGB> filterDelegate, byte minParams)
        {
            Commands = new List<string>() {option};
            FilterDelegate = filterDelegate;
            MinNumberOfParams = minParams;
        }
        
        public CliArg(List<string> options, Func<RGB, object[], RGB> filterDelegate, byte minParams)
        {
            Commands = new List<string>(options);
            FilterDelegate = filterDelegate;
            MinNumberOfParams = minParams;     
        }

        public string ToString()
        {
            StringBuilder cmds = new StringBuilder();
            Commands.ForEach(c => cmds.Append(c + " "));
            return cmds.ToString() + FilterDelegate.GetMethodInfo().Name + " " + MinNumberOfParams;
        }
    }
}