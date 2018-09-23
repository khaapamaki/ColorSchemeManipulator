##### *** You are in development branch ***

## Color Scheme Manipulator

This is a tiny command line tool for adjusting colors in color schemes.
Works currently with JetBrains IDEA (.icls) and Visual Studio (.vstheme) color scheme files.

Also has option to filter png-files for quick testing.

#### Currently available filters and corresponding CLI options
```
Available Filters:
  -h    --hue                      Hue shift. Takes single parameter as degrees (-360..360)

  -s    --saturation               HSL saturation multiplier. Takes single parameter (0..x)

  -g    --gain                     RGB multiplier. Takes single parameter (0..x)

  -l    --lightness                HSL lightness multiplier. Takes single parameter (0..x)

  -v    --value                    HSV value multiplier. Takes single parameter (0..x)

  -S    --hsv-saturation           HSV saturation multiplier. Takes single parameter (0..x)

  -c    --contrast                 Adjusts contrast. Takes one mandatory and one optional parameter,
                                   curve strength (-1..1), inflection point (0..1 default 0.5)

  -cs   --contrast-saturation      Adjusts contrast of saturation. Takes one mandatory and one optional
                                   parameter, curve strength (-1..1), inflection point (0..1 default
                                   0.5)

  -ga   --gamma                    Adjusts gamma of all RGB channels equally. Takes single parameter
                                   (0.01..9.99)

  -gar  --gamma-red                Adjusts gamma of red channel. Takes single parameter (0.01..9.99)

  -gag  --gamma-green              Adjusts gamma of green channel. Takes single parameter (0.01..9.99)

  -gab  --gamma-blue               Adjusts gamma of blue channel. Takes single parameter (0.01..9.99)

  -gal  --gamma-lightness          Adjusts gamma of HSL lightness. Takes single parameter (0.01..9.99)

  -gav  --gamma-value              Adjusts gamma of HSV value. Takes single parameter (0.01..9.99)

  -gas  --gamma-saturation         Adjusts gamma of saturation. Takes single parameter (0.01..9.99)

  -le   --levels                   Adjusts levels of all RGB channels. Takes five parameters: input
                                   black (0..1), input white (0..1), gamma (0.01..9.99), output black
                                   (0..1), output white (0..1)

  -ler  --levels-red               Adjusts levels of red channel. Takes five parameters: input black
                                   (0..1), input white (0..1), gamma (0.01..9.99), output black (0..1),
                                   output white (0..1)

  -leg  --levels-green             Adjusts levels of red channel. Takes five parameters: input black
                                   (0..1), input white (0..1), gamma (0.01..9.99), output black (0..1),
                                   output white (0..1)

  -leb  --levels-blue              Adjusts levels of red channel. Takes five parameters: input black
                                   (0..1), input white (0..1), gamma (0.01..9.99), output black (0..1),
                                   output white (0..1)

  -al   --auto-levels              Adjusts levels of RGB channels by normalizing levels so that darkest
                                   color will be black and lightest color max bright. Takes three
                                   parameters: output black (0..1), output white (0..1), gamma
                                   (0.01..9.99)

  -les  --levels-saturation        @LevelsHslSaturation

  -i    --invert-rgb               Inverts RGB channels. Takes no parameter.

  -il   --invert-lightness         Inverts HSL lightness. Takes no parameter.

  -iv   --invert-value             Inverts HSV value. Takes no parameter.

  -ib   --invert-brightness        Inverts perceived brightness. Takes no parameter.

  -gsb  --grayscale-brightness     Converts to gray scale based on perceived brightness. Takes no
                                   parameter.

        --clamp                    Clamps color values to normal range of 0..1. Tries to preserve hue.
                                   Takes no parameter. This is automatically done as last filter.


Experimental Filters:
  -lel  --levels-lightness         Adjusts levels of HSL lightness. Takes five parameters: input black
                                   (0..1), input white (0..1), gamma (0.01..9.99), output black (0..1),
                                   output white (0..1)

  -lev  --levels-value             Adjusts levels of HSV value. Takes five parameters: input black
                                   (0..1), input white (0..1), gamma (0.01..9.99), output black (0..1),
                                   output white (0..1)

  -ibc  --invert-brightness-corr   Inverts perceived brightness with correction parameter (0..1)

  -ilv  --invert-lightness-value   Inverts colors using both lightness and value, by mixing the result
                                   by parameter (0..1)

  -b2l  --brightness-to-lightness  @BrightnessToLightness

  -b2v  --brightness-to-value      @BrightnessToValue

  -cl   --contrast-lightness       Adjusts contrast of HSL lightness. Takes one mandatory and one
                                   optional parameter, curve strength (-1..1), inflection point (0..1
                                   default 0.5) Strength adjustments below zero will cause erroneuos
                                   coloring of dark tones

  -cv   --contrast-value           Adjusts contrast of HSV value. Takes one mandatory and one optional
                                   parameter, curve strength (-1..1), inflection point (0..1 default
                                   0.5) Strength adjustments below zero will cause erroneuos coloring
                                   of dark tones

  -cS   --contrast-hsv-saturation  Adjusts contrast of HSV saturation. Takes one mandatory and one
                                   optional parameter, curve strength (-1..1), inflection point (0..1
                                   default 0.5)

  -gaS  --gamma-hsv-saturation     Adjusts gamma of HSV saturation. Takes a single parameter
                                   (0.01..9.99)

        --tolight                  A preset with multiple filters to convert dark scheme to light

Usage:
  colschman [-filter] <sourcefile> [<targetfile>]
  colschman [-filter][=param1][,param2][,param3] <sourcefile> [<targetfile>]
  colschman [-filter1] [--filter2] <sourcefile> [<targetfile>]
  colschman [-filter][(rangeattr1:min-max,rangeattr2:min-max)[=param] <sourcefile> [<targetfile>]
  colschman [-filter][(rangeattr:min/slope-max/slope)[=param] <sourcefile> [<targetfile>]
  colschman [-filter][(rangeattr:minstart,minend,maxstart,maxend)[=param] <sourcefile> [<targetfile>]

Example:
  colschman -al=0.1,0.9 -s(hue:40/10-180/10)=1.2 my_scheme.icls fixed_scheme.icls

Range attributes:
  h, hue        Hue           |  r, red        Red
  s, sat        Saturation    |  g, green      Green
  l, light      Lightness     |  b, blue       Blue
  v, value      Value         |  bri, bright   Brightness


Usage:
  colschman [-filter] <sourcefile> [<targetfile>]
  colschman [-filter][=param1][,param2][,param3] <sourcefile> [<targetfile>]
  colschman [-filter1] [-filter2] <sourcefile> [<targetfile>]
  colschman [-filter][(rangeattr1:min-max,rangeattr2=min/slope-max/slope)[=param] <sourcefile> [<targetfile>]
  colschman [-filter][(rangeattr:minstart,minend,maxstart,maxend)[=param] <sourcefile> [<targetfile>]

Example:
  colschman -al=0.1,0.9 -s(hue:40/10-180/10)=1.2 my_scheme.icls fixed_scheme.icls

Range attributes:
  h, hue        Hue           |  r, red        Red
  s, sat        Saturation    |  g, green      Green
  l, light      Lightness     |  b, blue       Blue
  v, value      Value         |  bri, bright   Brightness
  
  
```

