using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ColorSchemeInverter.Colors;

namespace ColorSchemeInverter.Filters
{
    public class CliArg
    {
        public List<string> Commands { get; set; } = new List<string>();
        public byte MinNumberOfArguments { get; set; }
        public Delegate Filter { get; set; }

        private CliArg() { }
        
        public CliArg(string cliCmd, Delegate filter, byte minArguments)
        {
            Commands = new List<string>() {cliCmd};
            Filter = filter;
            MinNumberOfArguments = minArguments;
        }
        
        public CliArg(List<string> cliCmds, Func<Color, object[], Color> filter, byte minArguments)
        {
            Commands = new List<string>(cliCmds);
            Filter = filter;
            MinNumberOfArguments = minArguments;     
        }    
        
        
        public CliArg(string cliCmd, Func<HSL, object[], HSL> filter, byte minArguments)
        {
            Commands = new List<string>() {cliCmd};
            Filter = filter;
            MinNumberOfArguments = minArguments;
        }
        
        public CliArg(List<string> cliCmds, Func<HSL, object[], HSL> filter, byte minArguments)
        {
            Commands = new List<string>(cliCmds);
            Filter = filter;
            MinNumberOfArguments = minArguments;     
        }   
        
        public CliArg(string cliCmd, Func<RGB, object[], RGB> filter, byte minArguments)
        {
            Commands = new List<string>() {cliCmd};
            Filter = filter;
            MinNumberOfArguments = minArguments;
        }
        
        public CliArg(List<string> cliCmds, Func<RGB, object[], RGB> filter, byte minArguments)
        {
            Commands = new List<string>(cliCmds);
            Filter = filter;
            MinNumberOfArguments = minArguments;     
        }   

    }
}