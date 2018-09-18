using System;
using System.Collections.Generic;
using ColorSchemeManipulator.CLI;

namespace ColorSchemeManipulator.Common
{
    public class Utils
    {
        public static void PrintHelp(int filterCount = -1, int expermFilterCount = -1)
        {
            Console.WriteLine("Available Filters:");
            if (filterCount == -1 || expermFilterCount == -1) {
                Console.WriteLine(CliArgs.ToString());
                return;
            }
                
            for (int i = 0; i < filterCount; i++) {
                Console.WriteLine(CliArgs.GetItem(i).ToString());
            }

            if (expermFilterCount > 0) {
                Console.WriteLine("\nExperimental Filters:");
                for (int i = filterCount; i < filterCount + expermFilterCount; i++) {
                    Console.WriteLine(CliArgs.GetItem(i).ToString());
                }
            }
        }
        
        public static List<string> WordWrap(string sentence, int columnWidth)
        {
            List<string> lines = new List<string>(3);
            string[] words = sentence.Split(' ');

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