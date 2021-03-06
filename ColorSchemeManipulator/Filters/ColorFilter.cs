using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ColorSchemeManipulator.Colors;
using ColorSchemeManipulator.Ranges;

namespace ColorSchemeManipulator.Filters
{
    /// <summary>
    /// A class that encapsulates a filter delegate and all it's parameters
    /// It also invokes the delegate via ApplyTo() method with a set of colors
    /// </summary>
    public class ColorFilter
    {
        private FilterDelegate Filter { get; }
        public double[] Parameters { get; }
        public ColorRange ColorRange { get; }

        public ColorFilter(FilterDelegate filter,
            ColorRange colorColorRange = null,
            params double[] filterParams)
        {
            Filter = filter;
            ColorRange = colorColorRange;
            Parameters = filterParams;
        }

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

        /// <summary>
        /// Applies the filter for a color enumeration with final color clamping.
        /// </summary>
        /// <param name="colors"></param>
        /// <param name="outputClamping"></param>
        /// <returns></returns>
        private IEnumerable<Color> ApplyTo(IEnumerable<Color> colors, bool outputClamping)
        {
            colors = ApplyTo(colors);
        
            // Final clamping after last filter in chain
            foreach (var color in colors) {
                if (outputClamping)
                    color.ClampExceedingColors();
                yield return color;
            }
        }

        public FilterDelegate GetDelegate()
        {
            return Filter;
        }
        
        public override string ToString()
        {
            return Filter.ToString(ColorRange, Parameters);
        }
    }
}