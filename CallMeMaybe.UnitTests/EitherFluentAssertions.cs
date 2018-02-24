using CallMeMaybe;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

public static class EitherFluentAssertions
{
    public static AndConstraint<ObjectAssertions> ShouldBeLeft<TLeft, TRight>(this Either<TLeft, TRight> either,
        TLeft expected)
    {
        return either.Match(
                ifLeft: left => left,
                ifRight: right =>
                    throw new AssertionFailedException("Expected left value " + expected + " but got right value " +
                                                       right +
                                                       " instead."))
            .Should().Be(expected);
    }

    public static AndConstraint<ObjectAssertions> ShouldBeRight<TLeft, TRight>(this Either<TLeft, TRight> either,
        TRight expected)
    {
        return either.Match(
                ifLeft: left =>
                    throw new AssertionFailedException("Expected left value " + expected + " but got right value " +
                                                       left +
                                                       " instead."),
                ifRight: right => right)
            .Should().Be(expected);
    }
}