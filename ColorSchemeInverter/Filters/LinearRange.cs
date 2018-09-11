using System;
using ColorSchemeInverter.Common;

namespace ColorSchemeInverter.Filters
{
    public class LinearRange
    {
        public double MinStart { get; set; }
        public double MinEnd { get; set; }
        public double MaxStart { get; set; }
        public double MaxEnd { get; set; }

        public LinearRange() { }

        public LinearRange(double min, double max)
        {
            MinStart = min;
            MinEnd = min;
            MaxStart = max;
            MaxEnd = max;
        }
        
        public LinearRange(double min, double max, double minSlope = 0.0, double maxSlope = 0.0)
        {
            minSlope = minSlope.LimitHigh(Math.Abs(min - max));
            maxSlope = maxSlope.LimitHigh(Math.Abs(min - max));
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

    }
}