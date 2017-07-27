using System;
using System.Linq;
using Xunit;

namespace CallMeMaybe.UnitTests
{
    public class Examples
    {
        [Fact]
        public void TestHowLuckyIs()
        {
            // Next line won't compile
            // Assert.AreEqual(3, HowLuckyIs(13).IndexOf("lucky"));

            // `Maybe<>` implements IEnumerable, so LINQ methods like .Single() will
            // behave the way any developer would expect them to.
            Assert.Equal(3, HowLuckyIs(13).Single().IndexOf("lucky", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void TestHowLuckyIsImperative()
        {
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
        }

        [Fact]
        public void TestHowLuckyIsFunctional()
        {
            // `Else()` will return the given value if the Maybe has no value.
            Console.WriteLine("One is " + HowLuckyIs(1).Else("not lucky."));
        }

        [Fact]
        public void TestLinqSyntax()
        {
            var luckyNumbers =
                from n in Enumerable.Range(1, 20)
                from s in HowLuckyIs(n)
                where s.Contains("lucky")
                select new {number = n, howLucky = s};
            Assert.Equal(new {number = 13, howLucky = "So lucky."}, luckyNumbers.Single());
        }

        [Fact]
        public void FizzBuzz()
        {
            for (int i = 0; i < 100; i++)
            {
                var fizz = Maybe.If(i%3 == 0, "Fizz");
                var buzz = Maybe.If(i%5 == 0, "Buzz");
                var line = Maybe.If(fizz.HasValue || buzz.HasValue, "" + fizz + buzz)
                    .Else(i.ToString());
                Console.WriteLine(line);
            }
        }


        public Maybe<string> HowLuckyIs(int number)
        {
            if (number == 13)
            {
                return Maybe.From("So lucky.");
            }
            return Maybe.Not;
        }

        public Maybe<string> HowLuckyIs2(int number)
        {
            return number == 13 ? "So lucky." : null;
        }

        public Maybe<string> HowLuckyIs3(int number)
        {
            return Maybe.If(number == 13, "So lucky.");
        }
    }
}