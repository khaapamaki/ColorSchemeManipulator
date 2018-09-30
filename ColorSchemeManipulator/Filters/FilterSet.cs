using System;
using System.Collections.Generic;
using System.Text;
using ColorSchemeManipulator.Colors;

namespace ColorSchemeManipulator.Filters
{
    /// <summary>
    /// A collection class for storing multiple ColoFilters that
    /// are meant to be processed sequentially
    /// </summary>
    public class FilterSet
    {
        private readonly List<ColorFilter> _filters = new List<ColorFilter>();

        public FilterSet() { }


        public FilterSet Add(ColorFilter filter)
        {
            _filters.Add(filter);
            return this;
        }

        public FilterSet Add(Func<IEnumerable<Color>, ColorRange, double[], IEnumerable<Color>> filterDelegate)
        {
            _filters.Add(new ColorFilter(filterDelegate));
            return this;
        }

        public FilterSet Add(Func<IEnumerable<Color>, ColorRange, double[], IEnumerable<Color>> filterDelegate,
            ColorRange colorRange,
            params double[] filterParams)
        {
            _filters.Add(new ColorFilter(filterDelegate, colorRange, filterParams));
            return this;
        }

        public IEnumerable<Color> ApplyTo(IEnumerable<Color> colors, bool outputClamping = true)
        {
            // Process all filters in chain
            foreach (var filterDelegate in _filters) {
                colors = filterDelegate.ApplyTo(colors);
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