<Query Kind="Program">
  <Reference Relative="..\CallMeMaybe\bin\Release\CallMeMaybe.dll">C:\Work\CallMeMaybe\CallMeMaybe\bin\Release\CallMeMaybe.dll</Reference>
  <Namespace>CallMeMaybe</Namespace>
</Query>

// This method is bad--the compiler will not complain if you say:
// var len = BadHowLuckyIs(7).Length
public string BadHowLuckyIs(int number)
{
    return number == 13 ? "So lucky." : null;
}

// By wrapping my return type in a `Maybe<>`, I am forcing consumers
// to deal with the possibility that my result might not be there.
public Maybe<string> HowLuckyIs(int number)
{
    return number == 13 ? "So lucky." : null;
}

// The consumer can handle the results in a number of different ways.
void TestNumber(int number)
{
    Console.WriteLine("Testing number " + number);

    // `Is` will tell you whether the value matches another value or criteria.
    HowLuckyIs(number).Is("So lucky.")
        .Dump("Is 1");
    HowLuckyIs(number).Is(s => s.Contains("lucky"))
        .Dump("Is 2"); 

    // `Else` will return the given value if the `Maybe` has no value.
    HowLuckyIs(number).Else("").Contains("lucky")
        .Dump("Else");
    
    // `Select` will return a `Maybe<>`, running the lambda only if there's a value. 
    HowLuckyIs(number).Select(n => n.Contains("lucky")).Else(false)
        .Dump("Select");
    
    // `HasValue` will simply tell you whether there is a value in the `Maybe`.
    HowLuckyIs(number).HasValue.Dump("HasValue");
    
    // `Do` will only do something if the `Maybe` has a value.
    HowLuckyIs(number).Do(
        n => Console.WriteLine("Do: " + n.Contains("lucky")));
    
    // `Single` will throw an exception if there is no value.
    HowLuckyIs(number).Single().Contains("lucky")
        .Dump("Single");
}

// Go ahead and run this script to see if the output is what you think it will be.
void Main()
{
    // Try a lucky number
    TestNumber(13);
    // An unlucky number.
    // (This will throw an exception when it hits the "Single()" call.)
    TestNumber(7);
}