using System;
using System.Collections.Generic;

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

        public BatchFilterSet Add(Func<IFilterable, object[], IFilterable> filterDelegate)
        {
            _filters.Add(new BatchFilter(filterDelegate));
            return this;
        }

        public BatchFilterSet Add(Func<IFilterable, object[], IFilterable> filterDelegate, params object[] args)
        {
            _filters.Add(new BatchFilter(filterDelegate, args));
            return this;
        }
    }
}