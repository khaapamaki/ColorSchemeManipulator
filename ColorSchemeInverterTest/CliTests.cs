using NUnit.Framework;
using ColorSchemeInverter.Filters;
using ColorSchemeInverter.CLI;
using ColorSchemeInverter.Common;
using ColorSchemeInverter.Colors;

namespace ColorSchemeInverter.UnitTests
{
    [TestFixture]
    public class CliArgsTests
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
    }
}