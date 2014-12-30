namespace CallMeMaybe
{
    /// <summary>
    /// An object that can be implicitly cast to a <see cref="Maybe{T}"/> with no value.
    /// </summary>
    public struct MaybeNot
    {
        /// <summary>
        /// Gets a string representation of this <see cref="Maybe{T}"/>.
        /// </summary>
        /// <returns>An empty string.</returns>
        public override string ToString()
        {
            return "";
        }

        // TODO: Test and figure out what to do about GetHashCode and Equals for  `MaybeNot`s
        // that aren't cast to Maybe{T}s
    }
}