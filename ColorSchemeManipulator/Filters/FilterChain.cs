using System;
using System.Collections.Generic;
using System.Text;
using ColorSchemeManipulator.Colors;

namespace ColorSchemeManipulator.Filters
{
    /// <summary>
    /// A collection class for storing multiple filters with their parameters that
    /// are meant to be processed sequentially
    /// </summary>
    public class FilterChain
    {
        private readonly List<ColorFilter> _filters = new List<ColorFilter>();

        public FilterChain() { }

        public FilterChain Add(ColorFilter filter)
        {
            _filters.Add(filter);
            return this;
        }

        public FilterChain Add(FilterDelegate filterDelegate, ColorRange colorRange,
            params double[] filterParams)
        {
            _filters.Add(new ColorFilter(filterDelegate, colorRange, filterParams));
            return this;
        }
        
        /// <summary>
        /// Adds enumerating multi filter
        /// </summary>
        /// <param name="multiFilter"></param>
        /// <param name="colorRange"></param>
        /// <param name="filterParams"></param>
        /// <returns></returns>
        public FilterChain Add(Func<IEnumerable<Color>, ColorRange, double[], IEnumerable<Color>> multiFilter,
            ColorRange colorRange,
            params double[] filterParams)
        {
            _filters.Add(new ColorFilter(new FilterDelegate(multiFilter), colorRange, filterParams));
            return this;
        }
        
        public FilterChain Add(Func<IEnumerable<Color>, int, ColorRange, double[], IEnumerable<Color>> multiFilter,
            ColorRange colorRange,
            params double[] filterParams)
        {
            _filters.Add(new ColorFilter(new FilterDelegate(multiFilter), colorRange, filterParams));
            return this;
        }
        
        /// <summary>
        /// Adds single color filter
        /// </summary>
        /// <param name="singleFilter"></param>
        /// <param name="colorRange"></param>
        /// <param name="filterParams"></param>
        /// <returns></returns>
        public FilterChain Add(Func<Color, ColorRange, double[],Color> singleFilter,
            ColorRange colorRange,
            params double[] filterParams)
        {
            _filters.Add(new ColorFilter(new FilterDelegate(singleFilter), colorRange, filterParams));
            return this;
        }

        /// <summary>
        /// Applies all filters to set of colors
        /// </summary>
        /// <param name="colors"></param>
        /// <param name="outputClamping"></param>
        /// <returns></returns>
        public IEnumerable<Color> ApplyTo(IEnumerable<Color> colors, bool outputClamping = true)
        {
            // Process all filters in chain
            bool parallel = true;
            int count = 0;
            foreach (var filter in _filters) {
                if (filter.GetDelegate().IsParallelMultiFilter()) {
                    parallel = true;
                }
                
                colors = filter.ApplyTo(colors, count < 2 ? 4 : 0);
                count++;
                parallel = filter.GetDelegate().IsMultiFilter();
                if (filter.GetDelegate().IsMultiFilter()) {
                    count = 0;
                }
                if (filter.GetDelegate().IsParallelMultiFilter()) {
                    count = 1;
                }
            }    

            // Final clamping after last filter in chain
            foreach (var color in colors) {
                if (outputClamping)
                    color.ClampExceedingColors();
                yield return color;
            }
        }

        public string ToString(string delimiter = "\n", string prefix = "   ")
        {
            var sb = new StringBuilder();

            for (var i = 0; i < _filters.Count; i++) {
                sb.Append(prefix + _filters[i]);
                if (i != _filters.Count - 1)
                    sb.Append(delimiter);
            }

            return sb.ToString();
        }
    }
}