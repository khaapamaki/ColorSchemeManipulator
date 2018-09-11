using System;
using System.Text;
using ColorSchemeInverter.Common;

namespace ColorSchemeInverter.Filters
{
    public class LinearRange
    {
        public double MinStart { get; set; }
        public double MinEnd { get; set; }
        public double MaxStart { get; set; }
        public double MaxEnd { get; set; }

        private double _max = 0.0;
        private double _min = 0.0;
        private double _maxSlope = 0.0;
        private double _minSlope = 0.0;
        
        public LinearRange() { }

        public LinearRange(double min, double max)
        {
            _min = min;
            _max = max;
            _maxSlope = 0.0;
            _minSlope = 0.0;
            MinStart = min;
            MinEnd = min;
            MaxStart = max;
            MaxEnd = max;
        }
        
        public LinearRange(double min, double max, double minSlope = 0.0, double maxSlope = 0.0)
        {
            minSlope = minSlope.LimitHigh(Math.Abs(min - max));
            maxSlope = maxSlope.LimitHigh(Math.Abs(min - max));
            _min = min;
            _max = max;
            _maxSlope = maxSlope;
            _minSlope = minSlope;
            MinStart = min - minSlope / 2;
            MinEnd = min + minSlope / 2;
            MaxStart = max - maxSlope / 2;
            MaxEnd = max + maxSlope / 2;
        }
        
        public double InRangeFactor(double value)
        {
            // out of range
            if (value <= MinStart || value >= MaxEnd)
                return 0;
            // in full range
            if (value >= MinEnd && value <= MaxStart)
                return 1.0;
            // in min slope range
            if (value > MinStart && value < MinEnd)
                return (value - MinStart) / (MinEnd - MinStart);
            // in max slope range
            if (value > MaxStart && value < MaxEnd)
                return (value - MaxStart) / (MaxEnd - MaxStart);

            return 0;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (_minSlope == 0.0) {
                sb.Append(_min + "-");

            } else {
                sb.Append(_min + "/" + _minSlope + "-");
            }
            
            if (_maxSlope == 0.0) {
                sb.Append(_max );

            } else {
                sb.Append(_max + "/" + _maxSlope);
            }

            return sb.ToString();
        }
    }
}