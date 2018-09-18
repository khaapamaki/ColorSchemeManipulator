using System;
using System.Globalization;
using ColorSchemeManipulator.Colors;
using NUnit.Framework;

namespace ColorSchemeManipulator.UnitTests
{
    [TestFixture]
    public class HdrColorTests
    {
        [Test]
        public void Test()
        {
            var sourceColor = HexRgb.FromRgbString("2b6ca2", "rrggbb");
            var color = new Color(sourceColor);
            color.Saturation *= 2;
            Console.WriteLine(color.ToString());
            color.Saturation /= 2;
            Console.WriteLine(color.ToString());
            Assert.True(color.Equals(sourceColor));
           
        }
        
        [Test]
        public void Test2()
        {
            var sourceColor = HexRgb.FromRgbString("2b6ca2", "rrggbb");
            var color = new Color(sourceColor);
            color.Saturation *= 2;
            Console.WriteLine(color.ToString());
            color.ClampExceedingColors();
            Console.WriteLine(color.ToString());
            Assert.That(color.Hue, Is.EqualTo(sourceColor.Hue).Within(0.000001));
           
        }
    }
}