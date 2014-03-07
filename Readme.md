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

### Basic Usage ###

Try using `Maybe<>` when your method may or may not return a value.

    public Maybe<string> HowLuckyIs(int number)
    {
        if (number == 13)
        {
            return Maybe.From("So lucky.");
        }
        return Maybe.Not<string>();
    }

Then consumers of your code cannot use the result without recognizing that it may not be there:

    // Next line won't compile
    // Assert.AreEqual(3, HowLuckyIs(13).IndexOf("lucky"));

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

*Call Me Maybe* is open-sourced under the MIT License, as set forth below:

> ### The MIT License (MIT) ###
> 
> Copyright (c) 2014 James K. Jensen
> 
> Permission is hereby granted, free of charge, to any person obtaining a copy
> of this software and associated documentation files (the "Software"), to deal
> in the Software without restriction, including without limitation the rights
> to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
> copies of the Software, and to permit persons to whom the Software is
> furnished to do so, subject to the following conditions:
> 
> The above copyright notice and this permission notice shall be included in
> all copies or substantial portions of the Software.
> 
> THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
> IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
> FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
> AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
> LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
> OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
> THE SOFTWARE.

## Acknowledgements ##

## To Do:##

1. Finish Readme
2. Add Resharper annotations to assert that Maybe's value is NotNull.

## Authors ##

- [James Jensen](https://plus.google.com/+JamesJensenCoder)