using System;
using System.Collections.Generic;
using System.Linq;

namespace CallMeMaybe
{
    public static class MaybeExtensions
    {
        /// <summary>
        /// Produces a <see cref="Maybe{T}"/> value that will contain the value corresponding 
        /// to the given <see cref="key"/> in the dictionary if one exists, or which will be
        /// <see cref="Maybe.Not{T}"/> otherwise.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Maybe<TValue> GetMaybe<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            TKey key)
        {
            TValue value;
            if (dictionary.TryGetValue(key, out value))
            {
                return Maybe.From(value);
            }
            return Maybe.Not<TValue>();
        }

        public static IEnumerable<TResult> SelectMany<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, Maybe<TResult>> resultSelector)
        {
            return source.SelectMany(t => resultSelector(t).AsEnumerable());
        }

        public static IEnumerable<TResult> SelectMany<TSource, TResult>(
            this Maybe<TSource> source,
            Func<TSource, IEnumerable<TResult>> resultSelector)
        {
            return source.AsEnumerable().SelectMany(resultSelector);
        }


        // TODO: TryParse methods
    }
}