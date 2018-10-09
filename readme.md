#### *** You are in development branch ***

## Color Scheme Manipulator 0.7

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
  --hue=<offset>                  <offset> is hue offset in colorRange of -360..360 (0)

  -s=<gain>                       Saturation gain.
  --saturation=<gain>             <gain> is multiplier in colorRange of 0..10 (1.0)

  -g=<gain>                       RGB gain.
  --gain=<gain>                   <gain> is multiplier in colorRange of 0..10 (1.0)

  -l=<gain>                       Lightness gain.
  --lightness=<gain>              <gain> is multiplier in colorRange of 0..10 (1.0)

  -v=<gain>                       Value gain.
  --value=<gain>                  <gain> is multiplier in colorRange of 0..10 (1.0)

  -c=<contrast>[,<ip>]            Adjusts contrast by S-spline curve.
  --contrast=<contrast>[,<ip>]    <contrast> is curvature strength in colorRange of -1..1 (0.0), <ip> is
                                  inflection point in colorRange of 0..1 (0.5)

  -cl=<contrast>[,<ip>]           Applies contrast curve to lightness.
  --contrast-lightness=...        <contrast> is curvature strength in colorRange of -1..1 (0), <ip> is
                                  inflection point in colorRange of 0..1 (0.5)

  -cv=<contrast>[,<ip>]           Applies contrast curve to value.
  --contrast-value=...            <contrast> is curvature strength in colorRange of -1..1 (0), <ip> is
                                  inflection point in colorRange of 0..1 (0.5)

  -cs=<contrast>[,<ip>]           Applies contrast curve to saturation.
  --contrast-saturation=...       <contrast> is curvature strength in colorRange of -1..1 (0), <ip> is
                                  inflection point in colorRange of 0..1 (0.5)

  -ga=<gamma>                     Gamma correction for all RGB channels equally.
  --gamma=<gamma>                 <gamma> is value in colorRange of 0.01..9.99 (1.0)

  -gar=<gamma>                    Adjusts gamma of red channel.
  --gamma-red=<gamma>             <gamma> is value in colorRange of 0.01..9.99 (1.0)

  -gag=<gamma>                    Adjusts gamma of green channel.
  --gamma-green=<gamma>           <gamma> is value in colorRange of 0.01..9.99 (1.0)

  -gab=<gamma>                    Adjusts gamma of blue channel.
  --gamma-blue=<gamma>            <gamma> is value in colorRange of 0.01..9.99 (1.0)

  -gal=<gamma>                    Adjusts gamma of lightness.
  --gamma-lightness=<gamma>       <gamma> is value in colorRange of 0.01..9.99 (1.0)

  -gav=<gamma>                    Adjusts gamma of value.
  --gamma-value=<gamma>           <gamma> is value in colorRange of 0.01..9.99 (1.0)

  -gas=<gamma>                    Adjusts gamma of saturation.
  --gamma-saturation=<gamma>      <gamma> is value in colorRange of 0.01..9.99 (1.0)

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

  -al=<min>,<max>,<g>             Auto levels RGB channels by normalizing them by HSV values to full
  --auto-levels=...               scale between given minimum and maximum.
                                  <min> is output min 0..1 (0), <max> is output max 0..1 (1), <g> is
                                  gamma 0.01..9.99 (1)

  -all=<min>,<max>,<g>            Auto levels lightness by normalizing values to full scale between
  --auto-levels-lightness=...     given minimum and maximum.
                                  <min> is output min 0..1 (0), <max> is output max 0..1 (1), <g> is
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

  -ipl                            Inverts perceived lightness.
  --invert-perc-lightness

  -gsb                            Converts to gray scale based on perceived brightness.
  --grayscale-brightness

  -gsl                            Converts to gray scale based on perceived brightness.
  --grayscale-ligthness

  -gsv                            Converts to gray scale based on perceived brightness.
  --grayscale-value

  --min-lightness=<min>           Limits lower end of lightness.
                                  <min> minimum lightness

  --max-lightness=<max>           Limits higher end of lightness.
                                  <max> max lightness

  --min-value=<min>               Limits lower end of HSV value.
                                  <min> minimum lightness

  --max-value=<max>               Limits higher end of value.
                                  <max> max HSV value

  --max-saturation=<max>          Limits higher end of saturation.
                                  <max> max saturation

  --max-saturation-hsv=<max>      Limits higher end of HSV saturation.
                                  <max> max saturation

  --clamp                         Clamps color values to normal colorRange of 0..1. Tries to preserve
                                  hue. This is automatically done as last filter.

