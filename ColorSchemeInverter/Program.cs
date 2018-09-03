namespace ColorSchemeInverter
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            string sourceFile = @"";
            string targetFile = @"";

            if (args.Length == 2) {
                sourceFile = args[0];
                targetFile = args[1];
            }
            
            
            ColorSchemeProcessor.InvertColors(sourceFile, targetFile);
            
        }
    }
}