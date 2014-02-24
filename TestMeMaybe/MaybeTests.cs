using System;
using System.Collections.Generic;
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
        public void TestDefault()
        {
            Assert.IsNotNull(default(Maybe<int>));
            Assert.IsFalse(default(Maybe<int>).HasValue);
            Assert.AreEqual(Maybe.Empty<int>(), default(Maybe<int>));
            Assert.AreEqual(new Maybe<int>(), default(Maybe<int>));
            Assert.IsNotNull(default(Maybe<string>));
            Assert.IsFalse(default(Maybe<string>).HasValue);
            Assert.AreEqual(Maybe.Empty<string>(), default(Maybe<string>));
            Assert.AreEqual(new Maybe<string>(), default(Maybe<string>));
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
// ReSharper disable once EqualExpressionComparison
            Assert.IsTrue(nameA.Equals(nameA));
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
            Assert.IsTrue(emptyName != nameValue);
            Assert.IsTrue(nameValue != emptyName);
        }

        [Test]
        public void TestEqualityAgainstOtherTypes()
        {
            var emptyName = Maybe.Empty<string>();
            Assert.AreNotEqual(emptyName, "hi");
            Assert.AreNotEqual("hi", emptyName);

            var hiName = Maybe.From("hi");
// ReSharper disable once SuspiciousTypeConversion.Global
            Assert.IsTrue(hiName.Equals("hi"));
// ReSharper disable once SuspiciousTypeConversion.Global
            Assert.IsTrue(((object) hiName).Equals("hi"));
            Assert.IsTrue(hiName == "hi");
            Assert.IsFalse(hiName != "hi");
            Assert.IsTrue("hi" == hiName);
            Assert.IsFalse("hi" != hiName);

            var hs = new HashSet<Maybe<int>> {1, 2, Maybe.Empty<int>(), Maybe.From(1), Maybe.From(2)};
            Assert.AreEqual(3, hs.Count);
            Assert.IsTrue(hs.SequenceEqual(new[] {Maybe.From(1), Maybe.From(2), Maybe.Empty<int>()}));
        }

        [Test]
        public void TestCovariantEquality()
        {
            // ReSharper disable SuspiciousTypeConversion.Global
            Assert.IsTrue(Maybe.From<object>(1).Equals(Maybe.From(1)));
            Assert.IsTrue(Maybe.From(1).Equals(Maybe.From<object>(1)));
            Assert.IsTrue(Maybe.From(1) == Maybe.From<object>(1));
            Assert.IsFalse(Maybe.From(1) == Maybe.From<object>(2));
            Assert.IsFalse(Maybe.From(1) != Maybe.From<object>(1));
            Assert.IsTrue(Maybe.From(1) != Maybe.From<object>(2));

            var child = new Child();
            Assert.IsTrue(Maybe.From<Parent>(child).Equals(Maybe.From(child)));
            Assert.IsTrue(Maybe.From(child).Equals(Maybe.From<Parent>(child)));
            // Limitation: any attempt to do a covariant equality check results in a compiler error.
            /*
            Assert.IsTrue(Maybe.From<Child>(child) == Maybe.From<Parent>(child));
            Assert.IsFalse(Maybe.From<Child>(child) == Maybe.From<Parent>(new Child()));
            Assert.IsFalse(Maybe.From<Child>(child) != Maybe.From<Parent>(child));
            Assert.IsTrue(Maybe.From<Child>(child) != Maybe.From<Parent>(new Child()));
            */
            // ReSharper restore SuspiciousTypeConversion.Global

            // TODO: See if we can create a stronger version of the Cast and OfType LINQ methods
            // so we can say `parentMaybe.OfType<Child>() == childMaybe`
        }

        private class Parent
        {
        }

        private class Child : Parent
        {
        }

        [Test]
        public void TestWeirdness(){
        // This is the one major limitation that I've found so far:
            // The second Maybe get implicitly cast into a Maybe<object>,
            // whose value is a Maybe<int>
            // TODO: See if we can change this behavior by making Maybes unwrap inner Maybes.
            Assert.IsFalse(Maybe.From<object>(1) == Maybe.From(1));
            Assert.True(Maybe.From<object>(1) != Maybe.From(1));
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

        [Test]
        public void TestToString()
        {
            Assert.AreEqual("", Maybe.Empty<int>().ToString());
            Assert.AreEqual("", Maybe.Empty<string>().ToString());
            Assert.AreEqual("1", Maybe.From(1).ToString());
            Assert.AreEqual("hi", Maybe.From("hi").ToString());
        }

        [Test]
        public void TestOptionalParameters()
        {
            Assert.AreEqual(" => 1: ", FormatInfo(1));
            Assert.AreEqual("1 => 2: A", FormatInfo(2, "A", 1));
        }

        private string FormatInfo(int id,
            Maybe<string> name = default(Maybe<string>),
            Maybe<int> parentId = default(Maybe<int>))
        {
            Assert.IsNotNull(name);
            Assert.IsNotNull(parentId);
            return string.Format("{2} => {0}: {1}", id, name, parentId);
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