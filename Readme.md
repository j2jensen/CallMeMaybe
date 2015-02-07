# Call Me Maybe #
A C# library to help you deal with optional values. It is open-sourced here on [BitBucket](https://bitbucket.org/j2jensen/callmemaybe), and is available from [NuGet](https://www.nuget.org/packages/CallMeMaybe/).

##Purpose##

Provides a class and a few extension methods to facilitate common operations with values that may or may not exist.

Traditionally, C# programmers often use `null` references to represent values that "aren't there", but the problem is that this was never their intended purpose. 

- The inventor of null references has [apologized](http://en.wikipedia.org/wiki/Tony_Hoare#Quotations) for creating them in the first place, calling them his "billion-dollar mistake."
- This misuse of null references has spread far and wide, leading to the unfortunately-named `Nullable<>` type (which, being a value type, is never actually null), and attributes like `[CanBeNull]` and `[NotNull]` to help programmers know when they can expect a method to treat a null value as legitimate input.
- Many languages provide a way to deal with optional values that doesn't involve null references (e.g. [F#](https://msdn.microsoft.com/en-us/library/dd233245.aspx), [Scala](http://www.scala-lang.org/api/current/index.html#scala.Option), [Haskell](http://hackage.haskell.org/packages/archive/base/4.2.0.1/doc/html/Data-Maybe.html), and recently even [Java](http://docs.oracle.com/javase/8/docs/api/java/util/Optional.html)). This is one area where C# has lagged behind other languages.

Our best hope of avoiding `NullReferenceException`s lies in trying to make sure that our reference variables are *never* null. But in that case, how do we indicate when a reference value is *optional*?

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

    // `Is` will tell you whether the value matches another value or criteria.
    bool isLucky1 = HowLuckyIs(number).Is("So lucky.");
    bool isLucky2 = HowLuckyIs(number).Is(s => s.Contains("lucky")); 

    // `Else` will return the given value if the `Maybe` has no value.
    bool isLucky3 = HowLuckyIs(number).Else("").Contains("lucky");

    // `Select` will return a `Maybe<>`, running the lambda only if there's a value. 
    bool isLucky4 = HowLuckyIs(number).Select(n => n.Contains("lucky")).Else(false);

    // `Single` will throw an exception if there is no value.
    bool isLucky5 = HowLuckyIs(number).Single().Contains("lucky");

    // `HasValue` will simply tell you whether there is a value in the `Maybe`.
    bool isLucky6 = HowLuckyIs(number).HasValue;

    // `Do` will only do something if the `Maybe` has a value.
    HowLuckyIs(number).Do(n => Console.WriteLine(n.Contains("lucky")));

Notice that `Select` and `Single` behave just they way any LINQ user would expect them to. The same is true of several other LINQ operators, which makes `Maybe` work very smoothly in LINQ syntax:

    var luckyNumbers =
        from n in Enumerable.Range(1, 20)
        from s in HowLuckyIs(n)
        where s.Contains("lucky")
        select new {number = n, howLucky = s};

Now let's look at the `HowLuckyIs` method again. It was easy enough to rely on implicit casting, but what if we want to be more explicit, and avoid using `null` in our code?

    public Maybe<string> HowLuckyIs(int number)
    {
        return number == 13 ? Maybe.From("So lucky.") : Maybe<string>.Not;
    }


But that's way too verbose. Let's try this instead:

    public Maybe<string> HowLuckyIs(int number)
    {
        return Maybe.If(number == 13, "So lucky.");
    }


### Use Cases ###

Don't limit your usage of `Maybe<>` to return types. `Maybe<>` also works great for optional parameters, and any property that doesn't get set by an object's constructor. Because `Maybe<>` is a value type, if it never gets initialized, it will always be *empty* rather than `null`.

    // Can be called like this: CallMe("123-456-7890")
    public void CallMe(Maybe<string> phoneNumber = default(Maybe<string>))
    {
        ...
    }

    public class Callee
    {
        Maybe<string> PhoneNumber {get; set;}
    }

    

### Maybe.Not ###

`Maybe.Not` is a special value that implicitly casts to an empty `Maybe<>` object. However, because it requires an implicit cast, you may sometimes need to use `Maybe<T>.Not`. You can also use `new Maybe<T>()` or `default(Maybe<T>)`. Take your pick, but be consistent.


### Dictionaries ###

When working with dictionaries, try using the `.GetMaybe(key)` extension method instead of the dictionary's indexer or `.TryGetValue()`:

    var carsByOwner = GetCars().ToDictionary(c => c.OwnerPersonId);
    var activePeopleWithCars =
        from p in GetPeople()
        where p.IsActive
        from car in carsByOwner.GetMaybe(p.PersonId)
        select new {owner = p, car};

## Limitations and Caveats ##

### No Implicit Casting of `Nullable<>`s ###

It's not possible to create an implicit casting operator from `T?` (i.e. `Nullable<T>`) to `Maybe<T>` without restricting `Maybe<>`s to value types.

    public string PrintDate(Maybe<DateTime> dateTime) { ... }

    DateTime? d = GetDate();
    // This won't work
    PrintDate(d);
    // Instead, try this:
    PrintDate(d.Maybe());

The `Maybe()` extension method is available on all `Nullable<>` types, and there is a corresponding `Nullable()` method on any `Maybe<T>` where T is a value type.

### Covariance and Equality ###

`Maybe<T>` is intended to be a generically-typed compile-time aid, and can yield unexpected behavior when they are cast as `object`s. If you put `Nullable<int>`s into a `HashSet<object>`, .NET will automatically convert those values into either `int`s or `null` values. However, mere mortals don't have access to the magic required to make this happen. So, for consistency, a `Maybe<T>` `.Equals()` another object only if that other object is *of the same `Maybe<T>` type* and has the same value.

`Maybe.From(5) == 5` and `5 == Maybe.From(5)` will yield `true` because `5` is implicitly cast as a `Maybe<int>`. Also, `Maybe.From((string)null) == null` will yield `true` because `null` can be implicitly cast to a `string`, which then gets implicitly cast to a `Maybe<string>`. However, using the `.Equals(object)` method will not match this behavior. `Maybe.From(5).Equals(5)` yields `false` because `5.Equals(Maybe.From(5))` cannot be true.  

None of this will be a problem if you only use `Maybe<>` values as compile-time constructs. Don't cast `Maybe<T>`s as `object`s, don't try to compare them to types (even other `Maybe<T>` types), and use the `.Is()` method or the `==` and `!=` operators, rather than `.Equals(object)`.


### Third-Party Support ###

Unfortunately, `Maybe<>` is not a part of the BCL (though it probably should be). That means that there's not much support for it in third-party frameworks like Entity Framework. My hope is to add some plugin packages for frameworks that are extensible (e.g. ASP.NET MVC model binding). But there will be some places where other frameworks just won't know what to do with it.


## License ##

*Call Me Maybe* is open-sourced under the MIT License, as set forth [here](https://bitbucket.org/j2jensen/callmemaybe/wiki/License).

## Acknowledgements ##

- I drew on some of the best ideas I found in other similar frameworks:
    - [Functional.Maybe](https://github.com/AndreyTsvetkov/Functional.Maybe)
    - [Strilanc.May](https://github.com/Strilanc/May) - [Blog post](http://twistedoakstudios.com/blog/Post1130_when-null-is-not-enough-an-option-type-for-c)
- Special thanks to friends, and anyone else, who has given me feedback and suggestions on this project.
- Thanks to *you* for reading this and (hopefully) using the framework. Issues, ideas, and pull requests are welcome.

## Authors ##

- [James Jensen](https://plus.google.com/+JamesJensenCoder)
- (Contribute to add your name here)