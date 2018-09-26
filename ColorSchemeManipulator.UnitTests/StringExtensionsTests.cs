using ColorSchemeManipulator.Common;
using NUnit.Framework;

namespace ColorSchemeManipulator.UnitTests
{
    [TestFixture]
    public class StringExtensionsTests
    {
        [Test]
        [TestCase("000000", "2030", "002030")]
        public void PadLeft_Test_ReturnsExpected(string padding, string input, string expected)
        {
            var result = input.PadLeft(padding);
            
            Assert.That(result, Is.EqualTo(expected));
        }
        
        [Test]
        [TestCase("000000ff", "102030", "102030ff")]
        public void PadRight_Test_ReturnsExpected(string padding, string input, string expected)
        {
            var result = input.PadRight(padding);
            
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}