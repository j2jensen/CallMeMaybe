using System;

namespace CallMeMaybe
{
    public static class MaybeEitherExtensions
    {
        public static Either<TLeft, TRight> Or<TLeft, TRight>(this Maybe<TLeft> leftMaybe,
            Func<TRight> getRightValue)
        {
            return leftMaybe.Select(v => new Either<TLeft, TRight>(v))
                .Else(new Either<TLeft, TRight>(getRightValue()));
        }
    }
}