### Color Scheme Inverter

##### ** WORK IN PROGRESS **

This is a tiny command line tool for adjusting colors in color schemes.
Works currently with JetBrains IDEA (.icls) and Visual Studio (.vstheme) color scheme files.

Added option to convert colors on png-files for quick testing.


### Currently available filters and corresponding CLI options
```
Available Filters:
      -g    --gain                         
      -l    --lightness                    
      -h    --hue                          
      -s    --saturation                   
      -c    --contrast                     
      -cl   --contrast-lightness           
      -cs   --contrast-saturation          
      -ga   --gamma                        
      -gar  --gamma-red                    
      -gag  --gamma-green                  
      -gab  --gamma-blue                   
      -gas  --gamma-saturation             
      -gal  --gamma-lightness              
      -le   --levels                       
      -ler  --levels-red                   
      -leg  --levels-green                 
      -leb  --levels-blue                  
      -lel  --levels-lightness             
      -les  --levels-saturation            
      -i    --invert-rgb                   
      -il   --invert-lightness  
   
Usage example:
    <appname> -il -gs=1.1 --contrast-saturationt=0.2,0.6 <sourcefile> <targetfile>
    
Using color range with filter:
    <appname> --gamma(sat:0.5-1,l:0-0.5)=1.5 <sourcefile> <targetfile>
```


#### ToDo

+ HSV<->HSL conversions
+ Range system for other filters than levels based
+ Parse range cli arguments (done but buggy)
+ Slope parameter for range. Decide CLI syntax for it.
+ Filter naming consistency
+ More filters?
+ Unit tests
+ Add declaration field to CliArg and write them (used in quick help)
+ Parsing of string argumnents (mostly to doubles) before-hand to optimize for speed
+ Support for CSS and HTML files? What else?
+ Presets for quick operations
+ Change the project name. Inversion is not any more the only way to tweak colors!!


#### Manually filtering (no using CLI arguments)

```c#
using System;
using System.IO;
using ColorSchemeInverter.Filters;
using ColorSchemeInverter.SchemeFileSupport;


namespace ColorSchemeInverter
{
    internal class Program
    {
        public static void Main(string[] args)
        {    
            [...]
    
            SchemeFormat schemeFormat = SchemeFormatUtil.GetFormatFromExtension(Path.GetExtension(sourceFileName));
            
            var filters = new FilterSet()
                .Add(FilterBundle.LightnessInvert)
                .Add(FilterBundle.SaturationContrast, 0.3, 0.45, new ColorRange().Lightness(0.3, 1).Blue(0,0.5))
                .Add(FilterBundle.SaturationGain, 1.2)
                .Add(FilterBundle.Gain, 1.1);
                .Add(FilterBundle.Contrast, 0.3);
            
            ColorSchemeProcessor p = new ColorSchemeProcessor(schemeFormat);
            p.ProcessFile(sourceFile, targetFile, filters);
            
        }
    }
}
```

#### Filtering by CLI arguments

```c#
using System;
using System.IO;
using ColorSchemeInverter.Filters;
using ColorSchemeInverter.SchemeFileSupport;
using ColorSchemeInverter.CLI;

namespace ColorSchemeInverter
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            // Make FilterBundle filters available for CLI
            FilterBundle.RegisterCliOptions();
            
            // Parse CLI args and generate FilterSet of them
            (FilterSet filters, string[] remainingArgs) = CliArgs.ParseFilterArgs(args);
            
            SchemeFormat schemeFormat 
                = SchemeFormatUtil.GetFormatFromExtension(Path.GetExtension(sourceFileName));
            
            if (remainingArgs.Length == 2) {
            
                sourceFile = args[0];
                targetFile = args[1];
                
                if (schemeFormat == SchemeFormat.Idea || schemeFormat == SchemeFormat.VisualStudio) {
                    ColorSchemeProcessor p = new ColorSchemeProcessor(schemeFormat);
                    p.ProcessFile(sourceFile, targetFile, filters);
                }
                
            }        
        }
    }
}
```