using System.Xml.Schema;
using NUnit.Framework;
using ColorSchemeInverter.Filters;
using ColorSchemeInverter.CLI;
using ColorSchemeInverter.Common;
using ColorSchemeInverter.Colors;

namespace ColorSchemeInverter.UnitTests
{
    [TestFixture]
    public class ExtensionsTests
    {
        [Test]
        public void MaxMin_ComparesMaxMinDoubles_ReturnsExpectedValues()
        {
            double lesser = 10.0;
            double greater = 20.0;
            double min1 = lesser.Min(greater);
            double min2 = greater.Min(lesser);
            double max1 = lesser.Max(greater);
            double max2 = greater.Max(lesser);
            Assert.That(min1, Is.EqualTo(lesser));
            Assert.That(min2, Is.EqualTo(lesser));
            Assert.That(max1, Is.EqualTo(greater));
            Assert.That(max2, Is.EqualTo(greater));
        }
    }
}