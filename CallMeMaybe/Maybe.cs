namespace CallMeMaybe
{
    public struct Maybe<T>
    {
        private readonly T value;
        private readonly bool _hasValue;
        public bool HasValue { get { return _hasValue; } }
    }
}
