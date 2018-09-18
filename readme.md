## Color Scheme Manipulator

This is a tiny command line tool for adjusting colors in color schemes.
Works currently with JetBrains IDEA (.icls) and Visual Studio (.vstheme) color scheme files.

Also has option to filter png-files for quick testing.

#### Currently available filters and corresponding CLI options
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
  -ga   --gamma                    Adjusts gamma of all RGB channels equally. Accepts single parameter
                                   0.01..9.99
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
  -al   --auto-levels              @AutoLevelsRgb
  -i    --invert-rgb               @InvertRgb
  -il   --invert-lightness         @InvertLightness
  -iv   --invert-value             @InvertValue
  -ib   --invert-brightness        Inverts perceived brightness
  -gsb  --grayscale-brightness     Converts to gray scale based on perceived brightness
        --clamp                    @Clamp

Experimental Filters:
  -lel  --levels-lightness         @LevelsLightness
  -lev  --levels-value             @LevelsValue
  -les  --levels-saturation        @LevelsHslSaturation
  -leS  --levels-hsv-saturation    @LevelsHsvSaturation
  -ibc  --invert-brightness-corr   Inverts perceived brightness
  -ilv  --invert-lightness-value   Inverts colors using both lightness and value, by mixing the result
  -b2l  --brightness-to-lightness  @BrightnessToLightness
  -b2v  --brightness-to-value      @BrightnessToValue
        --tolight                  @ToLight

Usage:
  colschman [-filter] <sourcefile> [<targerfile>]
  colschman [-filter][=param1][,param2][,param3] <sourcefile> [<targerfile>]
  colschman [-filter1] [-filter2] <sourcefile> [<targerfile>]
  colschman [-filter][(rangeattr1:min-max,rangeattr2=min/slope-max/slope)[=param] <sourcefile> [<targerfile>]
  colschman [-filter][(rangeattr:minstart,minend,maxstart,maxend)[=param] <sourcefile> [<targerfile>]

Example:
  colschman -il -gs=1.1 --contrast=0.2,0.6 <sourcefile> <targetfile>
    
Example using filter with color range:
  colschman "--gamma(sat: 0.5-1, l: 0-0.5) = 1.5" <sourcefile> <targetfile>
    
Example using filter with color range defined with four points: (attribute: min1, min2, max1, max2)
  colschman "--gamma(sat: 0.4, 0.5, 1, 1, lightness:0, 0, 0.5, 0.7) = 1.5" <sourcefile> <targetfile>
    
Example using filter with color range with slope parameters: (attribute: min/slope - max/slope)
  colschman "--gamma(sat: 0.5/0.1 - 0.9/0.1, l: 0.1/0.1- 0.5/0.1) = 1.5" <sourcefile> <targetfile>
    
```

### Issues

Adding lightness or value gain on dark colors causes saturated colors


#### ToDo

+ Range parameter validation
+ SchemeFormat specific extra processing
    + IntelliJ: switch parent scheme based on light/dark background setting
+ More Unit tests
+ Support for Visual Studio Code
+ Support for CSS and HTML files
+ Proper HSV<->HSL conversions, now done by converting to RGB first