Experimental Filters:

  -ipc[=<corr>]                   Inverts perceived brightness with correction parameter.
  --invert-perceived-corr=...     <corr> is value between 0..1, 0 is safest conversion, 1 is closest to
                                  truth but also causes clipping of some colors.

  -ilv=<mix>                      Inverts colors using both lightness and value, by mixing the result by
  --invert-lightness-value=...    parameter (0..1)
                                  <mix> is mix parameter 0..1, 0 is full lightness inversion, 1 is full
                                  value inversion.

  -ipm=<mix>                      Inverts colors using both lightness and value, by mixing the result by
  --invert-perceived-mixed=...    parameter (0..1)
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


  -ipv                            Inverts perceived brightness.
  --invert-perceived-value
  
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
+ More Unit tests
+ Support for Visual Studio Code (implemented partially, not tested)
+ Proper HSV<->HSL conversions, now done by converting to RGB first


## For developers

### Adding support for a new color scheme format

All color files are processed in **ColorFileProcessor** class. It uses handlers that are specific to 
color scheme format (or any other file). A handler must conform generic **IColorFileHandler** interface where generic type 
**TData** is usually string, but it can also be anything else like **Bitmap**, though when **HandlerRegister<TData>** is used, you need to create
a new one for your custom data type.

```c#
public interface IColorFileHandler<TData>
{
    // Checks a file if can processed with the handler
    bool Accepts(string sourceFile);
    
    // Read the contents of the file
    TData ReadFile(string sourceFile);
    
    // Write the contents to a file
    void WriteFile(TData data, string targetFile);
    
    // Parses and enumerates all the color from the file
    IEnumerable<Color> GetColors(TData source);
    
    // Replaces original color values from the data with new colors (must be in the same order)
    // Return altered data
    TData ReplaceColors(TData data, IEnumerable<Color> colors);
}
```

Register your handler in **CliAppRunner.RegisterHandlers()** metdod. This is only needed if you want to use **HandlerRegister** to 
automatically find a proper handler by the file extension/contents.

```c#
namespace ColorSchemeManipulator
{
    public class CliAppRunner
    {
        [...]
        
        private HandlerRegister<string> _schemeHandlerRegister = new HandlerRegister<string>();
        
        public void RegisterHandlers()
        {
            _schemeHandlerRegister.Register(new IDEAFileHandler());
            _schemeHandlerRegister.Register(new VisualStudioFileHandler());
            _schemeHandlerRegister.Register(new VSCodeFileHandler());
        }
            
        [...]       
    }
}
```

Get a proper handler for a file.

```c#
var schemeHandler = _schemeHandlerRegister.GetHandlerForFile(sourceFile);
```

**NOTE:**
If you are making a handler for a color scheme that uses simple hex string for color definitions, abstract class **HexRgbFileHandler** provides
useful tools for parsing file with regular expressions, as well as applying filters. Subclass it, adjust some properties and you are good to go.


### Manually filtering (without using CLI arguments)

In this example three successive filters are added to a **FilterSet** object and **IDEAFileHandler** is used to convert IDEA color schemes.
If you want automatically detect file format you can use HandlerRegister object to first register all your handlers and then get the correct handler
for the current file by invoking `GetHandlerForFile(string sourceFile)` methdod.

