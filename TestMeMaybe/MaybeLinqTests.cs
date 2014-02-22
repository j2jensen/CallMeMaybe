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
                select m;
            Assert.AreEqual("", string.Join(",", q));
        }

        [Test]
        public void TestSelectLinq()
        {
            Assert.AreEqual(1, Maybe.From(1).Single());
            Assert.AreEqual("hi", Maybe.From("hi").Single());
        }

        [Test]
        public void TestSelectManyLinq()
        {
            var q =
                (from number in Maybe.From(1)
                    from name in Maybe.From("hi")
                    select new {number, name})
                    .Single();
            Assert.AreEqual(1, q.number);
            Assert.AreEqual("hi", q.name);
        }
    }
}