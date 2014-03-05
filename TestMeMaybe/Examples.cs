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

        public Maybe<string> HowLuckyIs(int number)
        {
            if (number == 13)
            {
                return Maybe.From("So lucky.");
            }
            return Maybe.Not<string>();
        }

        public Maybe<string> HowLuckyIs2(int number)
        {
            return number == 13 ? "So lucky." : null;
        }
    }
}