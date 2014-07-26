using System.Linq;
using CallMeMaybe;
using NUnit.Framework;

namespace TestMeMaybe
{
    [TestFixture]
    public class MaybeLinqTests
    {
        [Test]
        public void TestNotStringJoin()
        {
            var q = Maybe.Not;
            Assert.AreEqual("", string.Join(",", q));
        }

        [Test]
        public void TestSingleLinq()
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

        [Test]
        public void TestWhereLinq()
        {
            var q =
                (from number in Maybe.From(1)
                 from name in Maybe.From("hi")
                 select new { number, name })
                    .Single();
            Assert.AreEqual(1, q.number);
            Assert.AreEqual("hi", q.name);
        }

        [Test]
        public void TestConcat()
        {
            Assert.IsTrue(Maybe.From(1).Concat(new[] {2, 3}).SequenceEqual(new[]{1,2,3}));
            Assert.IsTrue(Maybe.From((int?)null).Concat(new[] {2, 3}).SequenceEqual(new[]{2,3}));
            Assert.IsTrue(Maybe.From("hi").Concat(new[] {"big", "world" })
                .SequenceEqual(new[] { "hi", "big", "world" }));
            Assert.IsTrue(Maybe.From((string)null).Concat(new[] { "big", "world" })
                .SequenceEqual(new[] { "big", "world" }));
        }
    }
}