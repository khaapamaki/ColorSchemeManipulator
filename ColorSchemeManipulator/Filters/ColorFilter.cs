using System;
using System.Collections.Generic;
using System.Text;
using ColorSchemeManipulator.Colors;

namespace ColorSchemeManipulator.Filters
{
    /// <summary>
    /// A class that encapsulates a filter delegate and it's parameters
    /// It also invokes the delegate via ApplyTo() method with a set of colors
    /// </summary>
    public class ColorFilter
    {
        private Func<IEnumerable<Color>, object[], IEnumerable<Color>> FilterDelegate { get; }
        private object[] Parameters { get; }

        public ColorFilter(Func<IEnumerable<Color>, object[], IEnumerable<Color>> filterDelegate,
            params object[] filterParams)
        {
            FilterDelegate = filterDelegate;
            Parameters = filterParams;
        }

        public IEnumerable<Color> ApplyTo(IEnumerable<Color> colors)
        {
            return FilterDelegate(colors, Parameters);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var argument in Parameters) {
                sb.Append(argument + " ");
            }

            return FilterDelegate.Method.Name + " " + sb;
        }
    }
}