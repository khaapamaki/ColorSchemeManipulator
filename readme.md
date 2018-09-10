### Color Scheme Inverter

##### ** WORK IN PROGRESS **

This is a tiny command line tool for adjusting colors in color schemes.
Works currently with JetBrains IDEA (.icls) and Visual Studio (.vstheme) color scheme files.

Added option to convert colors on png-files for quick testing.


### Currently available filters and corresponding CLI options
```
Available Filters:
      -g    --gain                         GainRgb
      -b    --brightness                   GainBrightness
      -l    --lightness                    GainLightness
      -h    --hue                          ShiftHsvHue
      -s    --saturation                   GainHsvSaturation
      -S    --hsl-saturation               GainHslSaturation
      -c    --contrast                     ContrastRgb
      -cb   --contrast-brighness           ContrastBrightness
      -cl   --contrast-lightness           ContrastLightness
      -cs   --contrast-saturation          ContrastHsvSaturation
      -cS   --contrast-hsl-saturation      ContrastHslSaturation
      -ga   --gamma                        GammaRgb
      -gar  --gamma-red                    GammaRed
      -gag  --gamma-green                  GammaGreen
      -gab  --gamma-blue                   GammaBlue
      -gab  --gamma-brightness             GammaBrightness
      -gal  --gamma-lightness              GammaLightness
      -gas  --gamma-saturation             GammaHsvSaturation
      -gaS  --gamma-hsl-saturation         GammaHslSaturation
      -le   --levels                       LevelsRgb
      -ler  --levels-red                   LevelsRed
      -leg  --levels-green                 LevelsGreen
      -le   --levels                       LevelsRgb
      -ler  --levels-red                   LevelsRed
      -leg  --levels-green                 LevelsGreen
      -leb  --levels-blue                  LevelsBlue
      -lel  --levels-lightness             LevelsLightness
      -les  --levels-saturation            LevelsHsvSaturation
      -leS  --levels-hsl-saturation        LevelsHslSaturation
      -i    --invert-rgb                   InvertRgb
      -ib   --invert-brightness            InvertBrightness
      -il   --invert-lightness             InvertLightness

Usage example:
    <appname> -il -gs=1.1 --contrast=0.2,0.6 <sourcefile> <targetfile>
    
Using color range with filter:
    <appname> --gamma(sat:0.5-1,l:0-0.5)=1.5 <sourcefile> <targetfile>
```


#### ToDo

+ Slope parameter for range. Decide CLI syntax for it + implement math
+ Interpolation for looping values (= hue)
+ Unit tests
+ Add declaration field to CliArg and write them (used in quick help)
+ Parsing of string argumnents (mostly to doubles) before-hand to optimize for speed
+ Support for Visual Studio Code
+ Support for CSS and HTML files? What else?
+ Presets for quick operations
+ Proper HSV<->HSL conversions, now done by converting to RGB first
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
            
            // Parse CLI args and generate FilterSet from them
            (FilterSet filters, string[] remainingArgs) = CliArgs.ParseFilterArgs(args);
            
            // Extract non-option and remaining option arguments
            string[] remainingOptArgs;            
            (remainingArgs, remainingOptArgs) = CliArgs.ExtractOptionArguments(remainingArgs);

            // PARSE other than filter options here, and remove them from remainingOptArgs array
            
            // All remaining option arguments are considered illegal... 
            
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