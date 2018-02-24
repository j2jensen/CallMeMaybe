using System;
using FluentAssertions;
using Xunit;

namespace CallMeMaybe.UnitTests
{
    public class MaybeEitherExtensionTests
    {
        [Fact]
        public void TestMaybeOr()
        {
            var eitherFoo = Maybe.From("foo").Or(() => "bar");
            eitherFoo
                .IsLeft
                .Should().BeTrue();

            var eitherBar = Maybe.From((string)null);
            eitherBar.Or(() => "bar")
                .IsRight
                .Should().BeTrue();
        }
        
    }
}