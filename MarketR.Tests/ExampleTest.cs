using MarketR.DAL;
using NUnit.Framework;
using NUnit;

namespace MarketR.Tests
{
    [TestFixture()]
    [Ignore("Example")]
    public class ExampleTest
    {
        private Example example;

        [SetUp]
        public void Init()
        {
            example = new Example();
        }

        [Test]
        //[Category("admnin")]
        public void SumTest()
        {
            const int a = 2;
            const int b = 3;
            int expected = a + b;            

            int result = example.Sum(a, b);
            Assert.AreEqual(result, expected);
        }

        [Test]        
        [Ignore("ignore it")]
        public void DivByZeroTest()
        {
            const int a = 2;
            const int b = 0;

            example = new Example();
            example.Div(a, b);
        }
    }
}
