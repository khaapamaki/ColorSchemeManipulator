### Color Scheme Inverter

##### ** WORK IN PROGRESS **

This is a tiny command line tool for adjusting colors in color schemes.
Works currently with JetBrains IDEA (.icls) and Visual Studio (.vstheme) color scheme files.

Added option to convert colors on png-files for quick testing.


### Currently available filters and corresponding CLI options
```
Available Filters:
  -h    --hue                      Hue shift as values of degrees -360..360
  -s    --saturation               Saturation multiplier
  -g    --gain                     @GainRgb
  -l    --lightness                @GainLightness
  -v    --value                    @GainValue
  -S    --hsv-saturation           @GainHsvSaturation
  -c    --contrast                 @ContrastRgb
  -cl   --contrast-lightness       @ContrastLightness
  -cv   --contrast-value           @ContrastValue
  -cs   --contrast-saturation      @ContrastHslSaturation
  -cS   --contrast-hsv-saturation  @ContrastHsvSaturation
  -ga   --gamma                    @GammaRgb
  -gar  --gamma-red                @GammaRed
  -gag  --gamma-green              @GammaGreen
  -gab  --gamma-blue               @GammaBlue
  -gal  --gamma-lightness          @GammaLightness
  -gav  --gamma-value              @GammaValue
  -gas  --gamma-saturation         @GammaHslSaturation
  -gaS  --gamma-hsv-saturation     @GammaHsvSaturation
  -le   --levels                   @LevelsRgb
  -ler  --levels-red               @LevelsRed
  -leg  --levels-green             @LevelsGreen
  -leb  --levels-blue              @LevelsBlue
  -lel  --levels-lightness         @LevelsLightness
  -lev  --levels-value             @LevelsValue
  -les  --levels-saturation        @LevelsHslSaturation
  -leS  --levels-hsv-saturation    @LevelsHsvSaturation
  -i    --invert-rgb               @InvertRgb
  -il   --invert-lightness         @InvertLightness
  -iv   --invert-value             @InvertValue

Usage example:
    <appname> -il -gs=1.1 --contrast=0.2,0.6 <sourcefile> <targetfile>
    
Using filter with color range:
    <appname> --gamma(sat:0.5-1,l:0-0.5)=1.5 <sourcefile> <targetfile>
    
Using filter with color range with slopes
    <appname> "--gamma(sat: 0.5/0.1 - 0.9/0.1, l: 0.1/0.1- 0.5/0.1) = 1.5" <sourcefile> <targetfile>
    where saturation range is 0.5-0.9 with slope of 0.1 on both ends and light range is..
```


#### ToDo

+ Optimization in range parameter handling during filtering
+ More Unit tests
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