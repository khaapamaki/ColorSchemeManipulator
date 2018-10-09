using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColorSchemeManipulator.Colors;

namespace ColorSchemeManipulator.Filters
{
    public class FilterWrapper
    {
        private Func<IEnumerable<Color>, ColorRange, double[], IEnumerable<Color>> MultiFilter { get; }
        private Func<Color, ColorRange, double[], Color> SingleFilter { get; }

        private FilterWrapper() { }
        
        public FilterWrapper(Func<IEnumerable<Color>, ColorRange, double[], IEnumerable<Color>> multiFilter)
        {
            MultiFilter = multiFilter;
        }
        
        public FilterWrapper(Func<Color, ColorRange, double[], Color> singleFilter)
        {
            SingleFilter = singleFilter;
        }
        
        public IEnumerable<Color> ApplyTo(IEnumerable<Color> colors, ColorRange colorRange, params double[] parameters)
        {
            if (MultiFilter != null) {
                return ApplyMultiFilter(colors, colorRange, parameters);
            } else {
                return ApplySingleFilter(colors, colorRange, parameters);
            }
        }

        private IEnumerable<Color> ApplyMultiFilter(IEnumerable<Color> colors, ColorRange colorRange, params double[] parameters)
        {
            return MultiFilter(colors, colorRange, parameters); 
        }
             
        private IEnumerable<Color> ApplySingleFilter(IEnumerable<Color> colors, ColorRange colorRange, params double[] parameters)
        {
            var result = colors.Select(color => SingleFilter(color, colorRange, parameters));
            foreach (var color in result) {
                yield return color;
            }
        }

        public string GetName()
        {
            return MultiFilter?.Method.Name ?? SingleFilter?.Method.Name;
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