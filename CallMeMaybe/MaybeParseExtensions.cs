namespace CallMeMaybe
{
    /// <summary>
    /// A class that contains extension methods with parsing helpers for <see cref="Maybe{T}"/>s.
    /// </summary>
    public static class MaybeParseExtensions
    {
        private static readonly MaybeTryParseWrapper<int> Int32Parser = WrapTryParse<int>(int.TryParse);
        private static readonly MaybeTryParseWrapper<long> Int64Parser = WrapTryParse<long>(long.TryParse);
        private static readonly MaybeTryParseWrapper<bool> BooleanParser = WrapTryParse<bool>(bool.TryParse);
        // TODO: Add more TryParse methods

        /// <summary>
        /// Attempts to parse the given string into an <see cref="System.Int32"/>,
        /// using the standard <see cref="int.TryParse(string, out int)"/> overload.
        /// <remarks>
        /// This is provided as a convenience method for common scenarios, but if
        /// you want to create your own parsing method (e.g. for a particular culture), it's easy to do.
        /// Just wrap a TryParse-patterned delegate in a <see cref="MaybeTryParseWrapper{T}"/>.
        /// </remarks>
        /// </summary>
        /// <param name="s">The string to parse.</param>
        /// <returns>The parsed integer if parsing was successful,
        /// or an empty <see cref="Maybe{T}"/> if this <see cref="Maybe{T}"/>
        /// if empty, or if the parse fails.</returns>
        public static Maybe<int> ParseInt32(this Maybe<string> s)
        {
            return s.SelectMany(Int32Parser.Parse);
        }

        /// <summary>
        /// Attempts to parse the given string into an <see cref="System.Int64"/>,
        /// using the standard <see cref="long.TryParse(string, out long)"/> overload.
        /// <remarks>
        /// This is provided as a convenience method for common scenarios, but if
        /// you want to create your own parsing method (e.g. for a particular culture), it's easy to do.
        /// Just wrap a TryParse-patterned delegate in a <see cref="MaybeTryParseWrapper{T}"/>.
        /// </remarks>
        /// </summary>
        /// <param name="s">The string to parse.</param>
        /// <returns>The parsed integer if parsing was successful,
        /// or an empty <see cref="Maybe{T}"/> if this <see cref="Maybe{T}"/>
        /// if empty, or if the parse fails.</returns>
        public static Maybe<long> ParseInt64(this Maybe<string> s)
        {
            return s.SelectMany(Int64Parser.Parse);
        }

        /// <summary>
        /// Attempts to parse the given string into a <see cref="bool"/>,
        /// using the standard <see cref="bool.TryParse(string, out bool)"/> overload.
        /// <remarks>
        /// This is provided as a convenience method for common scenarios, but if
        /// you want to create your own parsing method (e.g. for a particular culture), it's easy to do.
        /// Just wrap a TryParse-patterned delegate in a <see cref="MaybeTryParseWrapper{T}"/>.
        /// </remarks>
        /// </summary>
        /// <param name="s">The string to parse.</param>
        /// <returns>The parsed integer if parsing was successful,
        /// or an empty <see cref="Maybe{T}"/> if this <see cref="Maybe{T}"/>
        /// if empty, or if the parse fails.</returns>
        public static Maybe<bool> ParseBoolean(this Maybe<string> s)
        {
            return s.SelectMany(BooleanParser.Parse);
        }

        private static MaybeTryParseWrapper<T> WrapTryParse<T>(Delegates<T>.TryParse tryParse)
        {
            return new MaybeTryParseWrapper<T>(tryParse);
        }
    }
}