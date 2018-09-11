using System;
using System.Collections.Generic;
using ColorSchemeInverter.Filters;
using NUnit.Framework;
namespace ColorSchemeInverter.UnitTests
{
    [TestFixture]
    public class LinearRangeTests
    {

        [Test]
        [TestCase(0.1, 0.2, 0.4, 0.6, 0.6, 0)]
        [TestCase(0.1, 0.3, 0.4, 0.6, 0.3, 1)]
        [TestCase(0.1, 0.3, 0.4, 0.6, 0.2, 0.5)]
        [TestCase(0.1, 0.3, 0.4, 0.6, 0.5, 0.5)]
        [TestCase(0.0, 0.4, 0.8, 1.0, 0.1, 0.25)]
        [TestCase(0.0, 0.4, 0.6, 1.0, 0.3, 0.75)]
        [TestCase(0.0, 0.4, 0.6, 1.0, 0.7, 0.25)]
        public void InRangeFactor_DirectPointsByPassingEnteringSlopes_ReturnsExpectedValues(double minStart, double minEnd, double maxStart,
            double maxEnd, double val, double expected)
        {
            LinearRange range = new LinearRange
            {
                MinStart = minStart, MinEnd = minEnd, MaxStart = maxStart, MaxEnd = maxEnd
            };

            double factor = range.InRangeFactor(val);
            Assert.That(factor, Is.EqualTo(expected).Within(0.001));
        }
    }
}