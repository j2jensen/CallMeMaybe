using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace CallMeMaybe
{

    // TODO: Fail-fast checks on all methods (specifically watch for lambdas)
    // TODO: Comment all methods and types
    // TODO: Investigate [EditorBrowsable] and [DebuggerDisplay] attributes
    // TODO: Investigate Resharper annotations: Pure, InstantHandle, and NotNull
    /// <summary>
    /// Container for an optional value that may or may not exist.
    /// </summary>
    /// <remarks>
    /// Using a <see cref="Maybe{T}"/> for optional values will help you to catch
    /// problems at compile-time that may otherwise have created
    /// <see cref="NullReferenceException"/>s at runtime.
    /// </remarks>
    /// <typeparam name="T">The type of value that this may or may not contain.</typeparam>
    public struct Maybe<T> : IEquatable<Maybe<T>>, IMaybe
    {
        private readonly T _value;
        private readonly bool _hasValue;

        /// <summary>
        /// Gets whether or not this <see cref="Maybe{T}"/> contains a value.
        /// </summary>
        public bool HasValue
        {
            get { return _hasValue; }
        }

        /// <summary>
        /// Constructs a <see cref="Maybe{T}"/> that contains the given value, or
        /// an empty <see cref="Maybe{T}"/> if the value is null.
        /// </summary>
        /// <param name="value">
        /// The value the <see cref="Maybe{T}"/> should contain. If null, the
        /// <see cref="Maybe{T}"/> will not contain a value.
        /// </param>
        public Maybe(T value)
        {
            // Since the whole purpose of this class is to avoid null values,
            // we're going to treat them as if they are "Not".
            _hasValue = !ReferenceEquals(null, value);
            _value = value;
        }

        /// <summary>
        /// Attempts to get the value.
        /// </summary>
        /// <param name="value">
        /// An out parameter that will be set to the value inside this <see cref="Maybe{T}"/>
        /// if it has one, or the default value for type <see cref="T"/> if not.
        /// </param>
        /// <returns>True if this <see cref="Maybe{T}"/> has a value, false otherwise.</returns>
        bool IMaybe.TryGetValue(out object value)
        {
            value = _value;
            return _hasValue;
        }

        /// <summary>
        /// Implicitly casts a value of type <see cref="T"/> to a <see cref="Maybe{T}"/>
        /// </summary>
        /// <param name="value">
        /// The value the <see cref="Maybe{T}"/> should contain. If null, the
        /// <see cref="Maybe{T}"/> will not contain a value.
        /// </param>
        /// <returns>
        /// A <see cref="Maybe{T}"/> that contains the given value, or
        /// an empty <see cref="Maybe{T}"/> if the value is null.
        /// </returns>
        public static implicit operator Maybe<T>(T value)
        {
            // Slight corner case: since Maybes are objects themselves,
            // C# will try to do an implicit conversion from a Maybe into
            // a Maybe<object>.
            // We need to unwrap the inner value of the other Maybe, and
            // use that as the value that this Maybe intends to be.
            if (typeof (T) == typeof (object) && value is IMaybe)
            {
                var otherMaybe = (IMaybe) value;
                object otherValue;

                if (otherMaybe.TryGetValue(out otherValue))
                {
                    value = (T) otherValue;
                }
            }

            return new Maybe<T>(value);
        }

        /// <summary>
        /// Implicitly casts a <see cref="MaybeNot"/> to a <see cref="Maybe{T}"/>
        /// </summary>
        /// <remarks>The existence of this operator allows consumers to use <see cref="Maybe.Not"/>
        /// in many cases where they would otherwise have needed to explicitly (and needlessly) 
        /// specify the type via <see cref="Maybe{T}.Not"/></remarks>
        /// <param name="maybeNot">A <see cref="MaybeNot"/> value.</param>
        /// <returns>A <see cref="Maybe{T}"/> without a value.</returns>
        public static implicit operator Maybe<T>(MaybeNot maybeNot)
        {
            return default(Maybe<T>);
        }
        // TODO: Consider implicit conversion from Maybe<Maybe<T>>
        // Note: I tried the conversion mentioned above, but it broke Resharper. Let's wait until that
        // bug gets fixed before working on it again.

        /// <summary>
        /// Gets a <see cref="Maybe{T}"/> without a value.
        /// </summary>
        public static Maybe<T> Not
        {
            get { return default (Maybe<T>); }
        }

        #region LINQ Methods

        [Pure]
        public Maybe<TValue> Select<TValue>(Func<T, TValue> selector)
        {
            return _hasValue ? selector(_value) : Maybe<TValue>.Not;
        }

        /// <summary>
        /// Filters this <see cref="Maybe{T}"/> by the given criteria.
        /// </summary>
        /// <param name="criteria">A filtering function.
        /// This will only be invoked if this <see cref="Maybe{T}"/> has a value.
        /// </param>
        /// <returns>
        /// A <see cref="Maybe{T}"/> with the same value as this one if this
        /// <see cref="Maybe{T}"/> has a value that matches the given criteria.
        /// Otherwise, an empty <see cref="Maybe{T}"/>.
        /// </returns>
        public Maybe<T> Where(Func<T, bool> criteria)
        {
            return _hasValue && criteria(_value) ? this : default(Maybe<T>);
        }

        public Maybe<TResult> SelectMany<TResult>(
            Func<T, Maybe<TResult>> resultSelector)
        {
            return _hasValue ? resultSelector(_value) : default(Maybe<TResult>);
        }

        public Maybe<TResult> SelectMany<TOther, TResult>(Func<T, Maybe<TOther>> otherSelector,
            Func<T, TOther, TResult> resultSelector)
        {
            if (_hasValue)
            {
                var otherMaybe = otherSelector(_value);
                if (otherMaybe._hasValue)
                {
                    return resultSelector(_value, otherMaybe._value);
                }
            }
            return default(Maybe<TResult>);
        }

        public T Single()
        {
            return ToList().Single();
        }

        public List<T> ToList()
        {
            return _hasValue ? new List<T>(1) { _value } : new List<T>(0);
        }

        // TODO: Any() method

        #endregion

        // TODO: Consider making this method return `this` so that we can chain calls.
        /// <summary>
        /// Performs the given action if this <see cref="Maybe{T}"/> has a value.
        /// Otherwise, this will do nothing at all.
        /// </summary>
        /// <param name="action">
        /// The action to perform if this <see cref="Maybe{T}"/> has a value.
        /// (The value will be given as the action's parameter).
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the <see cref="action"/> parameter is null.
        /// </exception>
        public void Do(Action<T> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            if (_hasValue)
            {
                action(_value);
            }
        }

        // TODO: Consider an ElseDo() method: would this really add anything?

        // TODO: Consider an Or() method (switch to another Maybe<> value if no value). Are there any real use cases for this?

        // TODO: Consider an ElseIf() method, so we can chain Maybe.If(...).ElseIf(...).Else(...);

        /// <summary>
        /// Gets the value of this <see cref="Maybe{T}"/>, or 
        /// produces a fallback value if this <see cref="Maybe{T}"/> has no value,
        /// by invoking the given function.
        /// </summary>
        /// <param name="getValueIfNot">
        /// A function which will be invoked to produce a fallback value if this
        /// <see cref="Maybe{T}"/> has no value.</param>
        /// <returns>
        /// Either the value contained by this <see cref="Maybe{T}"/> (if it has one), 
        /// or the value produced by invoking <see cref="getValueIfNot"/> otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the <see cref="getValueIfNot"/> parameter is null.
        /// </exception>
        public T Else(Func<T> getValueIfNot)
        {
            if (getValueIfNot == null)
            {
                throw new ArgumentNullException("getValueIfNot");
            }
            return _hasValue ? _value : getValueIfNot();
        }

        /// <summary>
        /// Gets the value of this <see cref="Maybe{T}"/>, or 
        /// returns a given fallback value if this <see cref="Maybe{T}"/> has no value.
        /// </summary>
        /// <param name="valueIfNot">
        /// The fallback value to return if this <see cref="Maybe{T}"/> has no value.</param>
        /// <returns>
        /// Either the value contained by this <see cref="Maybe{T}"/> (if it has one), 
        /// or <see cref="valueIfNot"/> otherwise.
        /// </returns>
        public T Else(T valueIfNot)
        {
            return _hasValue ? _value : valueIfNot;
        }

        /// <summary>
        /// Gets a string representation of this <see cref="Maybe{T}"/>.
        /// </summary>
        /// <returns>
        /// An empty string ("") if this <see cref="Maybe{T}"/> has no value, or
        /// the result of calling <see cref="object.ToString"/> on the contained
        /// value if there is one.
        /// </returns>
        public override string ToString()
        {
            return _hasValue ? _value.ToString() : "";
        }

        #region Equality

        bool IEquatable<Maybe<T>>.Equals(Maybe<T> other)
        {
            return Equals(other);
        }


        public override bool Equals(object obj)
        {
            // Maybe values never equal `null`--that's sort of the point.
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            // Maybe values can be compared with other Maybe values.
            var maybe = obj as IMaybe;
            if (maybe != null)
            {
                object value;
                if (!maybe.TryGetValue(out value))
                {
                    // If the other one doesn't have a value, then we're
                    // only "equal" if this one doesn't either.
                    return !HasValue;
                }
                return Equals(_value, value);
            }
            // If it's not a Maybe, then maybe it's a T
            // Maybe.From(1) == 1
            if (obj is T)
            {
                return _hasValue && _value.Equals((T)obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_hasValue.GetHashCode()*397) ^ EqualityComparer<T>.Default.GetHashCode(_value);
            }
        }

        public static bool operator ==(Maybe<T> left, Maybe<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Maybe<T> left, Maybe<T> right)
        {
            return !left.Equals(right);
        }

        #endregion

        /// <summary>
        /// Determines whether this <see cref="Maybe{T}"/>'s value meets the given
        /// criteria.
        /// </summary>
        /// <param name="criteria">
        /// The criteria to check this <see cref="Maybe{T}"/>'s value against.
        /// This will only be invoked if this <see cref="Maybe{T}"/> has a value.
        /// </param>
        /// <returns>
        /// True if this <see cref="Maybe{T}"/> has a value that matches the given criteria.
        /// False otherwise.
        /// </returns>
        public bool Is(Func<T, bool> criteria)
        {
            return Where(criteria).HasValue;
        }
    }

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

    /// <summary>
    /// A non-generic interface implemented by <see cref="Maybe{T}"/> values, to allow basic
    /// operations against a non-generically-typed <see cref="Maybe{T}"/>.
    /// </summary>
    public interface IMaybe
    {
        bool HasValue { get; }
        bool TryGetValue(out object value);
    }

    /// <summary>
    /// A non-generic helper class for producing <see cref="Maybe{T}"/> values declaratively.
    /// </summary>
    public static class Maybe
    {
        /// <summary>
        /// Creates a <see cref="Maybe{T}"/> based on the given value.
        /// </summary>
        /// <typeparam name="T">
        /// The type of value that this <see cref="Maybe{T}"/> may contain.
        /// </typeparam>
        /// <param name="value">
        /// The value the <see cref="Maybe{T}"/> should contain. If null, the
        /// <see cref="Maybe{T}"/> will not contain a value.
        /// </param>
        /// <returns>
        /// A <see cref="Maybe{T}"/> that contains the given value, or
        /// an empty <see cref="Maybe{T}"/> if the value is null
        /// </returns>
        /// <remarks>This is the same as calling the Maybe constructor, but `Maybe.From(val)` often reads better in code.</remarks>
        public static Maybe<T> From<T>(T value)
        {
            return new Maybe<T>(value);
        }

        /// <summary>
        /// Creates a <see cref="Maybe{T}"/> based on the given value.
        /// </summary>
        /// <typeparam name="T">
        /// The type of value that this <see cref="Maybe{T}"/> may contain.
        /// </typeparam>
        /// <param name="value">
        /// The value the <see cref="Maybe{T}"/> should contain. If null, the
        /// <see cref="Maybe{T}"/> will not contain a value.
        /// </param>
        /// <returns>
        /// A <see cref="Maybe{T}"/> that contains the given value, or
        /// an empty <see cref="Maybe{T}"/> if the value is null
        /// </returns>
        /// <remarks>This overload automatically converts nullables to <see cref="Maybe{T}"/>s.</remarks>
        public static Maybe<T> From<T>(T? value) where T : struct 
        {
            return value.HasValue ? new Maybe<T>(value.Value) : new Maybe<T>();
        }

        // TODO: Figure out how to do the equivalent of Not, with anonymous types?

        /// <summary>
        /// A value that can be implicitly cast to a <see cref="Maybe{T}"/> with no value.
        /// </summary>
        /// <remarks>
        /// If you're in a context where implicit casting doesn't make sense, try 
        /// <see cref="Maybe{T}.Not"/> instead.
        /// </remarks>
        public static readonly MaybeNot Not = new MaybeNot();

        /// <summary>
        /// Produces a <see cref="Maybe{T}"/> which contains the given value if the given
        /// condition is met, or a <see cref="Maybe{T}"/> with no value otherwise.
        /// </summary>
        /// <typeparam name="T">
        /// The type of value that the returned <see cref="Maybe{T}"/> may contain.
        /// </typeparam>
        /// <param name="condition">
        /// A boolean representing whether the given value should be
        /// put into the returned <see cref="Maybe{T}"/>.
        /// </param>
        /// <param name="valueIfTrue">
        /// The value the <see cref="Maybe{T}"/> should contain if the given condition is true. 
        /// If null, the <see cref="Maybe{T}"/> will not contain a value.
        /// </param>
        /// <returns>
        /// A <see cref="Maybe{T}"/> with no value if the given condition is false, or if the
        /// given value is null. Otherwise, a <see cref="Maybe{T}"/> containing the given value.
        /// </returns>
        public static Maybe<T> If<T>(bool condition, T valueIfTrue)
        {
            return condition ? valueIfTrue : Maybe<T>.Not;
        }

        /// <summary>
        /// Produces a <see cref="Maybe{T}"/> which contains the value produced by invoking
        /// the given function if the given condition is met, or a <see cref="Maybe{T}"/> with
        /// no value otherwise.
        /// </summary>
        /// <typeparam name="T">
        /// The type of value that the returned <see cref="Maybe{T}"/> may contain.
        /// </typeparam>
        /// <param name="condition">
        /// A boolean representing whether the given function should be invoked to produce a 
        /// value.
        /// </param>
        /// <param name="valueIfTrue">
        /// A function that will be invoked to produce the value that the <see cref="Maybe{T}"/> 
        /// should contain if the given condition is true. 
        /// </param>
        /// <returns>
        /// A <see cref="Maybe{T}"/> with no value if the given condition is false, or if the
        /// given function returns null. Otherwise, a <see cref="Maybe{T}"/> containing the given value.
        /// </returns>
        // TODO: Throw ArgumentNullException if function is null, then update these comments.
        public static Maybe<T> If<T>(bool condition, Func<T> valueIfTrue)
        {
            return condition ? valueIfTrue() : Maybe<T>.Not;
        }
    }
}