```c#
using System;
using ColorSchemeManipulator.Filters;
using ColorSchemeManipulator.SchemeFormats;
using ColorSchemeManipulator.SchemeFormats.Handlers;

namespace ColorSchemeManipulator
{
    internal class Program
    {
        public static void Main(string[] args)
        {    
            [...]   
            
            var filters = new FilterSet()
                .Add(FilterBundle.InvertLightness)
                .Add(FilterBundle.AutoLevelsLRgb, 
                    colorRange: null,
                    0.1, 1, 1.05)
                .Add(FilterBundle.GammaHslSaturation,
                    new ColorRange()
                        .Saturation4P(0.1, 0.1, 0.3, 0.6)
                        .Brightness4P(0, 0.1, 0.4, 0.7),
                    2.4)
                 
            var processor = new ColorFileProcessor<string>(new IDEAFileHandler());
            processor.ProcessFile(sourceFile, targetFile, filters);       
        }
    }
}
```

### Creating a filter that uses range system

From version 0.7 and above there are two different type of filters, one for handling a single color value individually,
and another one that enumerates the whole set of colors. Both are chainable together.

Filter are used as Func<> delegates.

#### Single color filter delegate type

```c#
Func<Color, ColorRange, double[], Color>
```

The filter must have signature of
```c#
Color MyFilter(Color color, ColorRange range, params double[] params)
```

#### Enumerating color filter delegate type

```c#
Func<IEnumerable<Color>, ColorRange, double[], IEnumerable<Color>>
```

The filter must have signature of
```c#
IEnumerable<Color>  MyFilter(Enumerable<Color> colors, ColorRange range, params double[] params)
```


Example of a single color filter. This maybe defined in **FilterBundle** class or anywhere as long as you register it to **CliArgs**. 
See below.

```C#
public static Color GammaRgb(Color color, ColorRange colorRange = null, params double[] filterParams)
{
    if (filterParams.Any()) {
        double gamma = filterParams[0];
        filtered.Red = ColorMath.Gamma(color.Red, gamma);
        filtered.Green = ColorMath.Gamma(color.Green, gamma);
        filtered.Blue = ColorMath.Gamma(color.Blue, gamma);
    }
    
    return filtered;
}
```

Another example of enumerating filter. This will find lowest and highest value from color set and uses this before
filtering the color data

```C#
public static IEnumerable<Color> AutoLevelsRgb(IEnumerable<Color> colors, ColorRange colorRange = null,
    params double[] filterParams)
{
    List<Color> cache = colors.ToList();
    (double inBlack, double inWhite) = FilterUtils.GetLowestAndHighestValue(cache);

    var result = cache.Select(
        color =>
        {
            var rangeFactor = FilterUtils.GetRangeFactor(colorRange, color);
            var filtered = new Color(color);
            (double outBlack, double outWhite, double gamma) =
                FilterUtils.GetAutoLevelParameters(filterParams);
            filtered.Red = ColorMath.Levels(color.Red, inBlack, inWhite, gamma, outBlack, outWhite);
            filtered.Green = ColorMath.Levels(color.Green, inBlack, inWhite, gamma, outBlack, outWhite);
            filtered.Blue = ColorMath.Levels(color.Blue, inBlack, inWhite, gamma, outBlack, outWhite);

            color.InterpolateWith(filtered, rangeFactor);
            return color;
        });

    foreach (var color in result) {
        yield return color;
    }
}
```

Finally you need to register the filter to be used from command line 

```C#
CliArgs.Register(new CliArgBuilder()
    .Filter(GammaRgb)
    .AddOptions("-ga", "--gamma") // you can add as many options you want
    .Params(1) // .Params(min, max) is also available
    // all below are only for printing help
    .Description("Gamma correction for all RGB channels equally.")
    .ParamString("=<gamma>")
    .ParamDescription("<gamma> is value in colorRange of 0.01..9.99 (1.0)")
);
```

Legacy non-fluent way

```C#
CliArgs.Register(
    options: new List<string> {"-ga", "--gamma"}, 
    filter: GammaRgb, 
    minParams: 1,
    maxParams: 1,
    paramList: "=<gamma>",
    desc: "Gamma correction for all RGB channels equally.",
    paramDesc: "<gamma> is value in colorRange of 0.01..9.99 (1.0)");
```
