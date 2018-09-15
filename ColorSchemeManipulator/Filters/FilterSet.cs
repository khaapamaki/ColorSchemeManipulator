using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColorSchemeManipulator.Colors;

namespace ColorSchemeManipulator.Filters
{
    [Obsolete]
    public class FilterSet
    {
        private readonly List<ColorFilter> _filterChain = new List<ColorFilter>();

        public FilterSet() { }

        public FilterSet(Func<Hsl, object[], Hsl> filterDelegate)
        {
            _filterChain.Add(new HslFilter(filterDelegate));
        }

        public void Add(HslFilter filter)
        {
            _filterChain.Add(filter);
        }

        public FilterSet Add(Func<Hsl, object[], Hsl> filterDelegate)
        {
            _filterChain.Add(new HslFilter(filterDelegate));
            return this;
        }

        public FilterSet Add(Func<Hsl, object[], Hsl> filterDelegate, params object[] args)
        {
            _filterChain.Add(new HslFilter(filterDelegate, args));
            return this;
        }

        public FilterSet(Func<Rgb, object[], Rgb> filterDelegate)
        {
            _filterChain.Add(new RgbFilter(filterDelegate));
        }

        public void Add(RgbFilter filter)
        {
            _filterChain.Add(filter);
        }

        public FilterSet Add(Func<Rgb, object[], Rgb> filterDelegate)
        {
            _filterChain.Add(new RgbFilter(filterDelegate));
            return this;
        }

        public FilterSet Add(Func<Rgb, object[], Rgb> filterDelegate, params object[] args)
        {
            _filterChain.Add(new RgbFilter(filterDelegate, args));
            return this;
        }

        public FilterSet(Func<Hsv, object[], Hsv> filterDelegate)
        {
            _filterChain.Add(new HsvFilter(filterDelegate));
        }

        public void Add(HsvFilter filter)
        {
            _filterChain.Add(filter);
        }

        public FilterSet Add(Func<Hsv, object[], Hsv> filterDelegate)
        {
            _filterChain.Add(new HsvFilter(filterDelegate));
            return this;
        }

        public FilterSet Add(Func<Hsv, object[], Hsv> filterDelegate, params object[] args)
        {
            _filterChain.Add(new HsvFilter(filterDelegate, args));
            return this;
        }

        public bool Any()
        {
            return _filterChain.Any();
        }

        public Hsl ApplyTo(Hsl hsl)
        {
            return ApplyAnyColorTo(hsl).ToHsl();
        }

        public Rgb ApplyTo(Rgb rgb)
        {
            return ApplyAnyColorTo(rgb).ToRgb();
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
            var sb = new StringBuilder();

            for (var i = 0; i < _filterChain.Count; i++) {
                sb.Append(prefix + _filterChain[i].ToString());
                if (i != _filterChain.Count - 1)
                    sb.Append(delimiter);
            }

            return sb.ToString();
        }
    }
}