using System;
using System.Collections.Generic;
using System.Diagnostics;
using ColorSchemeInverter.Colors;

namespace ColorSchemeInverter.Filters
{
    public sealed class CliArgs
    {
        private static CliArgs _instance;
        private static readonly object Padlock = new object();
        
        public List<CliArg> Items { get; set; }
  
        private CliArgs()
        {
            Items = new List<CliArg>();
        }
       
        private static CliArgs GetInstance()
        {
            lock (Padlock) {
                return _instance ?? (_instance = new CliArgs());
            }
        }

        public static CliArg GetItem(int index)
        {
            return GetInstance().Items[index];
        }

        public static void Register(string option, Func<RGB, object[], RGB> filter, byte minArguments)
        {
            GetInstance().Items.Add(new CliArg(option, filter, minArguments));
        }
        
        public static void Register(List<string> option, Func<RGB, object[], RGB> filter, byte minArguments)
        {
            GetInstance().Items.Add(new CliArg(option, filter, minArguments));
        }    

        public static void Register(string option, Func<HSL, object[], HSL> filter, byte minArguments)
        {
            GetInstance().Items.Add(new CliArg(option, filter, minArguments));
        }
        
        public static void Register(List<string> option, Func<HSL, object[], HSL> filter, byte minArguments)
        {
            GetInstance().Items.Add(new CliArg(option, filter, minArguments));
        }  
        
    }
}