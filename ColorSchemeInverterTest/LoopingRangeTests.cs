using System;
using System.Collections.Generic;
using ColorSchemeInverter.Filters;
using NUnit.Framework;

namespace ColorSchemeInverter.UnitTests
{
    [Obsolete]
    public class LoopingRangeTests
    {
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
        public void InRangeFactor_MinMaxWithSlopes_ReturnsExpected(double min, double max, double minSlope, double maxSlope,
            double val, double expFactor)
        {
            LoopingRange range = new LoopingRange(min, max, minSlope, maxSlope, 360);
            double factor = range.InRangeFactor(val);
            Assert.That(factor, Is.EqualTo(expFactor).Within(0.001));
        }
    }
}