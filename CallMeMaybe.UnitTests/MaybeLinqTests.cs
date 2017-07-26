using System;
using System.Linq;
using Xunit;

namespace CallMeMaybe.UnitTests
{
    public class MaybeLinqTests
    {
        [Fact]
        public void TestNotStringJoin()
        {
            var q = Maybe.Not;
            Assert.Equal("", string.Join(",", q));
        }

        [Fact]
        public void TestSingleLinq()
        {
            Assert.Equal(1, Maybe.From(1).Single());
            Assert.Equal("hi", Maybe.From("hi").Single());
        }

        [Fact]
        public void TestSelectManyLinq()
        {
            var q =
                (from number in Maybe.From(1)
                    from name in Maybe.From("hi")
                    select new {number, name})
                    .Single();
            Assert.Equal(1, q.number);
            Assert.Equal("hi", q.name);
        }

        [Fact]
        public void TestSelectManyLambda()
        {
            Assert.Equal("hi", Maybe.From(1).SelectMany(i => Maybe.From("hi")).Single());
            Assert.Equal("hi", Maybe.From(Maybe.From("hi")).SelectMany(i => i).Single());
            Assert.False(Maybe.From(1).SelectMany(i => Maybe.From((string)null)).HasValue);
            Assert.False(Maybe.From((string)null).SelectMany(s => Maybe.From(1)).HasValue);
            Assert.False(Maybe.From(Maybe.From((string)null)).SelectMany(s => s).HasValue);
            // Make sure selector is not called
            Assert.False(Maybe.From((string)null).SelectMany(new Func<string, Maybe<int>>(s => throw new Exception())).HasValue);
        }

        [Fact]
        public void TestWhereLinq()
        {
            var q =
                (from number in Maybe.From(1)
                 from name in Maybe.From("hi")
                 select new { number, name })
                    .Single();
            Assert.Equal(1, q.number);
            Assert.Equal("hi", q.name);
        }


        [Fact]
        public void TestValuesLinq()
        {
            var q = new[] {Maybe.From("hi"), Maybe.From((string) null), Maybe.From("world")}
                .Values();
            Assert.True(q.SequenceEqual(new[]{"hi", "world"}));
        }

    }
}