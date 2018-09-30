using System;
using ColorSchemeManipulator.Common;

namespace ColorSchemeManipulator.Filters
{
    public class ParameterRange
    {
        public double MinStart { get; set; }
        public double MinEnd { get; set; }
        public double MaxStart { get; set; }
        public double MaxEnd { get; set; }

        public bool IsLoopingRange { get; private set; } = false;

        private double _loopMax;

        public double LoopMax
        {
            get => _loopMax;
            set
            {
                _loopMax = value;
                IsLoopingRange = true;
            }
        }

        public ParameterRange(double? loopMax = null)
        {
            if (loopMax != null)
                LoopMax = (double) loopMax;
        }

        public ParameterRange(double min, double max, double? loopMax = null)
        {
            MinStart = min;
            MinEnd = min;
            MaxStart = max;
            MaxEnd = max;
            if (loopMax != null)
                LoopMax = (double) loopMax;
        }

        public ParameterRange(double min, double max, double minSlope = 0.0, double maxSlope = 0.0,
            double? loopMax = null)
        {
            if (loopMax != null)
                LoopMax = (double) loopMax;

            minSlope = minSlope.LimitHigh(Math.Abs(min - max) * 2);
            maxSlope = maxSlope.LimitHigh(Math.Abs(min - max) * 2);
            if (IsLoopingRange) {
                min = min.NormalizeLoopingValue(LoopMax);
                max = max.NormalizeLoopingValue(LoopMax);
            }

            MinStart = min - minSlope / 2;
            MinEnd = min + minSlope / 2;
            MaxStart = max - maxSlope / 2;
            MaxEnd = max + maxSlope / 2;
            
            // check for overlapping slopes
            if (minSlope / 2 + maxSlope / 2 > Math.Abs(min-max)) {
                MinEnd = (MinEnd + MaxStart) / 2;
                MaxStart = MinEnd;
            }
        }

        public static ParameterRange Range(double min, double max, double minSlope = 0.0, double maxSlope = 0.0,
            double? loopMax = null)
        {
            return new ParameterRange(min, max, minSlope, maxSlope, loopMax);
        }

        public static ParameterRange FourPointRange(double minStart, double minEnd, double maxStart, double maxEnd,
            double? loopMax = null)
        {
            ParameterRange range = new ParameterRange(loopMax)
                {MinStart = minStart, MinEnd = minEnd, MaxStart = maxStart, MaxEnd = maxEnd};

            return range;
        }

        public double InRangeFactor(double value)
        {
            return IsLoopingRange ? InLoopingRangeFactor(value) : InLinearRangeFactor(value);
        }

        private double InLinearRangeFactor(double value)
        {
            // out of range
            if (value < MinStart || value > MaxEnd)
                return 0;
            // in full range
            if (value >= MinEnd && value <= MaxStart)
                return 1.0;
            // in min slope range
            if (value >= MinStart && value < MinEnd)
                return (value - MinStart) / (MinEnd - MinStart);
            // in max slope range
            if (value > MaxStart && value <= MaxEnd)
                return (MaxEnd - value) / (MaxEnd - MaxStart);

            return 0;
        }

        private double InLoopingRangeFactor(double value)
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

        public override string ToString()
        {
            return $"{MinStart}/{MinEnd}--{MaxStart}\\{MaxEnd}";
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

        public ParameterRange Copy()
        {
            ParameterRange range = new ParameterRange();
            range.MinStart = MinStart;
            range.MinEnd = MinEnd;
            range.MaxStart = MaxStart;
            range.MaxEnd = MaxEnd;
            range._loopMax = _loopMax;
            range.IsLoopingRange = IsLoopingRange;
            return range;
        }
    }
}