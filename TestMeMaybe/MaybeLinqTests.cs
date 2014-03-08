using CallMeMaybe;
using NUnit.Framework;
using System.Linq;

namespace TestMeMaybe
{
    [TestFixture]
    public class MaybeLinqTests
    {
        [Test]
        public void TestNotStringJoin()
        {
            var q = Maybe.Not<int>();
            Assert.AreEqual("", string.Join(",", q));
        }

        [Test]
        public void TestValuedStringJoin()
        {
            var q = Maybe.From(1).Concat(Maybe.From(2));
            Assert.AreEqual("1,2", string.Join(",", q));
        }


        [Test]
        public void TestMultipleTypeValuedStringJoin()
        {
            var q = Maybe.From<object>(1).Concat(Maybe.From("hi"));
            Assert.AreEqual("1,hi", string.Join(",", q));
        }

        [Test]
        public void TestCastLinq()
        {
            var q = Maybe.From(1).Cast<object>().Concat(Maybe.From("hi"));
            Assert.AreEqual("1,hi", string.Join(",", q));
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

    }
}