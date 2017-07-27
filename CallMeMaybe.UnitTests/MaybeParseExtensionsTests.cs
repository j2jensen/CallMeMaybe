using System;
using System.Globalization;
using FluentAssertions;
using Xunit;

namespace CallMeMaybe.UnitTests
{
    public class MaybeParseExtensionsTests
    {
        [Fact]
        public void TestIntParse()
        {
            Assert.Equal(Maybe<int>.Not, Maybe.From((string) null).ParseInt32());
            Assert.Equal(Maybe<int>.Not, Maybe.From("").ParseInt32());
            Assert.Equal(Maybe<int>.Not, Maybe.From("hi").ParseInt32());
            Assert.Equal(Maybe.From(42), Maybe.From("42").ParseInt32());
            Assert.NotEqual(Maybe.From(66), Maybe.From("42").ParseInt32());
            Assert.Equal(Maybe<int>.Not, Maybe.From("2A").ParseInt32());
            Assert.Equal(Maybe<int>.Not,
                Maybe.From((int.MaxValue + 1L).ToString(CultureInfo.InvariantCulture)).ParseInt32());
        }

        [Fact]
        public void TestLongParse()
        {
            Assert.Equal(Maybe<long>.Not, Maybe.From((string) null).ParseInt64());
            Assert.Equal(Maybe<long>.Not, Maybe.From("").ParseInt64());
            Assert.Equal(Maybe<long>.Not, Maybe.From("hi").ParseInt64());
            Assert.Equal(Maybe.From(42L), Maybe.From("42").ParseInt64());
            Assert.Equal(Maybe.From(long.MaxValue),
                Maybe.From(long.MaxValue.ToString(CultureInfo.InvariantCulture)).ParseInt64());
            Assert.Equal(Maybe.From(long.MinValue),
                Maybe.From(long.MinValue.ToString(CultureInfo.InvariantCulture)).ParseInt64());
            Assert.NotEqual(Maybe.From(66L), Maybe.From("42").ParseInt64());
            Maybe.From(42).Should().NotBe(Maybe.From("42").ParseInt64());
            Assert.Equal(Maybe<long>.Not, Maybe.From("2A").ParseInt64());
            Assert.Equal(Maybe<long>.Not, Maybe.From("2A").ParseInt64());
        }

        [Fact]
        public void TestBooleanParse()
        {
            Assert.Equal(Maybe<bool>.Not, Maybe.From((string) null).ParseBoolean());
            Assert.Equal(Maybe<bool>.Not, Maybe.From("").ParseBoolean());
            Assert.Equal(Maybe<bool>.Not, Maybe.From("hi").ParseBoolean());
            Assert.Equal(Maybe<bool>.Not, Maybe.From("42").ParseBoolean());
            Assert.Equal(Maybe<bool>.Not, Maybe.From("1").ParseBoolean());
            Assert.Equal(Maybe<bool>.Not, Maybe.From("0").ParseBoolean());
            Assert.Equal(Maybe.From(false), Maybe.From("false").ParseBoolean());
            Assert.Equal(Maybe.From(false), Maybe.From("False").ParseBoolean());
            Assert.Equal(Maybe.From(true), Maybe.From("true").ParseBoolean());
            Assert.Equal(Maybe.From(true), Maybe.From("True").ParseBoolean());
        }
    }
}