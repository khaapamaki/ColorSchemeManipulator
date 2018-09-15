using System;
using System.Collections.Generic;
using ColorSchemeManipulator.Colors;

namespace ColorSchemeManipulator.Filters
{
    public class BatchFilter
    {
        private Func<IEnumerable<ColorBase>, object[], IEnumerable<ColorBase>> FilterDelegate { get; }
        private object[] Arguments { get; }

        public BatchFilter(Func<IEnumerable<ColorBase>, object[], IEnumerable<ColorBase>> filterDelegate, params object[] args)
        {
            FilterDelegate = filterDelegate;
            Arguments = args;
        }
    }
}