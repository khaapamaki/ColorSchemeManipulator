using System;
using ColorSchemeInverter.Common;

namespace ColorSchemeInverter.Filters
{
    public class LoopingRange
    {
        public double MinStart { get; set; }
        public double MinEnd { get; set; }
        public double MaxStart { get; set; }
        public double MaxEnd { get; set; }
        private double _loopMax;
        
        public LoopingRange(double min, double max, double loopMax = 360)
        {
            MinStart = min;
            MinEnd = min;
            MaxStart = max;
            MaxEnd = max;
            _loopMax = loopMax;
        }
        
        public LoopingRange(double min, double max, double minSlope = 0.0, double maxSlope = 0.0, double loopMax = 360)
        {
            minSlope = minSlope.LimitHigh(Math.Abs(min - max));
            maxSlope = maxSlope.LimitHigh(Math.Abs(min - max));
            MinStart = min - minSlope / 2;
            MinEnd = min + minSlope / 2;
            MaxStart = max - maxSlope / 2;
            MaxEnd = max + maxSlope / 2;
            _loopMax = loopMax;
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

//        private double InHueRange(double hue)
//        {
//            if (_minHue <= _maxHue) {
//                return hue >= _minHue && hue <= _maxHue ? 1.0 : 0.0;
//            } else {
//                return hue >= _minHue || hue <= _maxHue ? 1.0 : 0.0;
//            }
//        }
        
    }
}