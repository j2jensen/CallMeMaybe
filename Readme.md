# Call Me Maybe #
A C# library to help you deal with optional values.

##Purpose##

Provides a class and a few extension methods to facilitate common operations with values that may or may not exist.

Traditionally, programmers often use `null` references to represent values that "aren't there", but the problem is that this was never their intended purpose. 

- Languages like C# don't provide a way to differentiate between reference variables that can be null and those that are guaranteed not to be.
- The inventor of null references has [apologized](http://en.wikipedia.org/wiki/Tony_Hoare#Quotations) for creating them in the first place, calling them his "billion-dollar mistake."
- This misuse of null references has spread far and wide, leading to the unfortunately-named `Nullable<>` type (which, being a value type, is never actually null), and attributes like `[CanBeNull]` and `[NotNull]` to help programmers know when they can expect a method to treat a null value as legitimate input.

All this leaves us in a position where our best hope of avoiding `NullReferenceException`s lies in trying to make sure that our reference variables are *never* null. But in that case, how do we indicate when a value is *optional*?

Well, that's where `Maybe` comes in.

##Examples##

### Basic Usage ###

Imagine you have this method:

    public string HowLuckyIs(int number)
    {
        return number == 13 ? "So lucky." : null;
    }

*This is error prone!* The person writing code to consume this method won't know that it might return a null value. They're likely to write something like this:

    bool isLucky = HowLuckyIs(number).Contains("lucky");

Instead, try using `Maybe<>` as your return value.


    public Maybe<string> HowLuckyIs(int number)
    {
        return number == 13 ? "So lucky." : null;
    }

Notice how the internal code of this method is exactly the same as before? It's *super easy* to switch to using `Maybe`. And now, consumers of your code are forced to acknowledge the possibility that you gave them *nothing*. They can do this in a few different ways:

    // `Else` will return the given value if the `Maybe` has no value.
    bool isLucky1 = HowLuckyIs(number).Else("").Contains("lucky");

    // `Select` will return a `Maybe<>`, running the lambda only if there's a value. 
    bool isLucky2 = HowLuckyIs(number).Select(n => n.Contains("lucky")).Else(false);

    // `Single` will throw an exception if there is no value.
    bool isLucky3 = HowLuckyIs(number).Single().Contains("lucky");

    // `HasValue` will simply tell you whether there is a value in the `Maybe`.
    bool isLucky4 = HowLuckyIs(number).HasValue;

Notice that `Select` and `Single` behave just they way any LINQ user would expect them to.


    public Maybe<string> HowLuckyIs(int number)
    {
        if (number == 13)
        {
            return Maybe.From("So lucky.");
        }
        return Maybe.Not;
    }

Then consumers of your code cannot use the result without recognizing that it may not be there:


This makes it impossible

So how do we use the result? Well, what do you want to have happen if there is no result? You could go with a traditional, imperative approach. 

    var luckyOne = HowLuckyIs(1);
    if (luckyOne.HasValue)
    {
        // Since Maybe<> implements IEnumerable<>, LINQ methods like .Single()
        // work exactly how you'd expect them to.
        Console.WriteLine("One is " + luckyOne.Single());
    }
    else
    {
        Console.WriteLine("One is not lucky.");
    }

But that looks awful! Let's try something a little more functional:

    // `Else()` will return the given value if the Maybe has no value.
    Console.WriteLine("One is " + HowLuckyIs(1).Else("not lucky."));

So with a few built-in utility methods, you can very easily handle the "not there" case. Now let's look at the `HowLuckyIs` method again, and see if we can't simplify it further.

For one thing, values get implicitly cast to their `Maybe<>` equivalents, and `null` is automatically treated the same as `Maybe.Not<>()`, so you *could* do this:

    public Maybe<string> HowLuckyIs2(int number)
    {
        return number == 13 ? "So lucky." : null;
    }

But we don't like `null`s, remember? Let's try this instead:

    public Maybe<string> HowLuckyIs3(int number)
    {
        return Maybe.If(number == 13, "So lucky.");
    }

### If/Else Selectors ###


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

## License ##

*Call Me Maybe* is open-sourced under the MIT License, as set forth [here](https://bitbucket.org/j2jensen/callmemaybe/wiki/License)

## Acknowledgements ##

## To Do:##

1. Finish Readme
2. Add Resharper annotations to assert that Maybe's value is NotNull.

## Authors ##

- [James Jensen](https://plus.google.com/+JamesJensenCoder)