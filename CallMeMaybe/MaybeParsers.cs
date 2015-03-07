using System.Globalization;

namespace CallMeMaybe
{
    /// <summary>
    /// A handful of standard parsers.
    /// If you can't find what you need here, create your own <see cref="MaybeParser{T}"/>.
    /// </summary>
    public class MaybeParsers
    {
        public readonly MaybeParser<int> Int32 = MaybeParser<int>.FromTryParse(int.TryParse);
        public readonly MaybeParser<long> Int64 = MaybeParser<long>.FromTryParse(long.TryParse);
        // TODO: Provide more standard parsers for everything you'd typically find in Convert.To()

        /// <summary>
        /// Initializes the standard set of parsers.
        /// </summary>
        internal MaybeParsers()
        {
        }

        /// <summary>
        /// Initializes the parsers for the given culture. 
        /// </summary>
        internal MaybeParsers(CultureInfo culture)
        {
            Int32 = MaybeParser<int>.FromTryParse((string source, out int value) =>
                int.TryParse(source, NumberStyles.Integer, culture, out value));
            Int64 = MaybeParser<long>.FromTryParse((string source, out long value) =>
                long.TryParse(source, NumberStyles.Integer, culture, out value));
        }
    }
}