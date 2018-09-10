using NUnit.Framework;
using ColorSchemeInverter.Filters;
using ColorSchemeInverter.CLI;
using ColorSchemeInverter.Common;
using ColorSchemeInverter.Colors;

namespace ColorSchemeInverter.UnitTests
{
    [TestFixture]
    public class ColorMathTests
    {
        [Test]
        [Ignore("Not implemented")]
        public void Test()
        {
            
        }
        
        [Test]
        public void Linear01_ValueAtMin_ReturnsRangeStart()
        {
            double result = ColorMath.LinearInterpolation(0, 100.0, 200.0);
            Assert.That(result == 100.0);
        }
        
        [Test]
        public void Linear01_ValueAtMax_ReturnsRangeEnd()
        {
            double result = ColorMath.LinearInterpolation(1, 100.0, 200.0);
            Assert.That(result == 200.0);
        }
        
        [Test]
        public void Linear01_ValueInBetween_ReturnsValueInBetween()
        {
            double result = ColorMath.LinearInterpolation(0.1, 100.0, 200.0);
            Assert.That(result == 110.0);
        }
        
        [Test]
        public void Linear01_ValueBelowMin_ReturnsRangeEnd()
        {
            double result = ColorMath.LinearInterpolation(-1, 100.0, 200.0);
            Assert.That(result == 100.0);
        }
        
        [Test]
        public void Linear01_ValueAboveMax_ReturnsRangeEnd()
        {
            double result = ColorMath.LinearInterpolation(1.1, 100.0, 200.0);
            Assert.That(result == 200.0);
        }

        
    }
}