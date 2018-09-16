using System;
using ColorSchemeManipulator.Colors;
using ColorSchemeManipulator.Common;
using ColorSchemeManipulator.Filters;
using System.Collections;
using NUnit.Framework;

namespace ColorSchemeManipulator.UnitTests
{
    [TestFixture]
    public class MiscTests
    {
        [SetUp]
        public void Setup() { }
/*
        [Test]
        public void TestRGB()
        {
            Color rgb1 = new Rgb8Bit(0x8A, 0x3B, 0x20).ToRgb();
            Color rgb2 = Rgb.FromRgbString("8A3B20FF", "RRGGBBAA");
            Assert.True(rgb1.Equals(rgb2));
        }

        [Test]
        public void TestInvertLightness()
        {
            Color hsl = Color.FromHsl(180, 0.7, 0.8);
            List list = new List();
            List result = FilterBundle.InvertLightness(hsl);
            Assert.That(result.Hue, Is.EqualTo(hsl.Hue).Within(0.00001));
            Assert.That(result.Saturation, Is.EqualTo(hsl.Saturation).Within(0.00001));
            Assert.That(result.Lightness, Is.EqualTo(0.2).Within(0.00001));
        }

  */             
          
        [Test]
        public void TestRGBtoHSLtoRGB()
        {
            Color rgb1 = Color.FromRgb(0x8A, 0x3B, 0x20);
            Color rgb2 = Color.FromHsl(rgb1.Hue, rgb1.Saturation, rgb1.Lightness);
            Assert.True(rgb1.Equals(rgb2));
        }

        [Test]
        public void TestRGBtoHSVtoRGB()
        {
            Color rgb1 = Color.FromRgb(0x8A, 0x3B, 0x20);
            Color rgb2 = Color.FromHsv(rgb1.Hue, rgb1.SaturationHsv, rgb1.Value);
            Assert.True(rgb1.Equals(rgb2));
        }
        
        [Test]
        public void TestManyRandomRGBtoHSLtoRGB()
        {
            for (int i = 0; i < 2000; i++) {
                var rnd = new Random();
                Color rgb1 = Color.FromRgb(
                    (byte) rnd.Next(0, 255),
                    (byte) rnd.Next(0, 255),
                    (byte) rnd.Next(0, 255),
                    (byte) rnd.Next(0, 255));
                Color rgb2 = Color.FromHsl(rgb1.Hue, rgb1.Saturation, rgb1.Lightness);
                Assert.True(rgb1.Equals(rgb2));
                //Assert.True(rgb.AboutEqual(rgb2.ToRgb8Bit())); // accepts difference of 1 for every 8bit component
            }
        }

        [Test]
        public void TestRGBToHSL()
        {
            // Todo: Implement TestRGBToHSL()
            Color color = Color.FromRgb(0x8A, 0x3B, 0x20, 0xFF);
            // Hue: 15.2830188679245, Saturation: 0.623529411764706, Lightness: 0.333333333333333 
            Assert.That(color.Hue, Is.EqualTo(15.2830188679245).Within(0.000001));
            Assert.That(color.Saturation, Is.EqualTo(0.623529411764706).Within(0.000001));
            Assert.That(color.Lightness, Is.EqualTo(0.333333333333333).Within(0.000001));
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