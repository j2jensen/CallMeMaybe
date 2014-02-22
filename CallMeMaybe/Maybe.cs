namespace CallMeMaybe
{
    public struct Maybe<T>
    {
        private readonly T _value;
        private readonly bool _hasValue;
        public bool HasValue { get { return _hasValue; } }

        public Maybe(T value)
        {
            _hasValue = true;
            _value = value;
        }
    }

    public static class Maybe
    {
        public static Maybe<T> From<T>(T value)
        {
            return new Maybe<T>(value);
        }
        
    }
}
