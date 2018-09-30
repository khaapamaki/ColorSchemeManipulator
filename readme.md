#### *** You are in development branch ***

## Color Scheme Manipulator 0.5

This is a tiny command line tool for adjusting colors in color schemes. 
Works currently with JetBrains IDEA (.icls), Visual Studio (.vstheme) and VS Code color scheme files (.json).

This can also filter png/jpg-files for quick testing.

### Features

+ Chainable filters in single run
+ Each filter can have fully customizable color range
with multiple attributes to narrow the range where the filter affects
+ Color ranges can have smooth slopes
+ Includes a set of basic filters like gain, gamma, levels, contrast, saturation, hue, but also special filters like
inverting image by its lightness, or adjusting contrast of saturation channel
+ Can process RGB, HSL and HSV channels.


### Usage and currently available filters
```
Usage:
  colschman [-filter] <sourcefile> [<targetfile>]
  colschman [-filter][=param1][,param2][,param3] <sourcefile> [<targetfile>]
  colschman [-filter1] [--filter2] <sourcefile> [<targetfile>]
  colschman [-filter][(rangeattr1:min-max,rangeattr2:min-max)[=param] <sourcefile> [<targetfile>]
  colschman [-filter][(rangeattr:min/slope-max/slope)[=param] <sourcefile> [<targetfile>]
  colschman [-filter][(rangeattr:minstart,minend,maxstart,maxend)[=param] <sourcefile> [<targetfile>]


Available Filters:

  -h=<offset>                     Hue shift.
  --hue=<offset>                  <offset> is hue offset in range of -360..360 (0)

  -s=<gain>                       Saturation gain.
  --saturation=<gain>             <gain> is multiplier in range of 0..10 (1.0)

  -g=<gain>                       RGB gain.
  --gain=<gain>                   <gain> is multiplier in range of 0..10 (1.0)

  -l=<gain>                       Lightness gain.
  --lightness=<gain>              <gain> is multiplier in range of 0..10 (1.0)

  -v=<gain>                       Value gain.
  --value=<gain>                  <gain> is multiplier in range of 0..10 (1.0)

  -c=<contrast>[,<ip>]            Contrast.
  --contrast=<contrast>[,<ip>]    <contrast> is curvature strength in range of -1..1 (0.0), <ip> is
                                  inflection point in range of 0..1 (0.5)

  -cl=<contrast>[,<ip>]           Applies contrast curve to lightness.
  --contrast-lightness=...        <contrast> is curvature strength in range of -1..1 (0), <ip> is
                                  inflection point in range of 0..1 (0.5)

  -cv=<contrast>[,<ip>]           Applies contrast curve to value.
  --contrast-value=...            <contrast> is curvature strength in range of -1..1 (0), <ip> is
                                  inflection point in range of 0..1 (0.5)

  -cs=<contrast>[,<ip>]           Applies contrast curve to saturation.
  --contrast-saturation=...       <contrast> is curvature strength in range of -1..1 (0), <ip> is
                                  inflection point in range of 0..1 (0.5)

  -ga=<gamma>                     Gamma correction for all RGB channels equally.
  --gamma=<gamma>                 <gamma> is value in range of 0.01..9.99 (1.0)

  -gar=<gamma>                    Adjusts gamma of red channel.
  --gamma-red=<gamma>             <gamma> is value in range of 0.01..9.99 (1.0)

  -gag=<gamma>                    Adjusts gamma of green channel.
  --gamma-green=<gamma>           <gamma> is value in range of 0.01..9.99 (1.0)

  -gab=<gamma>                    Adjusts gamma of blue channel.
  --gamma-blue=<gamma>            <gamma> is value in range of 0.01..9.99 (1.0)

  -gal=<gamma>                    Adjusts gamma of lightness.
  --gamma-lightness=<gamma>       <gamma> is value in range of 0.01..9.99 (1.0)

  -gav=<gamma>                    Adjusts gamma of value.
  --gamma-value=<gamma>           <gamma> is value in range of 0.01..9.99 (1.0)

  -gas=<gamma>                    Adjusts gamma of saturation.
  --gamma-saturation=<gamma>      <gamma> is value in range of 0.01..9.99 (1.0)

  -le=<ib>,<iw>,<g>,<ob>,<ow>     Adjusts levels of all RGB channels.
  --levels=...                    <ib> is input black 0..1 (0), <iw> is input white 0..1 (1), <g> is
                                  gamma 0.01..9.99 (1), <ob> is output black 0..1 (0), <ow> is output
                                  white 0..1 (1)

  -ler=<ib>,<iw>,<g>,<ob>,<ow>    Adjusts levels of red channel.
  --levels-red=...                <ib> is input black 0..1 (0), <iw> is input white 0..1 (1), <g> is
                                  gamma 0.01..9.99 (1), <ob> is output black 0..1 (0), <ow> is output
                                  white 0..1 (1)

  -leg=<ib>,<iw>,<g>,<ob>,<ow>    Adjusts levels of red channel.
  --levels-green=...              <ib> is input black 0..1 (0), <iw> is input white 0..1 (1), <g> is
                                  gamma 0.01..9.99 (1), <ob> is output black 0..1 (0), <ow> is output
                                  white 0..1 (1)

  -leb=<ib>,<iw>,<g>,<ob>,<ow>    Adjusts levels of red channel.
  --levels-blue=...               <ib> is input black 0..1 (0), <iw> is input white 0..1 (1), <g> is
                                  gamma 0.01..9.99 (1), <ob> is output black 0..1 (0), <ow> is output
                                  white 0..1 (1)

  -al=<ob>,<ow>,<g>               Adjusts levels of RGB channels by normalizing levels so that darkest
  --auto-levels=<ob>,<ow>,<g>     color will be black and lightest color max bright.
                                  <ob> is output black 0..1 (0), <ow> is output white 0..1 (1), <g> is
                                  gamma 0.01..9.99 (1)

  -lel=<ib>,<iw>,<g>,<ob>,<ow>    Adjusts levels of lightness.
  --levels-lightness=...          <ib> is input black 0..1 (0), <iw> is input white 0..1 (1), <g> is
                                  gamma 0.01..9.99 (1), <ob> is output black 0..1 (0), <ow> is output
                                  white 0..1 (1)

  -lev=<ib>,<iw>,<g>,<ob>,<ow>    Adjusts levels of value.
  --levels-value=...              <ib> is input black 0..1 (0), <iw> is input white 0..1 (1), <g> is
                                  gamma 0.01..9.99 (1), <ob> is output black 0..1 (0), <ow> is output
                                  white 0..1 (1)

  -les=<ib>,<iw>,<g>,<ob>,<ow>    Adjusts levels of saturation.
  --levels-saturation=...         <ib> is input black 0..1 (0), <iw> is input white (1), <g> is gamma
                                  0.01..9.99 (1), <ob> is output black 0..1 (0), <ow> is output white
                                  0..1 (1)

  -i                              Inverts RGB channels.
  --invert-rgb

  -il                             Inverts lightness.
  --invert-lightness

  -iv                             Inverts value.
  --invert-value

  -ib                             Inverts perceived brightness.
  --invert-brightness

  -gsb                            Converts to gray scale based on perceived brightness.
  --grayscale-brightness

  --clamp                         Clamps color values to normal range of 0..1. Tries to preserve hue.
                                  This is automatically done as last filter.


Experimental Filters:

  -ibc[=<corr>]                   Inverts perceived brightness with correction parameter.
  --invert-brightness-corr=...    <corr> is value between 0..1, 0 is safest conversion, 1 is closest to
                                  truth but also causes clipping of some colors.

  -ilv=<mix>                      Inverts colors using both lightness and value, by mixing the result by
  --invert-lightness-value=...    parameter (0..1)
                                  <mix> is mix parameter 0..1, 0 is full lightness inversion, 1 is full
                                  value inversion.

  -b2l                            Translates perceived brightness to lightness.
  --brightness-to-lightness

  -b2v                            Translates perceived brightness to value.
  --brightness-to-value

  -cS=<contrast>[,<ip>]           Applies contrast curve to HSV saturation.
  --contrast-hsv-sat=...          <contrast> is curvature strength in range of -1..1 (0), <ip> is
                                  inflection point in range of 0..1 (0.5)

  -gaS=<gamma>                    Adjusts gamma of HSV saturation.
  --gamma-hsv-sat=<gamma>         <gamma> is value in range of 0.01..9.99 (1.0)

  -S=<value>                      HSV saturation gain.
  --hsv-saturation=<value>        <value> is multiplier in range of 0..10 (1.0)

  --tolight                       A preset with multiple filters to convert dark scheme to light


Range attributes:
  h, hue        Hue           |  r, red        Red
  s, sat        Saturation    |  g, green      Green
  l, light      Lightness     |  b, blue       Blue
  v, value      Value         |  bri, bright   Brightness  


Example:
  colschman -al=0.1,0.9 -s(hue:40/10-180/10)=1.2 my_scheme.icls fixed_scheme.icls
```

