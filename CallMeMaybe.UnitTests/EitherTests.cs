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
            var eitherA = new Either<string, A>("foo");
            eitherA.Match(
                    ifLeft: v => v + "bar",
                    ifRight: v => throw new Exception())
                .Should().Be("foobar");


            var eitherB = new Either<B, string>("foo");
            eitherB.Match(
                    ifLeft: v => throw new Exception(),
                    ifRight: v => v + "bar")
                .Should().Be("foobar");


            var notMatching = new Either<string, A>(new A());
            Action action = () => {
                notMatching.Match(
                    ifLeft: v => v + "bar",
                    ifRight: v => throw new Exception()); };
            action.ShouldThrow<Exception>();
        }

        [Fact]
        public void TestFlip()
        {
            var eitherA = new Either<A, string>(new A());
            var flippedA = eitherA.Flip();
            flippedA.IsRight.Should().BeTrue();
            flippedA.IsLeft.Should().BeFalse();

            var eitherB = new Either<string, B>(new B());
            var flippedB = eitherB.Flip();
            flippedB.IsLeft.Should().BeTrue();
            flippedB.IsRight.Should().BeFalse();
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