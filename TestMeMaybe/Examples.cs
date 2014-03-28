using System;
using System.Linq;
using CallMeMaybe;
using NUnit.Framework;

namespace TestMeMaybe
{
    [TestFixture]
    public class Examples
    {
        [Test]
        public void TestHowLuckyIs()
        {
            // Next line won't compile
            // Assert.AreEqual(3, HowLuckyIs(13).IndexOf("lucky"));

            // `Maybe<>` implements IEnumerable, so LINQ methods like .Single() will
            // behave the way any developer would expect them to.
            Assert.AreEqual(3, HowLuckyIs(13).Single().IndexOf("lucky", StringComparison.InvariantCulture));
        }

        [Test]
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

        [Test]
        public void TestHowLuckyIsFunctional()
        {
            // `Else()` will return the given value if the Maybe has no value.
            Console.WriteLine("One is " + HowLuckyIs(1).Else("not lucky."));
        }

        [Test]
        public void TestLinqSyntax()
        {
            var luckyNumbers =
                from n in Enumerable.Range(1, 20)
                from s in HowLuckyIs(n)
                where s.Contains("lucky")
                select new {number = n, howLucky = s};
            Assert.AreEqual(new {number = 13, howLucky = "So lucky."}, luckyNumbers.Single());
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