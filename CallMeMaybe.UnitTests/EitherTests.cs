using System;
using FluentAssertions;
using Xunit;

namespace CallMeMaybe.UnitTests
{
    public class EitherTests
    {
        [Fact]
        public void TestIsLeftAndIsRight()
        {
            var eitherA = new Either<A, B>(new A());
            eitherA.IsLeft.Should().BeTrue();
            eitherA.IsRight.Should().BeFalse();
            var eitherB = new Either<A, B>(new B());
            eitherB.IsLeft.Should().BeFalse();
            eitherB.IsRight.Should().BeTrue();
        }

        [Fact]
        public void TestMatch()
        {
            var eitherA = new Either<string, B>("foo");
            eitherA.Match(
                    ifLeft: v => v + "bar",
                    ifRight: v => throw new Exception())
                .Should().Be("foobar");

            var eitherB = new Either<A, string>("foo");
            eitherB.Match(
                    ifLeft: v => throw new Exception(),
                    ifRight: v => v + "bar")
                .Should().Be("foobar");
        }


        [Fact]
        public void TestImplicitCasting()
        {
            {
                var a = new A();
                var b = new B();
                Either<A, B> eitherA = a;
                Either<A, B> eitherB = b;
                eitherA.ShouldBeLeft(a);
                eitherB.ShouldBeRight(b);
            }
            {
                Either<string, object> either = "foo";
                either.ShouldBeLeft("foo");
            }

            {
                Either<string, object> either = (object) "foo";
                either.ShouldBeRight("foo");
            }

            {
                var o = new object();
                Either<string, object> either = o;
                either.ShouldBeRight(o);
            }
        }

        // A couple of classes with short names
        public class A
        {
        }

        public class B
        {
        }

        // Structs
        public struct AStruct { }
        public struct BStruct { }
    }

}