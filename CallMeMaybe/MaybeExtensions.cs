﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace CallMeMaybe
{
    public static class MaybeExtensions
    {
        // TODO: Fail-fast checks on all methods (specifically watch for lambdas)
        // TODO: Comment all methods and types

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

        /// <summary>
        /// Finds the given <see cref="Maybe{T}"/>s that have values, and selects those values.
        /// </summary>
        /// <typeparam name="T">The type of object held by the given <see cref="Maybe{T}"/>s.</typeparam>
        /// <param name="source">A series of <see cref="Maybe{T}"/> values.</param>
        /// <returns>
        /// Returns the underlying values of any of the given <see cref="Maybe{T}"/>s
        /// that have values.
        /// </returns>
        public static IEnumerable<T> Values<T>(this IEnumerable<Maybe<T>> source)
        {
            return source.Where(m => m.HasValue).Select(m => m.Single());
        }

        /// <summary>
        /// Finds the first element in the given <see cref="IQueryable{T}"/>,
        /// and returns it as a <see cref="Maybe{T}"/>.
        /// </summary>
        /// <remarks>
        /// There is no "LastMaybe()" method, because we cannot ensure the degree of performance that
        /// calls to methods like <see cref="Queryable.First{TSource}(System.Linq.IQueryable{TSource})"/>
        /// or <see cref="Queryable.Last{TSource}(System.Linq.IQueryable{TSource})"/> would have.
        /// As a workaround, you may change the ordering of the <see cref="IQueryable{T}"/>, and
        /// then call <see cref="FirstMaybe{T}(System.Linq.IQueryable{T})"/>.
        /// </remarks>
        /// <typeparam name="T">The type of object held by the given <see cref="IQueryable{T}"/>.</typeparam>
        /// <param name="source">An <see cref="IQueryable{T}"/> to look for an element in.</param>
        /// <returns>
        /// An empty <see cref="Maybe{T}"/> if there are no elements in the given <see cref="IQueryable{T}"/>,
        /// or if the first element is a null value.
        /// Otherwise, returns the first element in the source.
        /// </returns>
        public static Maybe<T> FirstMaybe<T>(this IQueryable<T> source)
        {
            // When we've got an IQueryable, it may be backed by a data
            // access layer or something, so we'll use Take() first, in hopes
            // that it will help the underlying system to avoid loading more
            // than one object.
            return source.Take(1).AsEnumerable().FirstMaybe();
        }

        /// <summary>
        /// Finds the first element in the given <see cref="IEnumerable{T}"/>,
        /// and returns it as a <see cref="Maybe{T}"/>.
        /// </summary>
        /// <remarks>
        /// There is no "LastMaybe()" method, because we cannot provide a good Queryable-based LastMaybe() method,
        /// and we don't want people accidentally enumerating an entire <see cref="IQueryable{T}"/> because they
        /// are accidentally calling an Enumerable-based method. 
        /// As a workaround, you can call <code>source.Reverse().FirstMaybe()</code>, but you should be aware that
        /// this could be expensive, depending on the nature of the underlying <see cref="IEnumerable{T}"/>
        /// </remarks>
        /// <typeparam name="T">The type of object held by the given <see cref="IEnumerable{T}"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to look for an element in.</param>
        /// <returns>
        /// An empty <see cref="Maybe{T}"/> if there are no elements in the given <see cref="IEnumerable{T}"/>,
        /// or if the first element is a null value.
        /// Otherwise, returns the first element in the source.
        /// </returns>
        public static Maybe<T> FirstMaybe<T>(this IEnumerable<T> source)
        {
            var enumerator = source.GetEnumerator();
            return enumerator.MoveNext()
                ? new Maybe<T>(enumerator.Current)
                : new Maybe<T>(); 
        }


        /// <summary>
        /// Finds the first element in the given <see cref="IQueryable{T}"/>,
        /// and returns it as a <see cref="Maybe{T}"/>.
        /// </summary>
        /// <remarks>
        /// There is no "LastMaybe()" method, because we cannot ensure the degree of performance that
        /// calls to methods like <see cref="Queryable.First{TSource}(System.Linq.IQueryable{TSource})"/>
        /// or <see cref="Queryable.Last{TSource}(System.Linq.IQueryable{TSource})"/> would have.
        /// As a workaround, you may change the ordering of the <see cref="IQueryable{T}"/>, and
        /// then call <see cref="FirstMaybe{T}(System.Linq.IQueryable{T})"/>.
        /// </remarks>
        /// <typeparam name="T">The type of object held by the given <see cref="IQueryable{T}"/>.</typeparam>
        /// <param name="source">An <see cref="IQueryable{T}"/> to look for an element in.</param>
        /// <returns>
        /// An empty <see cref="Maybe{T}"/> if there are no elements in the given <see cref="IQueryable{T}"/>,
        /// or if the first element is a null value.
        /// Otherwise, returns the first element in the source.
        /// </returns>
        public static Maybe<T> FirstMaybe<T>(this IQueryable<T?> source)
            where T : struct 
        {
            // When we've got an IQueryable, it may be backed by a data
            // access layer or something, so we'll use Take() first, in hopes
            // that it will help the underlying system to avoid loading more
            // than one object.
            return source.Take(1).AsEnumerable().FirstMaybe();
        }

        /// <summary>
        /// Finds the first element in the given <see cref="IEnumerable{T}"/>,
        /// and returns it as a <see cref="Maybe{T}"/>.
        /// </summary>
        /// <remarks>
        /// There is no "LastMaybe()" method, because we cannot provide a good Queryable-based LastMaybe() method,
        /// and we don't want people accidentally enumerating an entire <see cref="IQueryable{T}"/> because they
        /// are accidentally calling an Enumerable-based method. 
        /// As a workaround, you can call <code>source.Reverse().FirstMaybe()</code>, but you should be aware that
        /// this could be expensive, depending on the nature of the underlying <see cref="IEnumerable{T}"/>
        /// </remarks>
        /// <typeparam name="T">The type of object held by the given <see cref="IEnumerable{T}"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to look for an element in.</param>
        /// <returns>
        /// An empty <see cref="Maybe{T}"/> if there are no elements in the given <see cref="IEnumerable{T}"/>,
        /// or if the first element is a null value.
        /// Otherwise, returns the first element in the source.
        /// </returns>
        public static Maybe<T> FirstMaybe<T>(this IEnumerable<T?> source)
            where T : struct
        {
            var enumerator = source.GetEnumerator();
            return enumerator.MoveNext()
                ? enumerator.Current.Maybe()
                : new Maybe<T>();
        }

        // Note: I've made a conscious decision not to implement a LastMaybe 
        // method. It would open the door for someone to accidentally invoke
        // it on an IQueryable, and there's no reliable way to ensure that
        // the IQueryable only requests the last item from its data store.
        // If the user has an in-memory collection, they should be able
        // to .Reverse().FirstMaybe() it. If not, they're probably doing
        // an .OrderBy() to make .First() meaningful--they'll just have to
        // .OrderByDescending().FirstMaybe() instead.

        /// <summary>
        /// Finds the only element in this sequence, and returns it as a <see cref="Maybe{T}"/>.
        /// If there is more than one element in the sequence, an <see cref="InvalidOperationException"/>
        /// will be thrown.
        /// </summary>
        /// <typeparam name="T">The type of object held by the given <see cref="IEnumerable{T}"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to look for an element in.</param>
        /// <returns>
        /// An empty <see cref="Maybe{T}"/> if there are no elements in the given <see cref="IQueryable{T}"/>,
        /// or if the first element is a null value.
        /// Otherwise, returns the only element in the source.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the <paramref name="source"/> contains more than one element.
        /// </exception>
        public static Maybe<T> SingleMaybe<T>(this IQueryable<T> source)
        {
            // When we've got an IQueryable, it may be backed by a data
            // access layer or something, so we'll use Take() first, in hopes
            // that it will help the underlying system to avoid loading more
            // objects than necessary.
            return source.Take(2).AsEnumerable().SingleMaybe();
        }

        /// <summary>
        /// Attempts to find the only element in the given sequence.
        /// </summary>
        /// <typeparam name="T">The type of object held by the given <see cref="IEnumerable{T}"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to look for an element in.</param>
        /// <returns>
        /// An empty <see cref="Maybe{T}"/> if there are no elements in the given <see cref="IEnumerable{T}"/>,
        /// or if the first element is a null value.
        /// Otherwise, returns the only element in the source.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the <paramref name="source"/> contains more than one element.
        /// </exception>
        public static Maybe<T> SingleMaybe<T>(this IEnumerable<T> source)
        {
            var enumerator = source.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                return new Maybe<T>();
            }
            T value = enumerator.Current;
            if (enumerator.MoveNext())
            {
                throw new InvalidOperationException("Sequence contains more than one element");
            }
            return value;
        }

        /// <summary>
        /// Finds the only element in this sequence, and returns it as a <see cref="Maybe{T}"/>.
        /// If there is more than one element in the sequence, an <see cref="InvalidOperationException"/>
        /// will be thrown.
        /// </summary>
        /// <typeparam name="T">The type of object held by the given <see cref="IEnumerable{T}"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to look for an element in.</param>
        /// <returns>
        /// An empty <see cref="Maybe{T}"/> if there are no elements in the given <see cref="IQueryable{T}"/>,
        /// or if the first element is a null value.
        /// Otherwise, returns the only element in the source.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the <paramref name="source"/> contains more than one element.
        /// </exception>
        public static Maybe<T> SingleMaybe<T>(this IQueryable<T?> source)
            where T : struct 
        {
            // When we've got an IQueryable, it may be backed by a data
            // access layer or something, so we'll use Take() first, in hopes
            // that it will help the underlying system to avoid loading more
            // objects than necessary.
            return source.Take(2).AsEnumerable().SingleMaybe();
        }

        /// <summary>
        /// Attempts to find the only element in the given sequence.
        /// </summary>
        /// <typeparam name="T">The type of object held by the given <see cref="IEnumerable{T}"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to look for an element in.</param>
        /// <returns>
        /// An empty <see cref="Maybe{T}"/> if there are no elements in the given <see cref="IEnumerable{T}"/>,
        /// or if the first element is a null value.
        /// Otherwise, returns the only element in the source.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the <paramref name="source"/> contains more than one element.
        /// </exception>
        public static Maybe<T> SingleMaybe<T>(this IEnumerable<T?> source)
            where T : struct
        {
            var enumerator = source.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                return new Maybe<T>();
            }
            T? value = enumerator.Current;
            if (enumerator.MoveNext())
            {
                throw new InvalidOperationException("Sequence contains more than one element");
            }
            return value.Maybe();
        }

        // TODO: SumMaybe/MaxMaybe/MinMaybe methods
        // TODO: AggregateMaybe method

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
        /// Converts the given <see cref="Maybe{T}"/> value into a <see cref="Nullable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of object the <see cref="Nullable{T}"/> will hold.</typeparam>
        /// <param name="maybe">A nullable object to convert to a <see cref="Nullable{T}"/></param>
        /// <returns>A <see cref="Nullable{T}"/> that is "null" if <see cref="maybe"/> does not have a value,
        /// or which contains the value if it does.</returns>
        public static T? Nullable<T>(this Maybe<T> maybe) where T : struct
        {
            return maybe.HasValue ? maybe.Single() : default(T?);
        }

        // TODO: Create ElseNull method for reference types?
        // TODO: Maybes() or AsMaybes() method? https://bitbucket.org/j2jensen/callmemaybe/issue/10

    }
}
