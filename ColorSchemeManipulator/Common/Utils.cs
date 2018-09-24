using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ColorSchemeManipulator.CLI;

namespace ColorSchemeManipulator.Common
{
    public class Utils
    {
        public static void PrintHelp(int filterCount = -1, int expermFilterCount = -1)
        {
            Console.WriteLine("Available Filters:");
            if (filterCount == -1 || expermFilterCount == -1) {
                Console.WriteLine(CliArgs.ToString("\n\n"));
                return;
            }

            for (int i = 0; i < filterCount; i++) {
                Console.WriteLine(CliArgs.GetItem(i).ToString());
                Console.WriteLine();
            }

            if (expermFilterCount > 0) {
                Console.WriteLine("\nExperimental Filters:");
                for (int i = filterCount; i < filterCount + expermFilterCount; i++) {
                    Console.WriteLine(CliArgs.GetItem(i).ToString());
                    Console.WriteLine();
                }
            }

            const int col1 = -13;
            const int col2 = -14;
                
            string usage =
                "Usage:\n"
                + "  colschman [-filter] <sourcefile> [<targetfile>]\n"
                + "  colschman [-filter][=param1][,param2][,param3] <sourcefile> [<targetfile>]\n"
                + "  colschman [-filter1] [--filter2] <sourcefile> [<targetfile>]\n"
                + "  colschman [-filter][(rangeattr1:min-max,rangeattr2:min-max)[=param] <sourcefile> [<targetfile>]\n"
                + "  colschman [-filter][(rangeattr:min/slope-max/slope)[=param] <sourcefile> [<targetfile>]\n"
                + "  colschman [-filter][(rangeattr:minstart,minend,maxstart,maxend)[=param] <sourcefile> [<targetfile>]\n\n"
                + "Example:\n"
                + "  colschman -al=0.1,0.9 -s(hue:40/10-180/10)=1.2 my_scheme.icls fixed_scheme.icls\n\n"
                + "Range attributes:\n"
                + $"  {"h, hue",col1} {"Hue",col2}|  {"r, red",col1} {"Red",col2}\n"
                + $"  {"s, sat",col1} {"Saturation",col2}|  {"g, green",col1} {"Green",col2}\n"
                + $"  {"l, light",col1} {"Lightness",col2}|  {"b, blue",col1} {"Blue",col2}\n"
                + $"  {"v, value",col1} {"Value",col2}|  {"bri, bright",col1} {"Brightness",col2}\n";


            Console.Write(usage);
        }

        public static List<string> WordWrap(string sentence, int columnWidth)
        {
            List<string> lines = new List<string>(3);
            string[] words = sentence.Split(' ');

            string line = "";
            foreach (string word in words) {
                if ((line + word).Length > columnWidth) {
                    lines.Add(line);
                    line = "";
                }

                line += $"{word} ";
            }

            if (line.Length > 0)
                lines.Add(line);

            return lines;
        }
        

        public static List<string> ParamsWrap(string sentence, int columnWidth)
        {
            List<string> lines = new List<string>(4);
            // todo make this to retain separators
            string[] words = sentence.Split(' ','=',',');

            string line = "";
            foreach (string word in words) {
                if ((line + word).Length > columnWidth) {
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