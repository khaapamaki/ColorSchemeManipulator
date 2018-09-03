using System;
using System.Xml.Schema;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;
using System.Xml.Schema;

namespace ColorSchemeInverter
{
    public class ColorSchemeProcessor
    {
        public static void InvertColors(string sourceFile, string targetFile)
        {
            DigIntoXML(sourceFile);
        }

        private static bool DigIntoXML(string sourceFile)
        {
            bool succeeded = false;
            try {
                var xData = XDocument.Load(sourceFile);
                var xRoot = xData.Root;
                succeeded = true;
            } catch (Exception e) {
                Console.Error.WriteLine(e.ToString());
            }

            return succeeded;
        }
    }
}