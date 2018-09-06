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
        
        public CliArg(string option, Func<HSL, object[], HSL> filter, byte minArguments)
        {
            Commands = new List<string>() {option};
            Filter = filter;
            MinNumberOfArguments = minArguments;
        }
        
        public CliArg(List<string> options, Func<HSL, object[], HSL> filter, byte minArguments)
        {
            Commands = new List<string>(options);
            Filter = filter;
            MinNumberOfArguments = minArguments;     
        }   
        
        public CliArg(string option, Func<RGB, object[], RGB> filter, byte minArguments)
        {
            Commands = new List<string>() {option};
            Filter = filter;
            MinNumberOfArguments = minArguments;
        }
        
        public CliArg(List<string> options, Func<RGB, object[], RGB> filter, byte minArguments)
        {
            Commands = new List<string>(options);
            Filter = filter;
            MinNumberOfArguments = minArguments;     
        }   

    }
}