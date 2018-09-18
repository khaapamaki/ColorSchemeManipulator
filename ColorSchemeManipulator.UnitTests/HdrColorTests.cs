using System;
using System.Globalization;
using ColorSchemeManipulator.Colors;
using ColorSchemeManipulator.Common;
using NUnit.Framework;

namespace ColorSchemeManipulator.UnitTests
{
    [TestFixture]
    public class HdrColorTests
    {
        
        // Todo test CopyFrom method in ClampedColor and Color
        
        [Test]
        public void Color_MultiplySaturationBy2ToOverSaturateThenDivideBy2_ReturnsOriginalColor()
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
        public void ClampedColor_MultiplySaturationBy2ToOverSaturateThenDivideBy2_ReturnsOriginalColor()
        {
            var sourceColor = HexRgb.FromRgbString("2b6ca2", "rrggbb");
            var color = new ClampedColor(sourceColor);
            color.Saturation *= 2;
            Console.WriteLine(color.ToString());
            color.Saturation /= 2;
            Console.WriteLine(color.ToString());
            Assert.That(color.Saturation, Is.EqualTo(0.5).Within(0.0000001));    
        }
        
        [Test]
        public void Color_ClampingManuallyOverSaturatedColor_ReturnsUnchangedHue()
        {
            var sourceColor = HexRgb.FromRgbString("2b6ca2", "rrggbb");
            var color = new Color(sourceColor);
            color.Saturation *= 2;
            Console.WriteLine(color.ToString());
            color.ClampExceedingColors();
            Console.WriteLine(color.ToString());
            Assert.That(color.Hue, Is.EqualTo(sourceColor.Hue).Within(0.000001));
        }
        
        [Test]
        [TestCase(1,0,0)]
        [TestCase(1.2,0,0)]
        [TestCase(2,0,0)]
        [TestCase(2,1,1)]
        [TestCase(2,2,1)]
        [TestCase(2,2,2)]
        [TestCase(1,2,2)]
        [TestCase(0.1,2,-0.2)]
        public void Color_ClampingManuallySetRgbBValue_ReturnsUnchangedHue(double r, double b, double g)
        {
            var sourceColor = Color.FromRgb(r, g, b);
            var color = new Color(sourceColor);
            double hue = sourceColor.Hue;
            color.ClampExceedingColors();
            Console.WriteLine(color.ToString());
            Assert.That(color.Hue, Is.EqualTo(sourceColor.Hue).Within(0.000001));
           
        }
    }
}