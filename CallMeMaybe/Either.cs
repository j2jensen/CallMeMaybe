using System;

namespace CallMeMaybe
{
    public sealed class Either<TLeft, TRight>
    {
        private readonly TLeft _left;
        private readonly TRight _right;
        private readonly bool _isLeft;

        public Either(TLeft left)
        {
            if (ReferenceEquals(null, left))
            {
                throw new ArgumentNullException(nameof(left), "Either cannot be initialized with a null value");
            }
            _left = left;
            _isLeft = true;
        }

        public Either(TRight right)
        {
            if (ReferenceEquals(null, right))
            {
                throw new ArgumentNullException(nameof(right), "Either cannot be initialized with a null value");
            }
            _right = right;
            _isLeft = false;
        }

        public bool IsLeft => _isLeft;

        public bool IsRight => !_isLeft;

        public TResult Match<TResult>(Func<TLeft, TResult> ifLeft, Func<TRight, TResult> ifRight)
        {
            return _isLeft ? ifLeft(_left) : ifRight(_right);
        }

        public Either<TRight, TLeft> Flip()
        {
            return _isLeft ? new Either<TRight, TLeft>(_left) : new Either<TRight, TLeft>(_right);
        }

        public Either<TLeftNew, TRight> SelectLeft<TLeftNew>(Func<TLeft, Either<TLeftNew, TRight>> selector)
        {
            return Match(selector, r => r);
        }

        public static implicit operator Either<TLeft, TRight>(TLeft value)
        {
            return new Either<TLeft, TRight>(value);
        }

        public static implicit operator Either<TLeft, TRight>(TRight value)
        {
            return new Either<TLeft, TRight>(value);
        }

    }
}