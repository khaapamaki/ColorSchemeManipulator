using System;
using System.Collections.Generic;
using System.Text;
using ColorSchemeManipulator.CLI;

namespace ColorSchemeManipulator.Common
{
    public class Utils
    {
        public static void PrintHelp(int filterCount = -1, int expermFilterCount = -1)
        {
            string usage =
                "\nUsage:\n"
                + "  colschman [-filter] <sourcefile> [<targetfile>]\n"
                + "  colschman [-filter][=param1][,param2][,param3] <sourcefile> [<targetfile>]\n"
                + "  colschman [-filter1] [--filter2] <sourcefile> [<targetfile>]\n"
                + "  colschman [-filter][(rangeattr1:min-max,rangeattr2:min-max)[=param] <sourcefile> [<targetfile>]\n"
                + "  colschman [-filter][(rangeattr:min/slope-max/slope)[=param] <sourcefile> [<targetfile>]\n"
                + "  colschman [-filter][(rangeattr:minstart,minend,maxstart,maxend)[=param] <sourcefile> [<targetfile>]\n\n";

            Console.WriteLine(usage);
            
            Console.WriteLine("Available Filters:\n");
            if (filterCount == -1 || expermFilterCount == -1) {
                Console.WriteLine(CliArgs.ToString("\n\n"));
                return;
            }

            for (int i = 0; i < filterCount; i++) {
                Console.WriteLine(CliArgs.GetItem(i).ToString());
                Console.WriteLine();
            }

            if (expermFilterCount > 0) {
                Console.WriteLine("Experimental Filters:\n");
                for (int i = filterCount; i < filterCount + expermFilterCount; i++) {
                    Console.WriteLine(CliArgs.GetItem(i).ToString());
                    Console.WriteLine();
                }
            }

            const int col1 = -13;
            const int col2 = -14;

             string help_tail = "Range attributes:\n"
                + $"  {"h, hue",col1} {"Hue",col2}|  {"r, red",col1} {"Red",col2}\n"
                + $"  {"s, sat",col1} {"Saturation",col2}|  {"g, green",col1} {"Green",col2}\n"
                + $"  {"l, light",col1} {"Lightness",col2}|  {"b, blue",col1} {"Blue",col2}\n"
                + $"  {"v, value",col1} {"Value",col2}|  {"bri, bright",col1} {"Brightness",col2}\n\n"
                + "Example:\n"
                + "  colschman -al=0.1,0.9 -s(hue:40/10-180/10)=1.2 my_scheme.icls fixed_scheme.icls\n\n";

            Console.Write(help_tail);
        }

        public static List<string> WordWrap(string str, int columnWidth)
        {
            List<string> lines = new List<string>(3);
            string[] words = str.Split(' ');

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


        public static List<string> ParamsWrap(string argumentStr, int columnWidth)
        {
            List<string> lines = new List<string>(4);
            string[] words = SplitArgument(argumentStr);

            string line = "";
            foreach (string word in words) {
                if ((line + word).Length > columnWidth) {
                    lines.Add(line);
                    line = "";
                }

                line += $"{word}";
            }

            if (line.Length > 0)
                lines.Add(line);

            return lines;
        }

        private static string[] SplitArgument(string argumentStr)
        {
            List<string> lines = new List<string>(12);
            var sb = new StringBuilder(20);
            char prev = ' ';
            foreach (var c in argumentStr) {
                if (c == ' ')
                    CutAfter(ref lines, ref sb, c);
                else if (c == '=') {
                    // sb.Append(' ');
                    CutBefore(ref lines, ref sb, c);
                    // sb.Append(' ');
                } else if (c == '[')
                    CutBefore(ref lines, ref sb, c);
                else if (c == ',' && prev != '[' || c == ']')
                    CutAfter(ref lines, ref sb, c);
                else
                    sb.Append(c);

                prev = c;
            }

            if (sb.ToString().Trim().Length > 0) {
                lines.Add(sb.ToString().Trim());
            }

            return lines.ToArray();
        }

        private static void CutAndRemove(ref List<string> lines, ref StringBuilder sb)
        {
            if (sb.Length > 0) {
                lines.Add(sb.ToString());
                sb.Clear();
            }
        }

        private static void CutAfter(ref List<string> lines, ref StringBuilder sb, char c)
        {
            sb.Append(c);
            lines.Add(sb.ToString());
            sb.Clear();
        }

        private static void CutBefore(ref List<string> lines, ref StringBuilder sb, char c)
        {
            if (sb.Length > 0) {
                lines.Add(sb.ToString());
                sb.Clear();
            }

            sb.Append(c);
        }
    }
}