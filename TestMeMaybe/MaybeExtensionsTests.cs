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
            Assert.AreEqual("1", UseMaybe(((int?)1).Maybe()));
            Assert.AreEqual("", UseMaybe(((int?)null).Maybe()));
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