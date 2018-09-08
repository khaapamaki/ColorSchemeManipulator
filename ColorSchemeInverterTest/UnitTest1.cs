using System;
using ColorSchemeInverter;
using ColorSchemeInverter.Colors;
using ColorSchemeInverter.Common;
using ColorSchemeInverter.Filters;
using NUnit.Framework;

namespace ColorSchemeInverterTest
{
    public class Tests
    {
        [SetUp]
        public void Setup() { }

        [Test]
        public void TestRGB()
        {
            RGB rgb1 = new RGB8bit(0x8A, 0x3B, 0x20).ToRGB();
            RGB rgb2 = RGB.FromRGBString("8A3B20FF", "RRGGBBAA");
            Assert.True(rgb1.Equals(rgb2));
        }

        [Test]
        public void TestInvertLightness()
        {
            RGB rgb1 = new RGB8bit(0x8A, 0x3B, 0x20).ToRGB();
            RGB rgb1inv = rgb1.ToHSL().ApplyFilter(new HSLFilter(FilterBundle.LightnessInvert)).ToRGB();
            Assert.True(rgb1inv.Equals(RGB.FromRGBAString("DF9075FF")));

            RGB rgb2 = new RGB8bit(0xE0, 0xE0, 0xE0, 0x80).ToRGB();
            RGB rgb2inv = rgb2.ToHSL().ApplyFilter(new HSLFilter(FilterBundle.LightnessInvert)).ToRGB();
            Assert.True(rgb2inv.Equals(RGB.FromRGBAString("1F1F1F80")));
        }

        [Test]
        public void TestRGBtoHSLtoRGB()
        {
            RGB rgb1 = new RGB8bit(0x8A, 0x3B, 0x20).ToRGB();
            RGB rgb2 = rgb1.ToHSL().ToRGB();
            Assert.True(rgb1.Equals(rgb2));
        }

        [Test]
        public void TestManyRandomRGBtoHSLtoRGB()
        {
            for (int i = 0; i < 2000; i++) {
                var rnd = new Random();
                RGB8bit rgb = new RGB8bit(
                    (byte) rnd.Next(0, 255),
                    (byte) rnd.Next(0, 255),
                    (byte) rnd.Next(0, 255),
                    (byte) rnd.Next(0, 255));
                RGB rgb1 = rgb.ToRGB();
                RGB rgb2 = rgb1.ToHSL().ToRGB();
                Assert.True(rgb1.Equals(rgb1.ToHSL().ToRGB()));
                Assert.True(rgb.AboutEqual(rgb2.ToRGB8Bit())); // accepts difference of 1 for every 8bit component
            }
        }

        [Test]
        public void TestRGBToHSL()
        {
            // Todo: Implement TestRGBToHSL()
            RGB rgb1 = new RGB(new RGB8bit(0x8A, 0x3B, 0x20, 0xFF));
            HSL hsl = rgb1.ToHSL();
            // Hue: 15.2830188679245, Saturation: 0.623529411764706, Lightness: 0.333333333333333 
            Assert.True(hsl.Hue.AboutEqual(15.2830188679245));
            Assert.True(hsl.Saturation.AboutEqual(0.623529411764706));
            Assert.True(hsl.Lightness.AboutEqual(0.333333333333333));
        }

        [Test]
        public void TestClamp()
        {
            double a = 2.0;
            Assert.True(a.Clamp(0,1.0) == 1.0);
            Assert.True(a.Min(0) == 2.0);
            Assert.True(a.Max(1.1) == 1.1);
   
        }
    }
}