using ColorSchemeManipulator.CLI;
using ColorSchemeManipulator.Colors;
using ColorSchemeManipulator.Filters;
using NUnit.Framework;

namespace ColorSchemeManipulator.UnitTests
{
    [TestFixture]
    public class ColorRangeTests
    {
        [Test]
        public void InRange_MultipleScenarios() { }

        [Test]
        public void ParseRange_InRangeFactorWithHue355WithHueFrom0to360_Return1()
        {
            Hsl hsl = new Hsl(355, 0.5, 0.5);
            var range = new ColorRange();
            range.Hue(0, 360);

            double factor = range.InRangeFactor(hsl);

            Assert.That(factor, Is.EqualTo(1.0));
        }

        [Test]
        public void ParseRange_InRangeFactorWithHue355WithRangeFromMinus10to0_Return1()
        {
            Rgb rgb = new Rgb(1, 0, 0.1); // a bit blueish max saturated red
            Hsl hsl = new Hsl(355, 0.5, 0.5);
            var range = new ColorRange();
            range.Hue(-10, 0);

            double factor = range.InRangeFactor(hsl);
            Assert.That(factor, Is.EqualTo(1.0));
        }
        [Test]
        public void ParseRange_InRangeFactorWithHue5WithRangeFrom10to0_Return0()
        {
            Rgb rgb = new Rgb(1, 0, 0.1); // a bit blueish max saturated red
            Hsl hsl = new Hsl(5, 0.5, 0.5);
            var range = new ColorRange();
            range.Hue(10, 0);

            double factor = range.InRangeFactor(hsl);
            Assert.That(factor, Is.EqualTo(0.0));
        }
        [Test]
        public void ParseRange_InRangeFactorWithHue355WithRangeFrom10to360_Return0()
        {
            Hsl hsl = new Hsl(355, 0.5, 0.5);
            var range = new ColorRange();
            range.Hue(10, 360);

            double factor = range.InRangeFactor(hsl);
            Assert.That(factor, Is.EqualTo(1.0));
        }
        [Test]
        public void ParseRange_InRangeFactorWithHue355WithRangeFrom355to355_Return1()
        {
            Hsl hsl = new Hsl(355, 0.5, 0.5);
            var range = new ColorRange();
            range.Hue(355, 355);

            double factor = range.InRangeFactor(hsl);
            Assert.That(factor, Is.EqualTo(1.0));
        }

        [Test]
        public void ParseRange_InRangeFactorWithHue355WithRangeFrom356to360_Return0()
        {
            Hsl hsl = new Hsl(355, 0.5, 0.5);
            var range = new ColorRange();
            range.Hue(356, 360);

            double factor = range.InRangeFactor(hsl);
            Assert.That(factor, Is.EqualTo(0.0));
        }

        [Test]
        public void ParseRange_InRangeFactorWithHue5WithRangeFrom360to10_Return1()
        {
            Hsl hsl = new Hsl(5, 0.5, 0.5);
            var range = new ColorRange();
            range.Hue(360, 10);

            double factor = range.InRangeFactor(hsl);
            Assert.That(factor, Is.EqualTo(1.0));
        }
    }
}