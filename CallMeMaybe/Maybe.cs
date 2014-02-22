﻿using System;
using System.Collections.Generic;

namespace CallMeMaybe
{
    public struct Maybe<T> : IEquatable<Maybe<T>>, IMaybe<T>
    {
        private readonly T _value;
        private readonly bool _hasValue;

        public bool HasValue
        {
            get { return _hasValue; }
        }

        public Maybe(T value)
        {
            _hasValue = true;
            _value = value;
        }

        #region Equality

        public bool Equals(Maybe<T> other)
        {
            return _hasValue == other._hasValue && 
                (!_hasValue || EqualityComparer<T>.Default.Equals(_value, other._value));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            var maybe = obj as IMaybe;
            if (maybe == null) return false;
            if (!maybe.HasValue)
            {
                return !HasValue;
            }
            return Equals(_value, ((Maybe<T>) obj)._value);
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
    }

    public interface IMaybe
    {
        bool HasValue { get; }
    }

    public interface IMaybe<out T> : IMaybe
    {
    }

    public static class Maybe
    {
        public static Maybe<T> From<T>(T value)
        {
            return new Maybe<T>(value);
        }

        public static Maybe<T> Empty<T>()
        {
            return new Maybe<T>();
        }
    }
}