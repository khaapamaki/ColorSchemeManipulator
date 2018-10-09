using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using ColorSchemeManipulator.Colors;
using ColorSchemeManipulator.Ranges;

namespace ColorSchemeManipulator.Filters
{
    public class FilterDelegate
    {
        private const int DegreeOfParallelism = 2;
        
        private Func<IEnumerable<Color>, ColorRange, double[], IEnumerable<Color>> MultiFilterDelegate { get; }
        private Func<Color, ColorRange, double[], Color> SingleFilterDelegate { get; }

        private FilterDelegate() { }
        
        public FilterDelegate(Func<IEnumerable<Color>, ColorRange, double[], IEnumerable<Color>> multiFilterDelegate)
        {
            MultiFilterDelegate = multiFilterDelegate;
        }
        
        public FilterDelegate(Func<Color, ColorRange, double[], Color> singleFilterDelegate)
        {
            SingleFilterDelegate = singleFilterDelegate;
        }
        
        public IEnumerable<Color> ApplyTo(IEnumerable<Color> colors, ColorRange colorRange, double[] parameters)
        {
            if (IsMultiFilter()) {
                return ApplyMultiFilter(colors, colorRange, parameters);
            } else if (IsSingleFilter()) {
                return ApplySingleFilter(colors, colorRange, parameters);
            }
            throw new Exception("No filter defined");
        }

        private IEnumerable<Color> ApplyMultiFilter(IEnumerable<Color> colors, ColorRange colorRange, double[] parameters)
        {
            return MultiFilterDelegate(colors, colorRange, parameters); 
        }
        
        private IEnumerable<Color> ApplySingleFilter(IEnumerable<Color> colors, ColorRange colorRange, double[] parameters)
        {
            IEnumerable<Color> result;
            if (DegreeOfParallelism > 0) {
                result = colors
                    .AsParallel()
                    .AsOrdered()
                    .WithDegreeOfParallelism(DegreeOfParallelism)
                    .Select(color => SingleFilterDelegate(color, colorRange, parameters));
            } else {
                result = colors
                    .Select(color => SingleFilterDelegate(color, colorRange, parameters));
            }
            foreach (var color in result) {
                yield return color;
            }
        }

        public bool IsMultiFilter()
        {
            return MultiFilterDelegate != null;
        }
        
        public bool IsSingleFilter()
        {
            return SingleFilterDelegate != null;
        }
        
        public string FilterName()
        {
            if (IsMultiFilter())
                return MultiFilterDelegate.Method.Name;
            if (IsSingleFilter())
                return SingleFilterDelegate?.Method.Name;
            return "<No filter set>";
        }
        
        public string ToString(ColorRange colorRange, params double[] parameters)
        {
            var sb = new StringBuilder();
            foreach (var argument in parameters) {
                sb.Append((sb.Length > 0 ? ", " : "") + argument);
            }
            
            return FilterName()+ (sb.Length > 0 ? $"({sb})" : "") + (colorRange != null ? " ==> " + colorRange  : "");
        }
    }
}