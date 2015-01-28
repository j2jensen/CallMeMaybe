using System;
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
        public void TestSelectManyLambda()
        {
            Assert.AreEqual("hi", Maybe.From(1).SelectMany(i => Maybe.From("hi")).Single());
            Assert.AreEqual("hi", Maybe.From(Maybe.From("hi")).SelectMany(i => i).Single());
            Assert.IsFalse(Maybe.From(1).SelectMany(i => Maybe.From((string)null)).HasValue);
            Assert.IsFalse(Maybe.From((string)null).SelectMany(s => Maybe.From(1)).HasValue);
            Assert.IsFalse(Maybe.From(Maybe.From((string)null)).SelectMany(s => s).HasValue);
            // Make sure selector is not called
            Assert.IsFalse(Maybe.From((string)null).SelectMany(new Func<string, Maybe<int>>(s =>
            {
                throw new Exception();
            })).HasValue);
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
        public void TestValuesLinq()
        {
            var q = new[] {Maybe.From("hi"), Maybe.From((string) null), Maybe.From("world")}
                .Values();
            Assert.IsTrue(q.SequenceEqual(new[]{"hi", "world"}));
        }

    }
}