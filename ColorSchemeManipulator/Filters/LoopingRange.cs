using System;
using System.Text;
using ColorSchemeManipulator.Common;

namespace ColorSchemeManipulator.Filters
{
    [Obsolete]
    public class LoopingRange
    {
        public double MinStart { get; set; }
        public double MinEnd { get; set; }
        public double MaxStart { get; set; }
        public double MaxEnd { get; set; }
        private readonly double _loopMax;

        private double _max = 0.0;
        private double _min = 0.0;
        private double _maxSlope = 0.0;
        private double _minSlope = 0.0;
        
        public LoopingRange(double loopMax = 360)
        {
            _loopMax = loopMax;
        }

        public LoopingRange(double min, double max, double loopMax = 360)
        {
            MinStart = min.NormalizeLoopingValue(loopMax);
            MinEnd = min.NormalizeLoopingValue(loopMax);
            MaxStart = max.NormalizeLoopingValue(loopMax);
            MaxEnd = max.NormalizeLoopingValue(loopMax);
            _loopMax = loopMax;
            _min = min;
            _max = max;
        }

        public LoopingRange(double min, double max, double minSlope = 0.0, double maxSlope = 0.0, double loopMax = 360)
        {
            minSlope = minSlope.LimitHigh(Math.Abs(min - max));
            maxSlope = maxSlope.LimitHigh(Math.Abs(min - max));
            min = min.NormalizeLoopingValue(loopMax);
            max = max.NormalizeLoopingValue(loopMax);
            _min = min;
            _max = max;
            _maxSlope = maxSlope;
            _minSlope = minSlope;
            MinStart = min - minSlope / 2;
            MinEnd = min + minSlope / 2;
            MaxStart = max - maxSlope / 2;
            MaxEnd = max + maxSlope / 2;
            _loopMax = loopMax;
        }

        public static LoopingRange Range(double min, double max, double minSlope = 0.0, double maxSlope = 0.0,
            double loopMax = 360)
        {
            return new LoopingRange(min, max, minSlope, maxSlope, loopMax);
        }
        
        public static LoopingRange FourPointRange(double minStart, double minEnd, double maxStart, double maxEnd,
            double loopMax = 360)
        {
            return new LoopingRange(loopMax)
                {MinStart = minStart, MinEnd = minEnd, MaxStart = maxStart, MaxEnd = maxEnd};
        }
        
        public double InRangeFactor(double value)
        {
            value = value.NormalizeLoopingValue(_loopMax);
            // out of range
            if (!IsInNormalizedRange(value, MinStart, MaxEnd))
                return 0;
            // in full range
            if (IsInNormalizedRange(value, MinEnd, MaxStart))
                return 1.0;
            // in min slope range
            if (IsInNormalizedRange(value, MinStart, MinEnd))
                return ShortestDifference(value, MinStart) / Math.Abs(MinStart - MinEnd);
            // in max slope range
            if (IsInNormalizedRange(value, MaxStart, MaxEnd))
                return ShortestDifference(value, MaxStart) / Math.Abs(MaxStart - MaxEnd);

            return 0;
        }

        private double ShortestDifference(double a, double b)
        {
            double d1 = Math.Abs(a - b);
            double d2 = _loopMax - d1;
            return d1.Min(d2);
        }

        private bool IsInNormalizedRange(double value, double min, double max)
        {
            min = min.NormalizeLoopingValue(_loopMax);
            max = max.NormalizeLoopingValue(_loopMax);
            if (min <= max) {
                return value >= min && value <= max;
            } else {
                return value >= min || value <= max;
            }
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