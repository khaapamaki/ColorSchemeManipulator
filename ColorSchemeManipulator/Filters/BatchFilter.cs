using System;
using System.Collections.Generic;
using System.Text;
using ColorSchemeManipulator.Colors;

namespace ColorSchemeManipulator.Filters
{
    public class BatchFilter
    {
        private Func<IEnumerable<ColorBase>, object[], IEnumerable<ColorBase>> FilterDelegate { get; }
        private object[] Arguments { get; }

        public BatchFilter(Func<IEnumerable<ColorBase>, object[], IEnumerable<ColorBase>> filterDelegate,
            params object[] args)
        {
            FilterDelegate = filterDelegate;
            Arguments = args;
        }

        public IEnumerable<ColorBase> ApplyTo(IEnumerable<ColorBase> colors)
        {
            return FilterDelegate(colors, Arguments);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var argument in Arguments) {
                sb.Append(argument.ToString() + " ");
            }

            return FilterDelegate.Method.Name + " " + sb;
        }
    }
}