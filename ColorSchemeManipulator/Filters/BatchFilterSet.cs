using System;
using System.Collections.Generic;
using System.Text;
using ColorSchemeManipulator.Colors;

namespace ColorSchemeManipulator.Filters
{
    public class BatchFilterSet
    {
        private readonly List<BatchFilter> _filters = new List<BatchFilter>();

        public BatchFilterSet() { }

        
        public BatchFilterSet Add(BatchFilter filter)
        {
            _filters.Add(filter);
            return this;
        }

        public BatchFilterSet Add(Func<IEnumerable<ColorBase>, object[], IEnumerable<ColorBase>> filterDelegate)
        {
            _filters.Add(new BatchFilter(filterDelegate));
            return this;
        }

        public BatchFilterSet Add(Func<IEnumerable<ColorBase>, object[], IEnumerable<ColorBase>> filterDelegate, params object[] args)
        {
            _filters.Add(new BatchFilter(filterDelegate, args));
            return this;
        }
        
        public IEnumerable<ColorBase> ApplyTo(IEnumerable<ColorBase> colors)
        {
            foreach (var filter in _filters) {
                colors = ApplyFilter(colors, filter);
            }

            return colors;
        }

        private IEnumerable<ColorBase> ApplyFilter(IEnumerable<ColorBase> colors, BatchFilter filter)
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