namespace CallMeMaybe
{
    public struct Maybe<T>
    {
        private T value;
        private bool _hasValue;
        public bool HasValue { get { return _hasValue; } }
    }
}