#### Issues

Adding lightness or value gain on dark colors causes saturated colors


### For developers

#### What's new in version 0.2 and 0.3

##### Version 0.2

+ ColorFilter is not more abtract base class, but the only filter type replacing HslFilter, RgbFilter and HsvFilter
+ ColorBase class is renamed simply to Color
+ Filter delegate signature changed. All filters must handle set of colors provided in IEnumerable\<Color>.
New delegate signature is:
   
```C#
Func<IEnumerable<Color>, object[], IEnumerable<Color>>
```

##### Version 0.3

+ The whole new Color class replaces all other color representations. Color class can hold all RGB, HSL and HSL color 
attributes, and it makes conversion only on demand and automatically without user knowing nothing of it
+ HexRgb class provides methods to conveting hex strings to color. Rgb8Bit is removed along with other old color formats.

##### Version 0.3.2

+ HDR color processing internally. Colors are only clamped to legal values at the end of filter chain
+ A bunch of bug fixes


#### ToDo

+ Range parameter validation
+ SchemeFormat specific extra processing
    + IntelliJ: switch parent scheme based on light/dark background setting
+ More Unit tests
+ Support for Visual Studio Code
+ Support for CSS and HTML files
+ Proper HSV<->HSL conversions, now done by converting to RGB first


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
                // dampen "neon" colors before inversion so don't get too dark
                .Add(FilterBundle.GainLightness, 0.6, 
                    new ColorRange().Brightness(0.7, 1, 0.15, 0).Saturation(0.7, 1, 0.1, 0)) 
                 // invert image
                .Add(FilterBundle.InvertPerceivedBrightness)
                 // adjust levels
                .Add(FilterBundle.LevelsLightness, 0.1, 0.9, 1, 0.1, 1)
                 // yellow-neon green boost
                .Add(FilterBundle.GammaRgb, 1.7, 
                    new ColorRange().Hue(37, 56, 6, 20).Lightness(0.04, 0.6, 0, 0.2))
                // yellow-neon green boost
                .Add(FilterBundle.GainHslSaturation, 1.7, 
                    new ColorRange().Hue(37, 56, 6, 20).Lightness(0.04, 0.6, 0, 0.2))
                // add saturation for weak colors
                .Add(FilterBundle.GammaHslSaturation, 1.4, 
                    new ColorRange().Saturation4P(0.1, 0.1, 0.5, 0.7));
            
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

#### Creating a filter delegate that uses range system

```C#
public static IEnumerable<Color> GammaRgb(IEnumerable<Color> colors, params object[] filterParams)
{
    ColorRange range;
    (range, filterParams) = FilterUtils.GetRangeAndRemainingParams(filterParams);
    foreach (var color in colors) {   
     
        var rangeFactor = FilterUtils.GetRangeFactor(range, color);
        var filtered = new Color(color);
        
        if (filterParams.Any()) {
            double gamma = FilterUtils.TryParseDouble(filterParams[0]) ?? 1.0;
            filtered.Red = ColorMath.Gamma(color.Red, gamma);
            filtered.Green = ColorMath.Gamma(color.Green, gamma);
            filtered.Blue = ColorMath.Gamma(color.Blue, gamma);
        }
        
        yield return color.InterpolateWith(filtered, rangeFactor);
    }
}
```

And registering it to be used from command line 
```C#
CliArgs.Register(
    options: new List<string> {"-ga", "--gamma"}, 
    filter: GammaRgb, 
    minParams: 1, 
    maxParams: 1,
    desc: "Adjusts gamma of all RGB channels equally. Accepts single parameter 0.01..9.99");
```
