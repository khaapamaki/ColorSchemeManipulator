using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ColorSchemeManipulator.Colors;

namespace ColorSchemeManipulator.Filters
{
    [Obsolete]
    public class ColorSet : IFilterable
    {
        private List<ColorBase> _items;


        public ColorSet()
        {
            _items = new List<ColorBase>();
        }
        
        public ColorSet(int capacity)
        {
            _items = new List<ColorBase>(capacity);
        }

        public IEnumerable<ColorBase> GetColors()
        {
            foreach (var color in _items) {
                yield return color;
            }
        }

        public void Add(ColorBase color)
        {
            _items.Add(color);
        }
    }
}