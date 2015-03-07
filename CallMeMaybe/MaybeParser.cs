namespace CallMeMaybe
{
    /// <summary>
    /// Converts strings into <see cref="Maybe{T}"/>s.
    /// This is useful when you have access to a TryParse-style method,
    /// and prefer to use <see cref="Maybe{T}"/> patterns instead.
    /// <example><code>
    /// public void IsLucky(string str)
    /// {
    ///   var intParser = MaybeParser&lt;int&gt;.FromTryParse(int.TryParse);
    ///   return intParser.Parse(input).Where(i => i == 13);
    /// }
    /// </code></example>
    /// </summary>
    /// <typeparam name="T">
    /// The Type of value you want to have in the resulting <see cref="Maybe{T}"/>s.
    /// </typeparam>
    public class MaybeParser<T>
    {
        private readonly Delegates<T>.TryParse _tryParse;

        protected MaybeParser(Delegates<T>.TryParse tryParse)
        {
            _tryParse = tryParse;
        }

        /// <summary>
        /// Creates a <see cref="MaybeParser{T}"/> from a method that follows the typical
        /// TryGet pattern.
        /// <example>
        /// Standard TryParse patterns can be passed in directly:
        /// <code>
        /// public void IsLucky(string str)
        /// {
        ///   var intParser = MaybeParser&lt;int&gt;.FromTryParse(int.TryParse);
        ///   return intParser.Parse(input).Where(i => i == 13);
        /// }
        /// </code></example>
        /// <example>
        /// You can also create a custom delegate to follow the TryParse pattern:
        /// <code>
        /// public void IsLuckyHex(string str)
        /// {
        ///   var intParser = MaybeParser&lt;int&gt;.FromTryParse(((string source, out int value) => 
        ///     int.TryParse(source, NumberStyles.HexNumber, CultureInfo.CurrentUICulture, out value)));
        ///   return intParser.Parse(input).Where(i => i == 13);
        /// }
        /// </code></example>
        /// </summary>
        /// <param name="tryParse">
        /// A method or delegate that follows the typical TryParse pattern.
        /// The resulting parser will invoke this, and return an empty <see cref="Maybe{T}"/> if 
        /// it returns false.
        /// </param>
        /// <returns>A parser that is ready to produce <see cref="Maybe{T}"/>s from strings.</returns>
        public static MaybeParser<T> FromTryParse(Delegates<T>.TryParse tryParse)
        {
            return new MaybeParser<T>(tryParse);
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
    /// they still have public accessibility, but they won't appear in intellisense
    /// when you type "MaybeParser&lt;int&gt;."
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