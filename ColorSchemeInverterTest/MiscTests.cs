using System;
using ColorSchemeInverter;
using ColorSchemeInverter.Colors;
using ColorSchemeInverter.Common;
using ColorSchemeInverter.Filters;
using NUnit.Framework;

namespace ColorSchemeInverter.UnitTests
{
    [TestFixture]
    public class MiscTests
    {
        [SetUp]
        public void Setup() { }

        [Test]
        public void TestRGB()
        {
            Rgb rgb1 = new Rgb8Bit(0x8A, 0x3B, 0x20).ToRgb();
            Rgb rgb2 = Rgb.FromRgbString("8A3B20FF", "RRGGBBAA");
            Assert.True(rgb1.Equals(rgb2));
        }

        [Test]
        public void TestInvertLightness()
        {
            Hsl hsl = new Hsl(180, 0.7, 0.8);
            Hsl result = FilterBundle.InvertLightness(hsl);
            Assert.That(result.Hue, Is.EqualTo(hsl.Hue).Within(0.00001));
            Assert.That(result.Saturation, Is.EqualTo(hsl.Saturation).Within(0.00001));
            Assert.That(result.Lightness, Is.EqualTo(0.2).Within(0.00001));
        }

        [Test]
        public void TestRGBtoHSLtoRGB()
        {
            Rgb rgb1 = new Rgb8Bit(0x8A, 0x3B, 0x20).ToRgb();
            Rgb rgb2 = rgb1.ToHsl().ToRgb();
            Assert.True(rgb1.Equals(rgb2));
        }

        [Test]
        public void TestManyRandomRGBtoHSLtoRGB()
        {
            for (int i = 0; i < 2000; i++) {
                var rnd = new Random();
                Rgb8Bit rgb = new Rgb8Bit(
                    (byte) rnd.Next(0, 255),
                    (byte) rnd.Next(0, 255),
                    (byte) rnd.Next(0, 255),
                    (byte) rnd.Next(0, 255));
                Rgb rgb1 = rgb.ToRgb();
                Rgb rgb2 = rgb1.ToHsl().ToRgb();
                Assert.True(rgb1.Equals(rgb1.ToHsl().ToRgb()));
                Assert.True(rgb.AboutEqual(rgb2.ToRgb8Bit())); // accepts difference of 1 for every 8bit component
            }
        }

        [Test]
        public void TestRGBToHSL()
        {
            // Todo: Implement TestRGBToHSL()
            Rgb rgb1 = new Rgb(new Rgb8Bit(0x8A, 0x3B, 0x20, 0xFF));
            Hsl hsl = rgb1.ToHsl();
            // Hue: 15.2830188679245, Saturation: 0.623529411764706, Lightness: 0.333333333333333 
            Assert.That(hsl.Hue, Is.EqualTo(15.2830188679245).Within(0.000001));
            Assert.That(hsl.Saturation, Is.EqualTo(0.623529411764706).Within(0.000001));
            Assert.That(hsl.Lightness, Is.EqualTo(0.333333333333333).Within(0.000001));
        }

        [Test]
        public void TestClamp()
        {
            double a = 2.0;
            Assert.True(a.Clamp(0,1.0) == 1.0);
            Assert.True(a.Min(0) == 0);
            Assert.True(a.Max(1.1) == 2.0);
   
        }
    }
}