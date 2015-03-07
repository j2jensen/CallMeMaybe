using System;
using System.Globalization;

namespace CallMeMaybe
{
    public interface ICulturedMaybeParsers
    {
        IMaybeParser<int> Int32 { get; }
        IMaybeParser<long> Int64 { get; }
    }

    /// <summary>
    /// A handful of standard parsers.
    /// If you can't find what you need here, create your own <see cref="MaybeTryParseWrapper{T}"/>.
    /// </summary>
    public class MaybeParsers : ICulturedMaybeParsers
    {
        public IMaybeParser<int> Int32 { get; private set; }
        public IMaybeParser<long> Int64 { get; private set; }
        // TODO: Provide more standard parsers for everything you'd typically find in Convert.To()

        /// <summary>
        /// Initializes the standard set of parsers.
        /// </summary>
        internal MaybeParsers()
        {
            Int64 = FromTryParse<long>(long.TryParse);
            Int32 = FromTryParse<int>(int.TryParse);
        }

        /// <summary>
        /// Initializes the parsers for the given culture. 
        /// </summary>
        internal MaybeParsers(CultureInfo culture)
        {
            Int64 = FromTryParse<long>(long.TryParse);
            Int32 = FromTryParse<int>(int.TryParse);
            Int32 = FromTryParse((string source, out int value) =>
                int.TryParse(source, NumberStyles.Integer, culture, out value));
            Int64 = FromTryParse((string source, out long value) =>
                long.TryParse(source, NumberStyles.Integer, culture, out value));
        }

        /// <summary>
        /// Creates a <see cref="MaybeTryParseWrapper{T}"/> from a method that follows the typical
        /// TryGet pattern.
        /// <example>
        /// Standard TryParse patterns can be passed in directly:
        /// <code>
        /// public void IsLucky(string str)
        /// {
        ///   var intParser = MaybeTryParseWrapper&lt;int&gt;.FromTryParse(int.TryParse);
        ///   return intParser.Parse(input).Where(i => i == 13);
        /// }
        /// </code></example>
        /// <example>
        /// You can also create a custom delegate to follow the TryParse pattern:
        /// <code>
        /// public void IsLuckyHex(string str)
        /// {
        ///   var intParser = MaybeTryParseWrapper&lt;int&gt;.FromTryParse(((string source, out int value) => 
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
        public IMaybeParser<T> FromTryParse<T>(Delegates<T>.TryParse tryParse)
        {
            return new MaybeTryParseWrapper<T>(tryParse);
        }

        /// <summary>
        /// Produces a set of standard parsers that will use the given culture.
        /// </summary>
        /// <param name="culture">The desired culture for the parsers to use.</param>
        public ICulturedMaybeParsers ForCulture(CultureInfo culture)
        {
            return new MaybeParsers(culture);
        }
    }
}