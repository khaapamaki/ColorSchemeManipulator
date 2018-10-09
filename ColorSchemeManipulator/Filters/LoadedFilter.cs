using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ColorSchemeManipulator.Colors;

namespace ColorSchemeManipulator.Filters
{
    /// <summary>
    /// A class that encapsulates a filter delegate and it's parameters
    /// It also invokes the delegate via ApplyTo() method with a set of colors
    /// </summary>
    public class LoadedFilter
    {
        private FilterWrapper Filter { get; }
        // private Func<IEnumerable<Color>, ColorRange, double[], IEnumerable<Color>> MultiFilter { get; }
        // private Func<Color, ColorRange, double[], Color> SingleFilter { get; }
        private double[] Parameters { get; }
        private ColorRange ColorRange { get; }

        public LoadedFilter(FilterWrapper filter,
            ColorRange colorColorRange = null,
            params double[] filterParams)
        {
            Filter = filter;
            ColorRange = colorColorRange;
            Parameters = filterParams;
        }
        
        // public LoadedFilter(Func<IEnumerable<Color>, ColorRange, double[], IEnumerable<Color>> multiFilter,
        //     ColorRange colorColorRange = null,
        //     params double[] filterParams)
        // {
        //     MultiFilter = multiFilter;
        //     ColorRange = colorColorRange;
        //     Parameters = filterParams;
        // }
        //
        // public LoadedFilter(Func<Color, ColorRange, double[], Color> singleFilterDelegate,
        //     ColorRange colorColorRange = null,
        //     params double[] filterParams)
        // {
        //     SingleFilter = singleFilterDelegate;
        //     ColorRange = colorColorRange;
        //     Parameters = filterParams;
        // }

        /// <summary>
        /// Applies the filter for a color enumeration
        /// </summary>
        /// <param name="colors"></param>
        /// <returns></returns>
        public IEnumerable<Color> ApplyTo(IEnumerable<Color> colors)
        {
            Console.WriteLine("  " + ToString());
            return Filter.ApplyTo(colors, ColorRange, Parameters);
        }

        /*public IEnumerable<Color> ApplyTo(IEnumerable<Color> colors)
        {
            Console.WriteLine("  " + ToString());
            if (MultiFilter != null) {
                return ApplyMultiFilter(colors);
            } else {
                return ApplySingleFilter(colors);
            }
        }

        public IEnumerable<Color> ApplyMultiFilter(IEnumerable<Color> colors)
        {
            return MultiFilter(colors, ColorRange, Parameters); 
        }
             
        public IEnumerable<Color> ApplySingleFilter(IEnumerable<Color> colors)
        {
            var result = colors.Select(color => SingleFilter(color, ColorRange, Parameters));
            foreach (var color in result) {
                yield return color;
            }
        }*/
          
        /// <summary>
        /// Applies the filter for a color enumeration with final color clamping.
        /// </summary>
        /// <param name="colors"></param>
        /// <param name="outputClamping"></param>
        /// <returns></returns>
        // [Obsolete]
        // private IEnumerable<Color> ApplyTo(IEnumerable<Color> colors, bool outputClamping)
        // {
        //     Console.WriteLine("  " + ToString());
        //     colors = MultiFilter(colors, ColorRange, Parameters);
        //
        //     // Final clamping after last filter in chain
        //     foreach (var color in colors) {
        //         if (outputClamping)
        //             color.ClampExceedingColors();
        //         yield return color;
        //     }
        // }
        
        public override string ToString()
        {
            return Filter.ToString(ColorRange, Parameters);
        }
    }
}