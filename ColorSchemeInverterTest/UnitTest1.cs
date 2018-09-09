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
            Rgb rgb1 = new Rgb8Bit(0x8A, 0x3B, 0x20).ToRgb();
            Rgb rgb2 = Rgb.FromRgbString("8A3B20FF", "RRGGBBAA");
            Assert.True(rgb1.Equals(rgb2));
        }

        [Test]
        public void TestInvertLightness()
        {
            Rgb rgb1 = new Rgb8Bit(0x8A, 0x3B, 0x20).ToRgb();
            Rgb rgb1inv = rgb1.ToHsl().ApplyFilter(new HslFilter(FilterBundle.LightnessInvert)).ToRgb();
            Assert.True(rgb1inv.Equals(Rgb.FromRgbaString("DF9075FF")));

            Rgb rgb2 = new Rgb8Bit(0xE0, 0xE0, 0xE0, 0x80).ToRgb();
            Rgb rgb2inv = rgb2.ToHsl().ApplyFilter(new HslFilter(FilterBundle.LightnessInvert)).ToRgb();
            Assert.True(rgb2inv.Equals(Rgb.FromRgbaString("1F1F1F80")));
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