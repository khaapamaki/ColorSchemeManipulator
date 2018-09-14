using System;
using System.Collections.Generic;
using ColorSchemeManipulator.Filters;
using NUnit.Framework;
namespace ColorSchemeManipulator.UnitTests
{
    [TestFixture]
    public class ParameterRangeTests
    {

        [Test]
        [TestCase(0.1, 0.2, 0.4, 0.6, 0.6, 0)]
        [TestCase(0.1, 0.3, 0.4, 0.6, 0.3, 1)]
        [TestCase(0.1, 0.3, 0.4, 0.6, 0.2, 0.5)]
        [TestCase(0.1, 0.3, 0.4, 0.6, 0.5, 0.5)]
        [TestCase(0.0, 0.4, 0.8, 1.0, 0.1, 0.25)]
        [TestCase(0.0, 0.4, 0.6, 1.0, 0.3, 0.75)]
        [TestCase(0.0, 0.4, 0.6, 1.0, 0.7, 0.25)]
        public void InRangeFactor_FourPointParameter_ReturnsExpectedValues(double minStart, double minEnd, double maxStart,
            double maxEnd, double val, double expected)
        {
            ParameterRange range = new ParameterRange
            {
                MinStart = minStart, MinEnd = minEnd, MaxStart = maxStart, MaxEnd = maxEnd
            };

            double factor = range.InRangeFactor(val);
            Assert.That(factor, Is.EqualTo(expected).Within(0.001));
        }
        
        
        [Test]
        [TestCase(10, 240, 40, 40, -20, 0)]
        [TestCase(60, 240, 40, 40, 0, 0)]
        [TestCase(60, 240, 40, 40, 60, 0.5)]
        [TestCase(60, 240, 40, 40, 70, 0.75)]
        [TestCase(10, 240, 40, 40, 40, 1)]
        [TestCase(10, 240, 40, 40, 10, 0.5)]
        [TestCase(10, 240, 40, 40, 0, 0.25)]
        [TestCase(340, 60, 40, 40, 0, 1)]
        [TestCase(340, 100, 80, 40, 10, 0.875)]
        [TestCase(340, 60, 40, 40,-10, 0.75)]
        [TestCase(350, 60, 40, 40, 0, 0.75)]
        public void InRangeFactor_LoopingRangeWithSlopeEntries_ReturnsExpectedValues(double min, double max, double minSlope, double maxSlope,
            double val, double expFactor)
        {
            ParameterRange range =ParameterRange.Range(min, max, minSlope, maxSlope, 360);
            
            double factor = range.InRangeFactor(val);
            Assert.That(factor, Is.EqualTo(expFactor).Within(0.001));
        }
    }
}