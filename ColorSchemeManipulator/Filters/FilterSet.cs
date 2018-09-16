using System;
using System.Collections.Generic;
using System.Text;
using ColorSchemeManipulator.Colors;

namespace ColorSchemeManipulator.Filters
{
    public class FilterSet
    {
        private readonly List<ColorFilter> _filters = new List<ColorFilter>();

        public FilterSet() { }

        
        public FilterSet Add(ColorFilter filter)
        {
            _filters.Add(filter);
            return this;
        }

        public FilterSet Add(Func<IEnumerable<Color>, object[], IEnumerable<Color>> filterDelegate)
        {
            _filters.Add(new ColorFilter(filterDelegate));
            return this;
        }

        public FilterSet Add(Func<IEnumerable<Color>, object[], IEnumerable<Color>> filterDelegate, params object[] args)
        {
            _filters.Add(new ColorFilter(filterDelegate, args));
            return this;
        }
        
        public IEnumerable<Color> ApplyTo(IEnumerable<Color> colors)
        {
            foreach (var filter in _filters) {
                colors = ApplyFilter(colors, filter);
            }

            return colors;
        }

        private IEnumerable<Color> ApplyFilter(IEnumerable<Color> colors, ColorFilter filter)
        {
            return filter.ApplyTo(colors);
        }

        public string ToString(string delimiter = "\n", string prefix = "   ")
        {
            var sb = new StringBuilder();

            for (var i = 0; i < _filters.Count; i++) {
                sb.Append(prefix + _filters[i].ToString());
                if (i != _filters.Count - 1)
                    sb.Append(delimiter);
            }

            return sb.ToString();
        }
    }
}