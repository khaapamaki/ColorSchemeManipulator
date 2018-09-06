### Color Scheme Inverter

##### ** WORK IN PROGRESS **

This is a tiny command line tool for adjusting colors in color schemes.
Works currently with JetBrains IDEA (.icls) and Visual Studio (.vstheme) color scheme files.


### Currently available filters (cli options)
```
Available Filters:
   -b  --brightness   =gain
   -c  --contrast     =value(-1..1),midpoint(0..1)
   -g  --gamma        =gamma(0..x)
   -i  --invert
   -il --invert-lightness
   -lg --lightness-gain
   -s  --saturation
   -sg --saturation-gamma
   -l  --lightness-gain
   -sc --saturation-contrast
   -lc --lightness-contrast
   
Usage:
    -b=1.1
    --contrast=0.1
    --contrast=0.1,0.6

```

#### ToDo

+ More filters
  + Levels (gamma, black, white) adjustments,
  + RBG levels adjustments, plus implementation that doesn't affect b/w colors at all
  + RGB gamma that doesn't affect b/w colors

+ Support for CSS and HTML files? What else?

+ Automatic detection which scheme is originally dark or light. For using pre-made settings for inversion process

+ Change the project name. Inversion is not any more the only way to tweak colors!!


#### Manually filtering bypassing CLI arguments

```c#
using System;
using System.IO;
using ColorSchemeInverter.Filters;
using ColorSchemeInverter.SchemeFileSupport;
using ColorSchemeInverter.CLI;

[...]

SchemeFormat schemeFormat = SchemeFormatUtil.GetFormatFromExtension(Path.GetExtension(sourceFileName));

var filters = new FilterSet()
    .Add(FilterBundle.LightnessInvert)
    .Add(FilterBundle.SaturationContrast, 0.3)
    .Add(FilterBundle.SaturationGain, 1.2)
    .Add(FilterBundle.Gain, 1.1)
    .Add(FilterBundle.Contrast, 0.3);

ColorSchemeProcessor p = new ColorSchemeProcessor(schemeFormat);
p.ProcessFile(sourceFile, targetFile, filters);
```