### Issues

Hunting for them...

### ToDo

+ Range parameter validation
+ SchemeFormat specific extra processing
    + IntelliJ: switch parent scheme based on light/dark background setting
+ More Unit tests
+ Support for Visual Studio Code (implemented partially, not tested)
+ Proper HSV<->HSL conversions, now done by converting to RGB first
+ Brief help in addition to current verbose one


## For developers

### Adding support for a new color scheme type

All color files are processed in _ColorFileProcessor_ class. It uses handlers that are specific to 
color scheme format (or any other file). A handler must conform generic _IColorFileHandler_ interface where generic type 
T is usually string, but it can also be anything else like Bitmap.

```c#
public interface IColorFileHandler<T>
{
    T ReadFile(string sourceFile);
    void WriteFile(T data, string targetFile);   
    IEnumerable<Color> GetColors(T source);
    T ReplaceColors(T source, IEnumerable<Color> colors);
}
```

**NOTE:**
If you are making a handler for a color scheme that uses simple hex string for color definitions, abstract class **SchemeFileHandler** provides
useful tools for parsing file with regular expressions, as well as applying filters. Subclass it, adjust some properties and you are good to go.

Currently, a new color scheme format must be added in _SchemeFormat_ enum. And **SchemeUtils.GetSchemeHandlerByFormat(SchemeFormat format)** 
must return an instance of the handler. And **SchemeUtils.GetFormatFromExtension(string extension)** must 
return SchemeFormat matching the file extension.


