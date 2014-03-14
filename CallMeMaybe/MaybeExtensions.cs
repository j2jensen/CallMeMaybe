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

        public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, Maybe<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
        {
            return source.SelectMany(t => collectionSelector(t).ToList(), resultSelector);
        }

        public static IEnumerable<TResult> SelectMany<TSource, TResult>(
            this Maybe<TSource> source,
            Func<TSource, IEnumerable<TResult>> resultSelector)
        {
            return source.ToList().SelectMany(resultSelector);
        }

        // TODO: MaybeFirst/MaybeLast/MaybeSingle methods
        // TODO: MaybeSum/MaybeMax/MaybeMin methods
        // TODO: MaybeAggregate method

        // TODO: Consider WhereHasValue method on IEnumerable<Maybe<T>>, returns IEnumerable<T>
        // (Would this just be the same as SelectMany(i => i)?)

        // TODO: Consider extension method to convert any object into a Maybe
        // On one hand, that's a huge scope to open up an extension method on, and in many cases
        // an implicit conversion is sufficient. But it could be handy in some use cases:
        // var maybeAnon = new {a = "foo"}.Maybe();
        // var str = val.Maybe().Select(v => v.ToString()).Else("None");
        // Does the first case make sense? How often will programmers be _optionally_ producing
        // an anonymous type?
        // Do we even want to encourage this last case, since it's basically guaranteed to be
        // used on a null value, rather than explicitly declaring `val` to be a Maybe?
        // Are these cases common enough to merit an extension, or should people just use
        // Maybe.From()?

        // TODO: TryParse methods

        // TODO: ElseDefault, only where T : struct
    }
}