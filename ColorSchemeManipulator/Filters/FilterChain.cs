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
        
        public FilterChain Add(Func<IEnumerable<Color>, ColorRange, double[], IEnumerable<Color>> multiFilter,
            ColorRange colorRange,
            params double[] filterParams)
        {
            _filters.Add(new ColorFilter(new FilterDelegate(multiFilter), colorRange, filterParams));
            return this;
        }
        
        public FilterChain Add(Func<Color, ColorRange, double[],Color> singleFilter,
            ColorRange colorRange,
            params double[] filterParams)
        {
            _filters.Add(new ColorFilter(new FilterDelegate(singleFilter), colorRange, filterParams));
            return this;
        }

        public IEnumerable<Color> ApplyTo(IEnumerable<Color> colors, bool outputClamping = true)
        {
            // Process all filters in chain
            foreach (var filter in _filters) {               
                colors = filter.ApplyTo(colors);
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