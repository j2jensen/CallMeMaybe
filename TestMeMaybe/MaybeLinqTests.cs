using System.Globalization;
using CallMeMaybe;
using NUnit.Framework;
using System.Linq;

namespace TestMeMaybe
{
    [TestFixture]
    public class MaybeLinqTests
    {
        [Test]
        public void TestEmptySelectLinq()
        {
            var q =
                from m in Maybe.Empty<int>()
                select m.ToString(CultureInfo.InvariantCulture);
            Assert.AreEqual("", string.Join(",", q));
        }

        [Test]
        public void TestValueSelectLinq()
        {
            var q =
                from m in Maybe.From(1)
                select m.ToString(CultureInfo.InvariantCulture);
            Assert.AreEqual("1", string.Join(",", q));
        }

        [Test]
        public void TestReferenceSelectLinq()
        {
            var q =
                from m in Maybe.From("hi")
                select m.ToString(CultureInfo.InvariantCulture);
            Assert.AreEqual("hi", string.Join(",", q));
        }
    }
}