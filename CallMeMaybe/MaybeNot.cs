namespace CallMeMaybe
{
    /// <summary>
    /// An object that can be implicitly cast to a <see cref="Maybe{T}"/> with no value.
    /// </summary>
    public struct MaybeNot : IMaybe
    {
        /// <summary>
        /// Gets a string representation of this <see cref="Maybe{T}"/>.
        /// </summary>
        /// <returns>An empty string.</returns>
        public override string ToString()
        {
            return "";
        }

        public override bool Equals(object obj)
        {
            // Each separate type of Maybe counts as its own value.
            // All MaybeNots are the same, but different Maybe<>.Nots
            // are their own values.
            // This is just to maintain consistent behavior--users are
            // not expected to put different types of Maybe objects
            // in hashsets and such.
            return obj is MaybeNot;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets whether or not this <see cref="Maybe{T}"/> contains a value.
        /// </summary>
        /// <remarks>For <see cref="MaybeNot"/> this is always false.</remarks>
        bool IMaybe.HasValue { get { return false; } }

        /// <summary>
        /// Attempts to get the value.
        /// <remarks>
        /// For <see cref="MaybeNot"/>, this will always return false, 
        /// and produce a null output value.
        /// </remarks>
        /// </summary>
        /// <param name="value">
        /// In the case of <see cref="MaybeNot"/>, this will always yield null.
        /// </param>
        /// <returns>False, because this is a <see cref="MaybeNot"/></returns>
        bool IMaybe.TryGetValue(out object value)
        {
            value = null;
            return false;
        }
    }
}