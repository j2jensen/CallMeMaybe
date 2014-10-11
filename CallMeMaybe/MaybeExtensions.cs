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
        /// empty otherwise.
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
                return new Maybe<TValue>(value);
            }
            return new Maybe<TValue>();
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

        /// <summary>
        /// Converts the given nullable value into a <see cref="Maybe{T}"/>
        /// </summary>
        /// <typeparam name="T">The type of object the <see cref="Maybe{T}"/> will hold.</typeparam>
        /// <param name="nullable">A nullable object to convert to a <see cref="Maybe{T}"/></param>
        /// <returns>A <see cref="Maybe{T}"/> that is empty if <see cref="nullable"/> does not have a value,
        /// or which contains the value if it does.</returns>
        /// <remarks>This is useful because nullables cannot be implicitly cast to <see cref="Maybe{T}"/>s,
        /// so we need an easy shortcut for passing a <see cref="Nullable{T}"/> into a method that
        /// takes a <see cref="Maybe{T}"/>.</remarks>
        public static Maybe<T> Maybe<T>(this T? nullable) where T : struct
        {
            return CallMeMaybe.Maybe.From(nullable);
        }


        /// <summary>
        /// Converts the given nullable value into a <see cref="Maybe{T}"/>
        /// </summary>
        /// <typeparam name="T">The type of object the <see cref="Maybe{T}"/> will hold.</typeparam>
        /// <param name="maybe">A nullable object to convert to a <see cref="Maybe{T}"/></param>
        /// <returns>A <see cref="Nullable{T}"/> that is "null" if <see cref="maybe"/> does not have a value,
        /// or which contains the value if it does.</returns>
        public static T? Nullable<T>(this Maybe<T> maybe) where T : struct
        {
            return maybe.HasValue ? maybe.Single() : default(T?);
        }

        // TODO: TryParse methods

        // TODO: ElseDefault, only where T : struct
    }
}