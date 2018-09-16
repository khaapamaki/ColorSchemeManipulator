using System;
using System.Collections.Generic;
using System.Text;
using ColorSchemeManipulator.Colors;

namespace ColorSchemeManipulator.Filters
{
    public class ColorFilter
    {
        private Func<IEnumerable<Color>, object[], IEnumerable<Color>> FilterDelegate { get; }
        private object[] Arguments { get; }

        public ColorFilter(Func<IEnumerable<Color>, object[], IEnumerable<Color>> filterDelegate,
            params object[] args)
        {
            FilterDelegate = filterDelegate;
            Arguments = args;
        }

        public IEnumerable<Color> ApplyTo(IEnumerable<Color> colors)
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