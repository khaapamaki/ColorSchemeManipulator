using System;
using System.Diagnostics;
using ColorSchemeInverter;
using NUnit.Framework;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup() { }

        [Test]
        public void TestRGB()
        {
            RGB rgb1 = new RGB(0x8A, 0x3B, 0x20, 0xFF);
            RGB rgb2 = RGB.FromRGBString("8A3B20FF", "RRGGBBAA");
            Assert.True(rgb1.Equals(rgb2));
        }

        [Test]
        public void TestInvertLightness()
        {
            RGB rgb1 = new RGB(0x8A, 0x3B, 0x20, 0xFF);
            RGB rgb1inv = rgb1.ToHSL().ApplyFilter(new HSLFilter(FilterBundle.InvertLightness)).ToRGB();
            Assert.True(rgb1inv.Equals(RGB.FromRGBAString("DF9075FF")));

            RGB rgb2 = new RGB(0xE0, 0xE0, 0xE0, 0x80);
            RGB rgb2inv = rgb2.ToHSL().ApplyFilter(new HSLFilter(FilterBundle.InvertLightness)).ToRGB();
            Assert.True(rgb2inv.Equals(RGB.FromRGBAString("1F1F1F80")));
        }

        [Test]
        public void TestRGBtoHSLtoRGB()
        {
            RGB rgb1 = new RGB(0x8A, 0x3B, 0x20, 0xFF);
            Assert.True(rgb1.Equals(rgb1.ToHSL().ToRGB()));
        }

        [Test]
        public void TestManyRandomRGBtoHSLtoRGB()
        {
            for (int i = 0; i < 1000; i++) {
                var rnd = new Random();
                RGB rgb = new RGB((byte) rnd.Next(0, 255),
                    (byte) rnd.Next(0, 255),
                    (byte) rnd.Next(0, 255),
                    (byte) rnd.Next(0, 255));
                Assert.True(rgb.Equals(rgb.ToHSL().ToRGB()));
            }
        }

        [Test]
        public void TestRGBToHSL()
        {
            // Todo: Implement TestRGBToHSL()
            RGB rgb1 = new RGB(0x8A, 0x3B, 0x20, 0xFF);
            HSL hsl = rgb1.ToHSL();
            // Hue: 15.2830188679245, Saturation: 0.623529411764706, Lightness: 0.333333333333333 
            Assert.True(hsl.Hue.AboutEqual(15.2830188679245));
            Assert.True(hsl.Saturation.AboutEqual(0.623529411764706));
            Assert.True(hsl.Lightness.AboutEqual(0.333333333333333));
        }
    }
}