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

### Dictionaries ###

When working with dictionaries, try using the `.GetMaybe(key)` extension method instead of the dictionary's indexer or `.TryGetValue()`:

    var carsByOwner = GetCars().ToDictionary(c => c.OwnerPersonId);
    var activePeopleWithCars =
        from p in GetPeople()
        where p.IsActive
        from car in carsByOwner.GetMaybe(p.PersonId)
        select new {owner = p, car};

## Limitations ##

### Covariance ###

Even though conceptually a `Maybe<Parent>` and a `Maybe<Child>` should be equivalent if they both have the same backing `Child` value, I can't figure out a way to make that work using either implicit type conversion or equality operators. That means that while `.Equals()` works just fine, the `==` and `!=` operators cannot be applied to `Maybe` objects of different types:

    [Test]
    public void TestCovariantInheritedClassEquality()
    {
        var child = new Child();
        Assert.IsTrue(Maybe.From<Parent>(child).Equals(Maybe.From(child)));
        Assert.IsTrue(Maybe.From(child).Equals(Maybe.From<Parent>(child)));
        // Limitation: any attempt to do a covariant equality check 
        // results in a compiler error.
        /*
        Assert.IsTrue(Maybe.From<Child>(child) == Maybe.From<Parent>(child));
        Assert.IsFalse(Maybe.From<Child>(child) == Maybe.From<Parent>(new Child()));
        Assert.IsFalse(Maybe.From<Child>(child) != Maybe.From<Parent>(child));
        Assert.IsTrue(Maybe.From<Child>(child) != Maybe.From<Parent>(new Child()));
        */
    }

    private class Parent
    {
    }

    private class Child : Parent
    {
    }


## Acknowledgements ##

## To Do:##

1. Finish Readme
2. Add Resharper annotations to assert that Maybe's value is NotNull.

## Authors ##

- [James Jensen](https://plus.google.com/+JamesJensenCoder)