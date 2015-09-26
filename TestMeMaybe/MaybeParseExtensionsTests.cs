using System.Globalization;
using CallMeMaybe;
using NUnit.Framework;

namespace TestMeMaybe
{
    [TestFixture]
    public class MaybeParseExtensionsTests
    {
        [Test]
        public void TestIntParse()
        {
            Assert.AreEqual(Maybe<int>.Not, Maybe.From((string) null).ParseInt32());
            Assert.AreEqual(Maybe<int>.Not, Maybe.From("").ParseInt32());
            Assert.AreEqual(Maybe<int>.Not, Maybe.From("hi").ParseInt32());
            Assert.AreEqual(Maybe.From(42), Maybe.From("42").ParseInt32());
            Assert.AreNotEqual(Maybe.From(66), Maybe.From("42").ParseInt32());
            Assert.AreEqual(Maybe<int>.Not, Maybe.From("2A").ParseInt32());
            Assert.AreEqual(Maybe<int>.Not,
                Maybe.From((int.MaxValue + 1L).ToString(CultureInfo.InvariantCulture)).ParseInt32());
        }

        [Test]
        public void TestLongParse()
        {
            Assert.AreEqual(Maybe<long>.Not, Maybe.From((string) null).ParseInt64());
            Assert.AreEqual(Maybe<long>.Not, Maybe.From("").ParseInt64());
            Assert.AreEqual(Maybe<long>.Not, Maybe.From("hi").ParseInt64());
            Assert.AreEqual(Maybe.From(42L), Maybe.From("42").ParseInt64());
            Assert.AreEqual(Maybe.From(long.MaxValue),
                Maybe.From(long.MaxValue.ToString(CultureInfo.InvariantCulture)).ParseInt64());
            Assert.AreEqual(Maybe.From(long.MinValue),
                Maybe.From(long.MinValue.ToString(CultureInfo.InvariantCulture)).ParseInt64());
            Assert.AreNotEqual(Maybe.From(66L), Maybe.From("42").ParseInt64());
            Assert.AreNotEqual(Maybe.From(42), Maybe.From("42").ParseInt64());
            Assert.AreEqual(Maybe<long>.Not, Maybe.From("2A").ParseInt64());
            Assert.AreEqual(Maybe<long>.Not, Maybe.From("2A").ParseInt64());
        }

        [Test]
        public void TestBooleanParse()
        {
            Assert.AreEqual(Maybe<bool>.Not, Maybe.From((string) null).ParseBoolean());
            Assert.AreEqual(Maybe<bool>.Not, Maybe.From("").ParseBoolean());
            Assert.AreEqual(Maybe<bool>.Not, Maybe.From("hi").ParseBoolean());
            Assert.AreEqual(Maybe<bool>.Not, Maybe.From("42").ParseBoolean());
            Assert.AreEqual(Maybe<bool>.Not, Maybe.From("1").ParseBoolean());
            Assert.AreEqual(Maybe<bool>.Not, Maybe.From("0").ParseBoolean());
            Assert.AreEqual(Maybe.From(false), Maybe.From("false").ParseBoolean());
            Assert.AreEqual(Maybe.From(false), Maybe.From("False").ParseBoolean());
            Assert.AreEqual(Maybe.From(true), Maybe.From("true").ParseBoolean());
            Assert.AreEqual(Maybe.From(true), Maybe.From("True").ParseBoolean());
        }
    }
}