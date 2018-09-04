using System;
using System.Collections.Generic;
using System.Linq;

namespace ColorSchemeInverter
{
    public class HSLFilterSet
    {
        private readonly List<HSLFilter> _filterChain = new List<HSLFilter>();

        public HSLFilterSet() { }
        

        public HSLFilterSet(Func<HSL, object[], HSL> filter)
        {
            _filterChain.Add(new HSLFilter(filter));
        }

        public void Add(HSLFilter filter)
        {
            _filterChain.Add(filter);
        }
        
        public HSLFilterSet Add(Func<HSL, object[], HSL> filter)
        {
            _filterChain.Add(new HSLFilter(filter));
            return this;
        }
        
        public HSLFilterSet Add(Func<HSL, object[], HSL> filter, object arg)
        {
            _filterChain.Add(new HSLFilter(filter, arg));
            return this;
        }
        
        public HSLFilterSet Add(Func<HSL, object[], HSL> filter, object[] args)
        {
            _filterChain.Add(new HSLFilter(filter, args));
            return this;
        }
        
        public bool Any()
        {
            return _filterChain.Any();
        }

        public HSL ApplyTo(HSL hsl)
        {
            HSL result  = new HSL(hsl);
            _filterChain.ForEach(f => result = f.ApplyTo(result));    
            return result;
        }
    }
}