```c#
    public enum SchemeFormat
    {
        Idea,
        VisualStudio,
        Image,
        XXXX,
        Unknown
    }
    
    public static class SchemeUtils
    {
        public static SchemeFormat GetFormatFromExtension(string extension)
        {
            if (extension.StartsWith(".")) {
                extension = extension.Substring(1);
            }

            switch (extension.ToLower()) {
                case "icls":
                    return SchemeFormat.Idea;
                case "XXXX":
                    return SchemeFormat.XXXX;
                default:
                    return SchemeFormat.Unknown;
            }
        }

        public static IColorFileHandler<string> GetSchemeHandlerByFormat(SchemeFormat schemeFormat)
        {
            switch (schemeFormat) {
                case SchemeFormat.Idea:
                    return new IdeaSchemeFileHandler();
                case SchemeFormat.XXXXX:
                    return new XXXXXFileHandler();                
                default:
                    return null;
            }
        }
    }
```

### Manually filtering (no using CLI arguments)

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
                .Add(FilterBundle.GainLightness,
                    new ColorRange().Brightness(0.7, 1, 0.15, 0).Saturation(0.7, 1, 0.1, 0),
                    0.6) 
                 // invert image
                .Add(FilterBundle.InvertPerceivedBrightness)
                 // adjust levels
                .Add(FilterBundle.LevelsLightness, null, 0.1, 0.9, 1, 0.1, 1)
                 // yellow-neon green boost
                .Add(FilterBundle.GammaRgb,
                    new ColorRange().Hue(37, 56, 6, 20).Lightness(0.04, 0.6, 0, 0.2),
                    1.7)
                // yellow-neon green boost
                .Add(FilterBundle.GainHslSaturation,
                    new ColorRange().Hue(37, 56, 6, 20).Lightness(0.04, 0.6, 0, 0.2),
                    1.7)
                // add saturation for weak colors
                .Add(FilterBundle.GammaHslSaturation,
                    new ColorRange().Saturation4P(0.1, 0.1, 0.5, 0.7),
                    1.4 );
            
            ColorSchemeProcessor p = new ColorSchemeProcessor(schemeFormat);
            p.ProcessFile(sourceFile, targetFile, filters);
            
        }
    }
}
```

### Filtering by CLI arguments

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

### Creating a filter that uses range system

Filter delegate signature:
```c#
    Func<IEnumerable<Color>, ColorRange, double[], IEnumerable<Color>>
```


```C#
        public static IEnumerable<Color> GammaRgb(IEnumerable<Color> colors, ColorRange colorRange = null,
            params double[] filterParams)
        {
            foreach (var color in colors) {
                var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
                var filtered = new Color(color);

                if (filterParams.Any()) {
                    double gamma = filterParams[0];
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
            CliArgs.Register(new List<string> {"-ga", "--gamma"}, 
                GammaRgb, 
                minParams: 1,
                maxParams: 1,
                paramList: "=<gamma>",
                desc: "Gamma correction for all RGB channels equally.",
                paramDesc: "<gamma> is value in colorRange of 0.01..9.99 (1.0)");
```
