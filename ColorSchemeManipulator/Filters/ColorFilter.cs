using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColorSchemeManipulator.Colors;

namespace ColorSchemeManipulator.Filters
{
    public class ColorFilter
    {
        private Func<IEnumerable<Color>, ColorRange, double[], IEnumerable<Color>> MultiFilterDelegate { get; }
        private Func<Color, ColorRange, double[], Color> SingleFilterDelegate { get; }

        private ColorFilter() { }
        
        public ColorFilter(Func<IEnumerable<Color>, ColorRange, double[], IEnumerable<Color>> multiFilterDelegate)
        {
            MultiFilterDelegate = multiFilterDelegate;
        }
        
        public ColorFilter(Func<Color, ColorRange, double[], Color> singleFilterDelegate)
        {
            SingleFilterDelegate = singleFilterDelegate;
        }
        
        public IEnumerable<Color> ApplyTo(IEnumerable<Color> colors, ColorRange colorRange, params double[] parameters)
        {
            if (MultiFilterDelegate != null) {
                return ApplyMultiFilter(colors, colorRange, parameters);
            } else {
                return ApplySingleFilter(colors, colorRange, parameters);
            }
        }

        private IEnumerable<Color> ApplyMultiFilter(IEnumerable<Color> colors, ColorRange colorRange, params double[] parameters)
        {
            return MultiFilterDelegate(colors, colorRange, parameters); 
        }
             
        private IEnumerable<Color> ApplySingleFilter(IEnumerable<Color> colors, ColorRange colorRange, params double[] parameters)
        {
            var result = colors.Select(color => SingleFilterDelegate(color, colorRange, parameters));
            foreach (var color in result) {
                yield return color;
            }
        }

        public string GetName()
        {
            return MultiFilterDelegate?.Method.Name ?? SingleFilterDelegate?.Method.Name;
        }
        
        public string ToString(ColorRange colorRange, params double[] parameters)
        {
            var sb = new StringBuilder();
            foreach (var argument in parameters) {
                sb.Append((sb.Length > 0 ? ", " : "") + argument);
            };
            
            return GetName()+ (sb.Length > 0 ? $"({sb})" : "") + (colorRange != null ? " ==> " + parameters  : "");
        }
    }
}