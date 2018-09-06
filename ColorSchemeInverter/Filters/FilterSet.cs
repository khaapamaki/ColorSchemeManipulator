using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColorSchemeInverter.Colors;

namespace ColorSchemeInverter.Filters
{
    public class FilterSet
    {
        private readonly List<ColorFilter> _filterChain = new List<ColorFilter>();

        public FilterSet() { }
        

        public FilterSet(Func<HSL, object[], HSL> filter)
        {
            _filterChain.Add(new HSLFilter(filter));
        }

        public void Add(HSLFilter filter)
        {
            _filterChain.Add(filter);
        }
        
        public FilterSet Add(Func<HSL, object[], HSL> filter)
        {
            _filterChain.Add(new HSLFilter(filter));
            return this;
        }
        
        public FilterSet Add(Func<HSL, object[], HSL> filter, object arg)
        {
            _filterChain.Add(new HSLFilter(filter, arg));
            return this;
        }
        
        public FilterSet Add(Func<HSL, object[], HSL> filter, object[] args)
        {
            _filterChain.Add(new HSLFilter(filter, args));
            return this;
        }
        
        public FilterSet(Func<RGB, object[], RGB> filter)
        {
            _filterChain.Add(new RGBFilter(filter));
        }

        public void Add(RGBFilter filter)
        {
            _filterChain.Add(filter);
        }
        
        public FilterSet Add(Func<RGB, object[], RGB> filter)
        {
            _filterChain.Add(new RGBFilter(filter));
            return this;
        }
        
        public FilterSet Add(Func<RGB, object[], RGB> filter, object arg)
        {
            _filterChain.Add(new RGBFilter(filter, arg));
            return this;
        }
        
        public FilterSet Add(Func<RGB, object[], RGB> filter, object[] args)
        {
            _filterChain.Add(new RGBFilter(filter, args));
            return this;
        }
        

        public bool Any()
        {
            return _filterChain.Any();
        }


        public HSL ApplyTo(HSL hsl)
        {
            return ApplyAnyColorTo(hsl).ToHSL();
        }
        
        public RGB ApplyTo(RGB rgb)
        {
            return ApplyAnyColorTo(rgb).ToRGB();
        }
        
        private Color ApplyAnyColorTo(Color color)
        {
            foreach (var filter in _filterChain) {
                color = ApplyFilter(color, filter);
            }
            return color;
        }
        
        private Color ApplyFilter(Color color, ColorFilter filter)
        {
            return filter.ApplyTo(color);
        }

        public string ToString(string delimiter = "\n", string prefix = "   ")
        {
            StringBuilder sb = new StringBuilder();
            
            for (var i = 0; i < _filterChain.Count; i++) {
                sb.Append(prefix + _filterChain[i].ToString());
                if (i != _filterChain.Count - 1)
                    sb.Append(delimiter);
            }
            return sb.ToString();
        }

    }
}