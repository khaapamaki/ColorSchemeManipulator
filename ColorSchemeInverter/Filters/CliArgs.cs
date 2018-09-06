using System;
using System.Collections.Generic;
using System.Diagnostics;
using ColorSchemeInverter.Colors;

namespace ColorSchemeInverter.Filters
{
    public sealed class CliArgs
    {
        private static CliArgs _instance;
        private static readonly object padlock = new object();
        
        public List<CliArg> Items { get; set; }
  
        private CliArgs()
        {
            Items = new List<CliArg>();
        }
       
        private static CliArgs GetInstance()
        {
            lock (padlock) {
                if (_instance == null)
                    _instance = new CliArgs();

                return _instance;
            }
        }

        public static CliArg GetItem(int index)
        {
            return GetInstance().Items[index];
        }

        public static void Register(string cliCmd, Delegate filter, byte minArguments)
        {
            GetInstance().Items.Add(new CliArg(cliCmd, filter, minArguments));
        }
        
        public static void Register(List<string> cliCmds, Func<Color, object[], Color> filter, byte minArguments)
        {
            GetInstance().Items.Add(new CliArg(cliCmds, filter, minArguments));
        }    

        public static void Register(string cliCmd, Func<HSL, object[], HSL> filter, byte minArguments)
        {
            GetInstance().Items.Add(new CliArg(cliCmd, filter, minArguments));
        }
        
        public static void Register(List<string> cliCmds, Func<HSL, object[], HSL> filter, byte minArguments)
        {
            GetInstance().Items.Add(new CliArg(cliCmds, filter, minArguments));
        }  
        
    }
}