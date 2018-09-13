### Color Scheme Inverter

##### ** WORK IN PROGRESS **

This is a tiny command line tool for adjusting colors in color schemes.
Works currently with JetBrains IDEA (.icls) and Visual Studio (.vstheme) color scheme files.

Added option to convert colors on png-files for quick testing.


### Currently available filters and corresponding CLI options
```
Available Filters:
  -h    --hue                      Hue shift. Accepts single parameter as degrees -360..360
  -s    --saturation               HSL saturation multiplier. Accepts single parameter 0..x
  -g    --gain                     RGB multiplier. Accepts single parameter 0..x
  -l    --lightness                HSL lightness multiplier. Accepts single parameter 0..x
  -v    --value                    HSV value multiplier. Accepts single parameter 0..x
  -S    --hsv-saturation           HSV saturation multiplier. Accepts single parameter 0..x
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
  -ib   --invert-brightness        Inverts perceived brightness - experimental
  -il   --invert-lightness         @InvertLightness
  -iv   --invert-value             @InvertValue
  -ilv  --invert-lightness-value   @InvertMixedLightnessAndValue
  -gsb  --grayscale-brightness     Converts to gray scale based on perceived brightness


Usage example:
    <appname> -il -gs=1.1 --contrast=0.2,0.6 <sourcefile> <targetfile>
    
Using filter with color range:
    <appname> "--gamma(sat: 0.5-1, l: 0-0.5) = 1.5" <sourcefile> <targetfile>
    
Using filter with color range defined with four points: (attribute: min1,min2,max1,max2)
    <appname> "--gamma(sat: 0.4, 0.5, 1, 1, lightness:0, 0, 0.5, 0.7) = 1.5" <sourcefile> <targetfile>
    
Using filter with color range with slope parameters: (attribute: min/slope - max/slope)
    <appname> "--gamma(sat: 0.5/0.1 - 0.9/0.1, l: 0.1/0.1- 0.5/0.1) = 1.5" <sourcefile> <targetfile>
    

Note: Some filters are purely for experimental purposes!
```

#### Issues

Hunting for them...


#### ToDo

+ Optimization in range parameter handling during filtering
+ Range parameter validation
+ CliArg subclassing(?) so other than filter delegate type arguments are possible
    + FilterSet arg type could be useful for creating presets
+ More Unit tests
+ Support for Visual Studio Code
+ Support for CSS and HTML files
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
                .Add(FilterBundle.GainLightness, 0.6, new ColorRange().Brightness(0.7, 1, 0.15, 0).Saturation(0.7, 1, 0.1, 0)) // dampen "neon" colors before inversion so don't get too dark
                .Add(FilterBundle.InvertPerceivedBrightness)  // invert image
                .Add(FilterBundle.LevelsLightness, 0.1, 0.9, 1, 0.1, 1) // adjust levels
                .Add(FilterBundle.GammaRgb, 1.7, new ColorRange().Hue(37, 56, 6, 20).Lightness(0.04, 0.6, 0, 0.2)) // yellow-neon green boost
                .Add(FilterBundle.GainHslSaturation, 1.7, new ColorRange().Hue(37, 56, 6, 20).Lightness(0.04, 0.6, 0, 0.2)) // yellow-neon green boost
                .Add(FilterBundle.GammaHslSaturation, 1.4, new ColorRange().Saturation4P(0.1, 0.1, 0.5, 0.7)) // add saturation for weak colors
                ;
            
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