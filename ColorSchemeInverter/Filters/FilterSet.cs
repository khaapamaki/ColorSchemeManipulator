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


        public FilterSet(Func<HSL, object[], HSL> filterDelegate)
        {
            _filterChain.Add(new HSLFilter(filterDelegate));
        }

        public void Add(HSLFilter filter)
        {
            _filterChain.Add(filter);
        }

        public FilterSet Add(Func<HSL, object[], HSL> filterDelegate)
        {
            _filterChain.Add(new HSLFilter(filterDelegate));
            return this;
        }

        public FilterSet Add(Func<HSL, object[], HSL> filterDelegate, params object[] args)
        {
            _filterChain.Add(new HSLFilter(filterDelegate, args));
            return this;
        }

        public FilterSet(Func<RGB, object[], RGB> filterDelegate)
        {
            _filterChain.Add(new RGBFilter(filterDelegate));
        }

        public void Add(RGBFilter filter)
        {
            _filterChain.Add(filter);
        }

        public FilterSet Add(Func<RGB, object[], RGB> filterDelegate)
        {
            _filterChain.Add(new RGBFilter(filterDelegate));
            return this;
        }

        public FilterSet Add(Func<RGB, object[], RGB> filterDelegate, params object[] args)
        {
            _filterChain.Add(new RGBFilter(filterDelegate, args));
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

        private ColorBase ApplyAnyColorTo(ColorBase colorBase)
        {
            foreach (var filter in _filterChain) {
                colorBase = ApplyFilter(colorBase, filter);
            }

            return colorBase;
        }

        private ColorBase ApplyFilter(ColorBase colorBase, ColorFilter filter)
        {
            return filter.ApplyTo(colorBase);
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