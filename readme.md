### Color Scheme Inverter

## ** WORK IN PROGRESS **

This is going to be a tiny tool for inverting color schemes from dark to light and vice versa.
Works currently with JetBrains IDEA (.icls) and Visual Studio (.vstheme) color scheme files.

Current operation includes luminance invert filter and saturation enhancement.


#### ToDo

+ Add more filters, at least for adjusting saturation and gamma 
  + Levels (gamma, black, white) adjustements,
  + Gamma adjustment for saturation
  + Contrast (levels may be enough?)
  + RBG gamma and levels adjustments (implementation that doesn't affect b/w colors at all!)

+ CLI arguments to apply desired filters with desired parameters

+ Support for CSS and HTML files? What else?

+ Automatic detection which scheme is originally dark or light. For using pre-made settings for inversion process.

+ Change the project name. Inversion is not any more the only way to tweak colors!!

#### Filtering colors in Program.cs

```c#
using ColorSchemeInverter.Filters;
using ColorSchemeInverter.SchemeFileSupport;

SchemeFormat schemeFormat = SchemeFormatUtil.GetFormatFromExtension(Path.GetExtension(sourceFileName));

var filters = new FilterSet()
    .Add(FilterBundle.LightnessInvert)
    .Add(FilterBundle.SaturationGain, 1.5);

ColorSchemeProcessor p = new ColorSchemeProcessor(schemeFormat);
p.ProcessFile(sourceFile, targetFile, filters);
```