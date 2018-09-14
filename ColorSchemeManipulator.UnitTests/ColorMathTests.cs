using NUnit.Framework;
using ColorSchemeManipulator.Filters;
using ColorSchemeManipulator.CLI;
using ColorSchemeManipulator.Common;
using ColorSchemeManipulator.Colors;

namespace ColorSchemeManipulator.UnitTests
{
    [TestFixture]
    public class ColorMathTests
    {
        [Test]
        [Ignore("Not implemented")]
        public void Test() { }

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

        [Test]
        [TestCase(40, 340, 0.5, 10)]
        [TestCase(340, 40, 0.5, 10)]
        [TestCase(10, 20, 0.5, 15)]
        [TestCase(20, 10, 0.5, 15)]
        [TestCase(20, 320, 0.5, 350)]
        [TestCase(0, 360, 0.5, 0)]
        [TestCase(360, 0, 0.5, 0)]
        [TestCase(90, 270, 0.1, null)]
        public void LinearLoopingValues(double a, double b, double x, double? exp)
        {
            double? mid = ColorMath.LinearInterpolationForLoopingValues(x, a, b, 360);
            Assert.That(mid, Is.EqualTo(exp));
            
        }
    }
}