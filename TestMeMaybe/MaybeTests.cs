using System;
using System.Linq;
using System.Text;
using CallMeMaybe;
using NUnit.Framework;

namespace TestMeMaybe
{
    [TestFixture]
    public class MaybeTests
    {
        [Test]
        public void TestUninitializedValues()
        {
            var foo = new Foo();
            Assert.IsNotNull(foo.Number, "Uninitialized values should not be null.");
            Assert.IsNotNull(foo.Number, "Uninitialized values should not be null.");
            Assert.IsFalse(foo.Number.HasValue, "Uninitialized values should have no value.");
            Assert.IsFalse(foo.Name.HasValue, "Uninitialized values should have no value.");
        }

        [Test]
        public void TestInitializedValues()
        {
            var foo = new Foo
            {
                Name = Maybe.From("Name"),
                Number = Maybe.From(1)
            };

            Assert.IsNotNull(foo.Name);
            Assert.IsTrue(foo.Name.HasValue);
            Assert.IsNotNull(foo.Number);
            Assert.IsTrue(foo.Number.HasValue);
        }

        [Test]
        public void TestEmpty()
        {
            var emptyNumber = Maybe.Empty<int>();
            var emptyName = Maybe.Empty<string>();

            Assert.IsNotNull(emptyNumber);
            Assert.IsNotNull(emptyName);

            Assert.IsFalse(emptyNumber.HasValue);
            Assert.IsFalse(emptyName.HasValue);
        }

        [Test]
        public void TestEmptyEquality()
        {
            var emptyNumber1 = Maybe.Empty<int>();
            var emptyNumber2 = Maybe.Empty<int>();
            var emptyName = Maybe.Empty<string>();

            Assert.AreEqual(emptyNumber1, emptyNumber2, "All empty maybes should be object-equal, just as null == null");
            Assert.AreEqual(emptyNumber1, emptyName, "All empty maybes should be object-equal, just as null == null");
            Assert.IsTrue(emptyNumber1 == emptyNumber2, "All empty maybes should be equal, just as null == null");
            Assert.IsFalse(emptyNumber1 != emptyNumber2, "All empty maybes should be equal, just as null == null");
            Assert.IsFalse(emptyNumber1 == null, "maybes should never equal null");
            Assert.IsTrue(emptyNumber1 != null, "maybes should never equal null");
            Assert.IsFalse(emptyNumber1.Equals(null), "maybes should never equal null");
        }

        [Test]
        public void TestValueEqualityForSameValueType()
        {
            var numberA = Maybe.From(1);
            var numberB = Maybe.From(1);
            var numberC = Maybe.From(2);

            Assert.AreEqual(numberA, numberA);
            Assert.AreEqual(numberA, numberB);
            Assert.AreEqual(numberB, numberA);
            Assert.AreNotEqual(numberA, numberC);
            Assert.AreNotEqual(numberC, numberA);
            Assert.AreNotEqual(numberB, numberC);
            Assert.AreNotEqual(numberC, numberB);
            Assert.IsTrue(numberA == numberB);
            Assert.IsTrue(numberB == numberA);
            Assert.IsFalse(numberA == numberC);
            Assert.IsFalse(numberC == numberA);
        }

        [Test]
        public void TestValueEqualityForSameReferenceType()
        {
            var nameA = Maybe.From("hi");
            var nameB = Maybe.From(new StringBuilder("hi").ToString());
            var nameC = Maybe.From("hello");

            Assert.AreEqual(nameA, nameA);
            Assert.IsTrue(nameA.Equals((object) nameA));
            Assert.AreEqual(nameA, nameB);
            Assert.AreEqual(nameB, nameA);
            Assert.AreNotEqual(nameA, nameC);
            Assert.AreNotEqual(nameC, nameA);
            Assert.AreNotEqual(nameB, nameC);
            Assert.AreNotEqual(nameC, nameB);
            Assert.IsTrue(nameA == nameB);
            Assert.IsTrue(nameB == nameA);
            Assert.IsFalse(nameA == nameC);
            Assert.IsFalse(nameC == nameA);
        }

        [Test]
        public void TestEqualityForDifferentMaybeTypes()
        {
            var nameA = Maybe.From("hi");
            var numberA = Maybe.From(1);
            Assert.AreNotEqual(nameA, numberA);
            Assert.AreNotEqual(numberA, nameA);
        }

        [Test]
        public void TestEmptyAndNonEmptyEquality()
        {
            var nameValue = Maybe.From("hi");
            var emptyName = Maybe.Empty<string>();
            Assert.AreNotEqual(nameValue, emptyName);
            Assert.AreNotEqual(emptyName, nameValue);
            Assert.IsFalse(emptyName == nameValue);
            Assert.IsFalse(nameValue == emptyName);
        }

        [Test]
        public void TestEqualityAgainstOtherTypes()
        {
            var emptyName = Maybe.Empty<string>();
            Assert.AreNotEqual(emptyName, "hi");
            Assert.AreNotEqual("hi", emptyName);
        }

        [Test]
        public void TestHashCodeDistribution()
        {
            const int max = 100;
            var maybes = Enumerable.Range(1, max)
                .Select(Maybe.From)
                .Select(m => m.GetHashCode());
            // I am intentionally invoking these twice, in order to ensure
            // that GetHashCode produces consistent values.
            // ReSharper disable PossibleMultipleEnumeration
            var hashCodes1 = maybes.ToList();
            var hashCodes2 = maybes.ToList();
            // ReSharper restore PossibleMultipleEnumeration
            Assert.IsTrue(hashCodes1.SequenceEqual(hashCodes2), "The same values should produce the same hash codes.");

            var maxModThirteenCollisions = hashCodes1.GroupBy(c => c%13).Max(c => c.Count());
            // We can accept as many as twice the ideal number of collisions.
            Assert.IsTrue(maxModThirteenCollisions < (max/13)*2,
                "GetHashCode should avoid producing likely collisions: " + maxModThirteenCollisions);
        }

        [Test]
        public void TestImplicitCasting()
        {
            Maybe<int> number = 1;
            Assert.IsTrue(number.HasValue);
            FooDo(0, "hi");
        }

        [Test]
        public void TestNullImplicitCasting()
        {
#pragma warning disable 168
            Assert.Throws<ArgumentNullException>(() => { Maybe<string> number = null; });
#pragma warning restore 168
            Assert.Throws<ArgumentNullException>(() => FooDo(0, null));
        }

        private string FooDo(Maybe<int> number, Maybe<string> name)
        {
            return number + ": " + name;
        }

        // Basic class, useful for testing.
        public class Foo
        {
            // Backed by a Value type
            public Maybe<int> Number { get; set; }
            // Backed by a reference type
            public Maybe<string> Name { get; set; }
        }
    }
}