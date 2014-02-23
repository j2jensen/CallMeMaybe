# Call Me Maybe #
A C# library to help you deal with optional values.

##Purpose##

Provides a class and a few extension methods to facilitate common operations with values that may or may not exist.

Traditionally, programmers often use `null` references to represent values that "aren't there", but the problem is that this was never their intended purpose. 

- Languages like C# don't provide a way to differentiate between reference variables that can be null and those that are guaranteed not to be.
- The inventor of null references has [apologized](http://en.wikipedia.org/wiki/Tony_Hoare#Quotations) for creating them in the first place, calling them his "billion-dollar mistake."
- This misuse of null references has spread far and wide, leading to the unfortunately-named `Nullable<>` type (which, being a value type, is never actually null), and attributes like `[CanBeNull]` and `[NotNull]` to help programmers know when they can expect a method to treat a null value as legitimate input.

All this leaves us in a position where our best hope of avoiding `NullReferenceException`s lies in trying to make sure that our reference variables are *never* null. But in that case, how do we indicate when a value is *optional*?

Well, that's where `Maybe<>` comes in.

##Examples##


## Limitations ##

### Covariance ###

Even though conceptually a `Maybe<string>` and a `Maybe<object>` should be equivalent if they both have the same backing value, I can't figure out a way to make that work for either Type Conversion or for equality operators. That leads to this unintuitive behavior:

    [Test]
    public void TestCovariantEquality()
    {
        Assert.IsTrue(Maybe.From<object>(1).Equals(Maybe.From(1)));
        Assert.IsTrue(Maybe.From(1).Equals(Maybe.From<object>(1)));
        // This is the one major limitation that I've found so far:
        // Equality Operators can't be defined in a way that makes these equal.
        Assert.IsFalse(Maybe.From<object>(1) == Maybe.From(1));
        Assert.IsFalse(Maybe.From(1) == Maybe.From<object>(1));
    }
 

## Acknowledgements ##

## To Do:##

1. Finish Readme
2. Add Resharper annotations to assert that Maybe's value is NotNull.

## Authors ##

- [James Jensen](https://plus.google.com/+JamesJensenCoder)