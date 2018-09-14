using System;
using System.Collections.Generic;
using NUnit.Framework;
using ColorSchemeManipulator.Filters;
using ColorSchemeManipulator.CLI;
using ColorSchemeManipulator.Common;
using ColorSchemeManipulator.Colors;

namespace ColorSchemeManipulator.UnitTests
{
    [TestFixture]
    public class CliTests
    {
        [Test]
        [TestCase("-li", "-li", null, null)]
        [TestCase("-c=12", "-c", "12", null)]
        [TestCase("--s=12", null, null, null)]
        [TestCase("--saturation=1.2", "--saturation", "1.2", null)]
        [TestCase("--saturation(hue:-350-10)=1.2", "--saturation", "1.2", "hue:-350-10")]
        [TestCase("-e(m:-300-20)=2", "-e", "2", "m:-300-20")]
        [TestCase("-li=.2", "-li", ".2", null)]
        [TestCase("-s(x:-300-20, a:0.1-0.5)=2", "-s", "2", "x:-300-20, a:0.1-0.5")]
        public void SplitArgIntoPieces_CallMethod(string arg, string optionString, string filterString, string rangeString)
        {
            (string o, string f, string r) = CliUtils.SplitArgIntoPieces(arg);
            Assert.That(optionString, Is.EqualTo(o));
            Assert.That(filterString, Is.EqualTo(f));
            Assert.That(rangeString, Is.EqualTo(r));
        }

        [Test]
        [Ignore("Private method")]
        [TestCase("sat:0-.5,hue:-10-20,l:0.1-1", "h|hue", true, -10, 20)]
        [TestCase("s:0-.5,hue:-10-20,l:0.1-1", "s|sat|saturation", true, 0, 0.5)]
        [TestCase("s:0-.5,hue:-10-20,l:0.1-1", "r|red", false, 0, 0)]
        public void TryParseRangeForRangeParam_CallMethod_ReturnMinAndMaxAndSucceeded(string rangeStr, string rangeParam,
            bool expSuccess, double expMin, double expMax)

        {
            ParameterRange range = CliUtils.TryParseRangeForRangeParam(rangeStr, rangeParam);
            Assert.That(range != null == expSuccess);
            Assert.That(range?.MinStart ?? 0, Is.EqualTo(expMin));
            Assert.That(range?.MaxEnd ?? 0, Is.EqualTo(expMax));
        }

        [Test]
        [TestCase("sat:0-0.5,hue:-10-20", 0.0)]
        [TestCase("sat:0.5-1.0,hue:-10-20", 1.0)]
        [TestCase("sat:0-.5,red:0.9-1", 0.0)]
        [TestCase("sat:0.5-1.0,red:0.9-1", 1.0)]
        [TestCase("hue:-10-0", 0.0)]
        [TestCase("hue:350-0", 0.0)]
        [TestCase("hue:350-10", 1.0)]
        public void ParseRange_GettingInRangeFactorForRgb_ReturnExpectedFactor(string rangeString, double expFactor)
        {
            Rgb rgb = new Rgb(1, 0.1, 0); // a bit yellowish max saturated red
            var range = CliUtils.ParseRange(rangeString);
            double factor = range.InRangeFactor(rgb);
            
            Assert.That(factor, Is.EqualTo(expFactor));
        }
            
        [Test]
        [TestCase("sat:0-0.5,hue:-10-20", 0.0)]
        [TestCase("sat:0.5-1.0", 1.0)]
        [TestCase("sat:0-.5,red:0.9-1", 0.0)]
        [TestCase("sat:0.5-1.0,red:0.9-1", 1.0)]
        [TestCase("hue:-10-0", 0.0)]
        [TestCase("hue:350-0", 0.0)]
        [TestCase("hue:350-10", 1.0)]
        public void ParseRange_GettingInRangeFactorForHsl_ReturnExpectedFactor(string rangeString, double expFactor)
        {
            Rgb rgb = new Rgb(1, 0.1, 0); // a bit yellowish max saturated red
            Hsl hsl = new Hsl(rgb);
            var range = CliUtils.ParseRange(rangeString);
            double factor = range.InRangeFactor(hsl);
            
            Assert.That(factor, Is.EqualTo(expFactor));
        }

        [Test]
        public void ExtractArgs_NullOrEmptyString_ReturnsEmptyList()
        {
            List<Object> resultNullString = CliUtils.ExtractParams(null);
            List<Object> resultEmptyString = CliUtils.ExtractParams("");
            Assert.That(resultNullString, Is.EqualTo(new List<object>()));
            Assert.That(resultEmptyString, Is.EqualTo(new List<object>()));
        }

        [Test]
        public void ExtractParams_CorrectlyFormattedString_ReturnsExpectedList()
        {
            const string inputString = "2.1, 1.0, 0.1,1 ";
            var result = CliUtils.ExtractParams(inputString);
            var expected = new List<object> {"2.1", "1.0", "0.1", "1"};

            if (result.Count == expected.Count) {
                for (int i = 0; i < expected.Count; i++) {
                    Assert.That(result[i], Is.EqualTo(expected[i]));
                }
            } else {
                Assert.Fail();
            }
        }
        
        [Test]
        public void ExtractParams_StringWithExtraComma_ReturnsListWithEmptyStrings()
        {
            const string inputString = ",2.1, 1.0, 0.1,1, ";
            var result = CliUtils.ExtractParams(inputString);
            var expected = new List<object> {"", "2.1", "1.0", "0.1", "1", ""};

            if (result.Count == expected.Count) {
                for (int i = 0; i < expected.Count; i++) {
                    Assert.That(result[i], Is.EqualTo(expected[i]));
                }
            } else {
                Assert.Fail();
            }
        }
        
    }
}