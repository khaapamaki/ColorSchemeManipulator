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
    public class ColorFilter
    {
        private Func<IEnumerable<Color>, ColorRange, double[], IEnumerable<Color>> FilterDelegate { get; }
        private Func<Color, ColorRange, double[], Color> FilterDelegate1 { get; }
        private double[] Parameters { get; }
        private ColorRange ColorRange { get; }

        public ColorFilter(Func<IEnumerable<Color>, ColorRange, double[], IEnumerable<Color>> filterDelegate,
            ColorRange colorColorRange = null,
            params double[] filterParams)
        {
            FilterDelegate = filterDelegate;
            ColorRange = colorColorRange;
            Parameters = filterParams;
        }
        
        public ColorFilter(Func<Color, ColorRange, double[], Color> filterDelegate,
            ColorRange colorColorRange = null,
            params double[] filterParams)
        {
            FilterDelegate1 = filterDelegate;
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
            if (FilterDelegate != null) {
                return ApplyMultiFilter(colors);
            } else {
                return ApplySingleFilter(colors);
            }
        }



        public IEnumerable<Color> ApplyMultiFilter(IEnumerable<Color> colors)
        {
            return FilterDelegate(colors, ColorRange, Parameters); 
        }
        
        
        public IEnumerable<Color> ApplySingleFilter(IEnumerable<Color> colors)
        {
            var result = colors.Select(color => { return FilterDelegate1(color, ColorRange, Parameters); });
            foreach (var color in result) {
                yield return color;
            }
        }
        
        
        
        /// <summary>
        /// Applies the filter for a color enumeration with final color clamping.
        /// </summary>
        /// <param name="colors"></param>
        /// <param name="outputClamping"></param>
        /// <returns></returns>
        private IEnumerable<Color> ApplyTo(IEnumerable<Color> colors, bool outputClamping)
        {
            Console.WriteLine("  " + ToString());
            colors = FilterDelegate(colors, ColorRange, Parameters);

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
                sb.Append((sb.Length > 0 ? ", " : "") + argument);
            }

            string name = FilterDelegate?.Method.Name ?? FilterDelegate1?.Method.Name;
            
            return name + (sb.Length > 0 ? $"({sb})" : "") + (ColorRange != null ? " ==> " + ColorRange  : "");
        }
    }
}