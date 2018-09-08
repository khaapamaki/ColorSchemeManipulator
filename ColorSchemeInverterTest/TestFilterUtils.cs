using NUnit.Framework;
using ColorSchemeInverter.Filters;
    
namespace ColorSchemeInverterTest
{
    public class TestFilterUtils
    {
        [Test]
        public void Linear01_FactorAtMin_ReturnsRangeStart()
        {
            double result = FilterUtils.Linear01(0, 100.0, 200.0);
            Assert.That(result == 100.0);
        }
        
        [Test]
        public void Linear01_FactorAtMax_ReturnsRangeEnd()
        {
            double result = FilterUtils.Linear01(1, 100.0, 200.0);
            Assert.That(result == 200.0);
        }
        
        [Test]
        public void Linear01_FactorInBetween_ReturnsValueInBetween()
        {
            double result = FilterUtils.Linear01(0.1, 100.0, 200.0);
            Assert.That(result == 110.0);
        }
        
        [Test]
        public void Linear01_FactorBelowMin_ReturnsRangeEnd()
        {
            double result = FilterUtils.Linear01(-1, 100.0, 200.0);
            Assert.That(result == 100.0);
        }
        
        [Test]
        public void Linear01_FactorAboveMax_ReturnsRangeEnd()
        {
            double result = FilterUtils.Linear01(1.1, 100.0, 200.0);
            Assert.That(result == 200.0);
        }
        
    }
}