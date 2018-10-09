using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColorSchemeManipulator.Colors;

namespace ColorSchemeManipulator.Filters
{
    public class FilterDelegate
    {
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
        
        public IEnumerable<Color> ApplyTo(IEnumerable<Color> colors, ColorRange colorRange, params double[] parameters)
        {
            if (MultiFilterDelegate != null) {
                return ApplyMultiFilter(colors, colorRange, parameters);
            } else if (SingleFilterDelegate != null) {
                return ApplySingleFilter(colors, colorRange, parameters);
            }
            throw new Exception("No filter defined");
        }

        private IEnumerable<Color> ApplyMultiFilter(IEnumerable<Color> colors, ColorRange colorRange, params double[] parameters)
        {
            return MultiFilterDelegate(colors, colorRange, parameters); 
        }
             
        private IEnumerable<Color> ApplySingleFilter(IEnumerable<Color> colors, ColorRange colorRange, params double[] parameters)
        {
            var result = colors
                .AsParallel()
                .AsOrdered()
                .WithDegreeOfParallelism(2)
                .Select(color => SingleFilterDelegate(color, colorRange, parameters));
            foreach (var color in result) {
                yield return color;
            }
        }

        public string FilterName()
        {
            return MultiFilterDelegate?.Method.Name ?? SingleFilterDelegate?.Method.Name;
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