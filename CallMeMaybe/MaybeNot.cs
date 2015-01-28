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
            // Maybe values never equal `null`--that's sort of the point.
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            // Maybe values can be compared with other Maybe values.
            var maybe = obj as IMaybe;
            if (maybe != null)
            {
                object value;
                return !maybe.TryGetValue(out value);
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return 0;
            }
        }

        public static bool operator ==(MaybeNot left, MaybeNot right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MaybeNot left, MaybeNot right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Gets whether or not this <see cref="Maybe{T}"/> contains a value.
        /// <remarks>For <see cref="MaybeNot"/> this is always false.</remarks>
        /// </summary>
        // TODO: Test and figure out what to do about GetHashCode and Equals for  `MaybeNot`s
        // that aren't cast to Maybe{T}s
        public bool HasValue { get { return false; } }

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