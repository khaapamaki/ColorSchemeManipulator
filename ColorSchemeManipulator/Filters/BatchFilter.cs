using System;
using ColorSchemeManipulator.Colors;

namespace ColorSchemeManipulator.Filters
{
    public class BatchFilter
    {
        private Func<IFilterable, object[], IFilterable> FilterDelegate { get; }
        private object[] Arguments { get; }

        public BatchFilter(Func<IFilterable, object[], IFilterable> filterDelegate, params object[] args)
        {
            FilterDelegate = filterDelegate;
            Arguments = args;
        }
    }
}