using System;
using System.Collections.Generic;
using System.Text;
using ColorSchemeManipulator.Colors;

namespace ColorSchemeManipulator.Filters
{
    /// <summary>
    /// A class that encapsulates a filter delegate and it's parameters
    /// It also invokes the delegate via ApplyTo() method with a set of colors
    /// </summary>
    public class ColorFilter
    {
        private Func<IEnumerable<Color>, object[], IEnumerable<Color>> FilterDelegate { get; }
        private object[] Parameters { get; }

        public ColorFilter(Func<IEnumerable<Color>, object[], IEnumerable<Color>> filterDelegate,
            params object[] filterParams)
        {
            FilterDelegate = filterDelegate;
            Parameters = filterParams;
        }

        /// <summary>
        /// Applies the filter for a color enumeration
        /// </summary>
        /// <param name="colors"></param>
        /// <returns></returns>
        public IEnumerable<Color> ApplyTo(IEnumerable<Color> colors)
        {
            return FilterDelegate(colors, Parameters);
        }
        
        /// <summary>
        /// Applies the filter for a color enumeration with final color clamping.
        /// </summary>
        /// <param name="colors"></param>
        /// <param name="outputClamping"></param>
        /// <returns></returns>
        private IEnumerable<Color> ApplyTo(IEnumerable<Color> colors, bool outputClamping)
        {
            colors = FilterDelegate(colors, Parameters);

            // Final clamping after last filter in chain
            foreach (var color in colors) {
                if (outputClamping)
                    color.ClampExceedingColors();
                yield return color;
            }
        }
        
        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var argument in Parameters) {
                sb.Append(argument + " ");
            }

            return FilterDelegate.Method.Name + " " + sb;
        }
    }
}