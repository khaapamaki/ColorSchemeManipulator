
## Color Scheme Manipulator 0.4

This is a tiny command line tool for adjusting colors in color schemes.
Works currently with JetBrains IDEA (.icls) and Visual Studio (.vstheme) color scheme files.

This can also filter png/jpg-files for quick testing.

#### Usage and currently available filters
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

#### Issues

Hunting for them...
