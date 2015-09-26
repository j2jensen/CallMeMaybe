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

        /// <summary>
        /// An <see cref="Equals"/> override, which will only return <value>true</value> if
        /// the object is another <see cref="MaybeNot"/>.
        /// </summary>
        /// <remarks>
        /// Each separate type of Maybe counts as its own value.
        /// All <see cref="MaybeNot"/>s are the same, but different <see cref="Maybe{T}.Not"/>s
        /// are their own values.
        /// This is just to maintain consistent behavior--users are not expected to put different types of Maybe objects
        /// in hashsets and such.
        /// </remarks>
        /// <param name="obj">The object to compare this one with.</param>
        /// <returns><value>true</value> if the given value is a <see cref="MaybeNot"/>, <value>false</value> otherwise.</returns>
        public override bool Equals(object obj)
        {
            return obj is MaybeNot;
        }

        /// <summary>
        /// A <see cref="GetHashCode"/> override.
        /// </summary>
        /// <returns>A hashcode which is guaranteed to be the same for all <see cref="MaybeNot"/>s.</returns>
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
        /// </summary>
        /// <remarks>
        /// For <see cref="MaybeNot"/>, this will always return false, 
        /// and produce a null output value.
        /// </remarks>
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