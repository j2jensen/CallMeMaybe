using System;
using System.Globalization;
using CallMeMaybe;
using NUnit.Framework;
using System.Linq;

namespace TestMeMaybe
{
    [TestFixture]
    public class MaybeExtensionsTests
    {
        [Test]
        public void TestDictionaryMaybeValue()
        {
            var values = new[] {1, 2, 4};
            var dict = values.ToDictionary(v => v.ToString(CultureInfo.InvariantCulture));
            var values2 =
                (from k in new[] {"1", "2", "3"}
                    from v in dict.GetMaybe(k)
                    select new {k, v})
                    .ToList();
            Assert.AreEqual(values2.Count, 2);
            Assert.AreEqual(1, values2[0].v);
            Assert.AreEqual("1", values2[0].k);
            Assert.AreEqual(2, values2[1].v);
            Assert.AreEqual("2", values2[1].k);
        }

        [Test]
        public void TestNullableConversions()
        {
            Assert.AreEqual("1", UseNullable(Maybe.From(1).Nullable()));
            Assert.AreEqual("", UseNullable(Maybe<int>.Not.Nullable()));
            Assert.AreEqual("1", UseMaybe(((int?) 1).Maybe()));
            Assert.AreEqual("", UseMaybe(((int?) null).Maybe()));
        }

        [Test]
        public void TestFirstMaybe()
        {
            Assert.AreEqual(Maybe.From(1), new[] {1}.FirstMaybe());
            Assert.AreEqual(Maybe.From(1), new[] {1}.AsQueryable().FirstMaybe());
            Assert.AreEqual(Maybe<int>.Not, new int?[] {null}.FirstMaybe());
            Assert.AreEqual(Maybe<int>.Not, new int?[] {null}.AsQueryable().FirstMaybe());
            Assert.AreEqual(Maybe<int>.Not, new int?[] {null, 2, 3}.FirstMaybe());
            Assert.AreEqual(Maybe<int>.Not, new int?[] {null, 2, 3}.AsQueryable().FirstMaybe());
            Assert.AreEqual(Maybe.From(1), new[] {1, 2, 3}.FirstMaybe());
            Assert.AreEqual(Maybe.From(1), new[] {1, 2, 3}.AsQueryable().FirstMaybe());
            Assert.AreEqual(Maybe<int>.Not, new int[0].FirstMaybe());
            Assert.AreEqual(Maybe<int>.Not, new int[0].AsQueryable().FirstMaybe());

            Assert.AreEqual(Maybe.From("1"), new[] {"1"}.FirstMaybe());
            Assert.AreEqual(Maybe.From("1"), new[] {"1"}.AsQueryable().FirstMaybe());
            Assert.AreEqual(Maybe<string>.Not, new string[] {null}.FirstMaybe());
            Assert.AreEqual(Maybe<string>.Not, new string[] {null}.AsQueryable().FirstMaybe());
            Assert.AreEqual(Maybe<string>.Not, new[] {null, "2", "3"}.FirstMaybe());
            Assert.AreEqual(Maybe<string>.Not, new[] {null, "2", "3"}.AsQueryable().FirstMaybe());
            Assert.AreEqual(Maybe.From("1"), new[] {"1", "2", "3"}.FirstMaybe());
            Assert.AreEqual(Maybe.From("1"), new[] {"1", "2", "3"}.AsQueryable().FirstMaybe());
            Assert.AreEqual(Maybe<string>.Not, new string[0].FirstMaybe());
            Assert.AreEqual(Maybe<string>.Not, new string[0].AsQueryable().FirstMaybe());
        }

        [Test]
        public void TestSingleMaybe()
        {
            Assert.AreEqual(Maybe.From(1), new[] {1}.SingleMaybe());
            Assert.AreEqual(Maybe.From(1), new[] {1}.AsQueryable().SingleMaybe());
            Assert.AreEqual(Maybe<int>.Not, new int?[] { null }.SingleMaybe());
            Assert.AreEqual(Maybe<int>.Not, new int?[] { null }.AsQueryable().SingleMaybe());
            Assert.Throws<InvalidOperationException>(() => new int?[] { null, 2, 3 }.SingleMaybe(),
                "Sequence contains more than one element");
            Assert.Throws<InvalidOperationException>(() => new int?[] { null, 2, 3 }.AsQueryable().SingleMaybe(),
                "Sequence contains more than one element");
            Assert.Throws<InvalidOperationException>(() => new[] { 1, 2, 3 }.SingleMaybe(),
                "Sequence contains more than one element");
            Assert.Throws<InvalidOperationException>(() => new[] {1, 2, 3}.AsQueryable().SingleMaybe(),
                "Sequence contains more than one element");
            Assert.AreEqual(Maybe<int>.Not, new int[0].SingleMaybe());
            Assert.AreEqual(Maybe<int>.Not, new int[0].AsQueryable().SingleMaybe());

            Assert.AreEqual(Maybe.From("1"), new[] {"1"}.SingleMaybe());
            Assert.AreEqual(Maybe.From("1"), new[] {"1"}.AsQueryable().SingleMaybe());
            Assert.AreEqual(Maybe<string>.Not, new string[] {null}.SingleMaybe());
            Assert.AreEqual(Maybe<string>.Not, new string[] {null}.AsQueryable().SingleMaybe());
            Assert.Throws<InvalidOperationException>(() => new [] { null, "2", "3" }.SingleMaybe(),
                "Sequence contains more than one element");
            Assert.Throws<InvalidOperationException>(() => new [] { null, "2", "3" }.AsQueryable().SingleMaybe(),
                "Sequence contains more than one element");
            Assert.Throws<InvalidOperationException>(() => new[] { "1", "2", "3" }.SingleMaybe(),
                "Sequence contains more than one element");
            Assert.Throws<InvalidOperationException>(() => new[] {"1", "2", "3"}.AsQueryable().SingleMaybe(),
                "Sequence contains more than one element");
            Assert.AreEqual(Maybe<string>.Not, new string[0].SingleMaybe());
            Assert.AreEqual(Maybe<string>.Not, new string[0].AsQueryable().SingleMaybe());
        }

        private string UseNullable(int? x)
        {
            return x.ToString();
        }

        private string UseMaybe(Maybe<int> x)
        {
            return x.ToString();
        }
    }
}