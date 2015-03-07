namespace CallMeMaybe
{
    public interface IMaybeParser<T>
    {
        /// <summary>
        /// Attempts to parse the given string.
        /// </summary>
        /// <param name="source"></param>
        /// <returns>
        /// An empty <see cref="Maybe{T}"/> if parsing fails, or one containing the parsed value
        /// if parsing succeeds.
        /// </returns>
        Maybe<T> Parse(string source);
    }
    /// <summary>
    /// Converts strings into <see cref="Maybe{T}"/>s.
    /// This is useful when you have access to a TryParse-style method,
    /// and prefer to use <see cref="Maybe{T}"/> patterns instead.
    /// <example><code>
    /// public void IsLucky(string str)
    /// {
    ///   var intParser = MaybeTryParseWrapper&lt;int&gt;.FromTryParse(int.TryParse);
    ///   return intParser.Parse(input).Where(i => i == 13);
    /// }
    /// </code></example>
    /// </summary>
    /// <typeparam name="T">
    /// The Type of value you want to have in the resulting <see cref="Maybe{T}"/>s.
    /// </typeparam>
    internal sealed class MaybeTryParseWrapper<T> : IMaybeParser<T>
    {
        private readonly Delegates<T>.TryParse _tryParse;

        internal MaybeTryParseWrapper(Delegates<T>.TryParse tryParse)
        {
            _tryParse = tryParse;
        }


        /// <summary>
        /// Attempts to parse the given string.
        /// </summary>
        /// <param name="source"></param>
        /// <returns>
        /// An empty <see cref="Maybe{T}"/> if parsing fails, or one containing the parsed value
        /// if parsing succeeds.
        /// </returns>
        public Maybe<T> Parse(string source)
        {
            // TODO: Consider providing a way for users to specify exception-handling behavior.
            // (For now we're assuming that exceptions are not expected, and we'd better fail-fast
            // if one occurs.)
            T value;
            if (_tryParse(source, out value))
            {
                return value;
            }
            return new Maybe<T>();
        }
    }

    /// <summary>
    /// This is just a place for us to keep generically-typed delegates, where
    /// they have public accessibility."
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Delegates<T>
    {
        /// <summary>
        /// A typical TryParse-patterned method, which takes in a source string,
        /// and returns a boolean to represent whether parsing succeeded.
        /// </summary>
        public delegate bool TryParse(string source, out T value);
    }
}