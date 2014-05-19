using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
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
            Assert.AreEqual(Maybe<int>.Not, default(Maybe<int>));
            Assert.AreEqual(new Maybe<int>(), default(Maybe<int>));
            Assert.IsNotNull(default(Maybe<string>));
            Assert.IsFalse(default(Maybe<string>).HasValue);
            Assert.AreEqual(Maybe<string>.Not, default(Maybe<string>));
            Assert.AreEqual(new Maybe<string>(), default(Maybe<string>));
        }

        [Test]
        public void TestNot()
        {
            var not = Maybe.Not;
            Assert.IsNotNull(not);

            Assert.IsFalse(not.HasValue);
        }

        [Test]
        public void TestNotEquality()
        {
            var notNumber1 = Maybe<int>.Not;
            var notNumber2 = Maybe<int>.Not;
            var notName = Maybe<string>.Not;

            Assert.AreEqual(notNumber1, notNumber2, "All not maybes should be object-equal, just as null == null");
            Assert.AreEqual(notNumber1, notName, "All not maybes should be object-equal, just as null == null");
            Assert.IsTrue(notNumber1 == notNumber2, "All not maybes should be equal, just as null == null");
            Assert.IsFalse(notNumber1 != notNumber2, "All not maybes should be equal, just as null == null");
            Assert.IsFalse(notNumber1 == null, "maybes should never equal null");
            Assert.IsTrue(notNumber1 != null, "maybes should never equal null");
            Assert.IsFalse(notNumber1.Equals(null), "maybes should never equal null");
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
        public void TestNotAndNonNotEquality()
        {
            var nameValue = Maybe.From("hi");
            var notName = Maybe<string>.Not;
            Assert.AreNotEqual(nameValue, notName);
            Assert.AreNotEqual(notName, nameValue);
            Assert.IsFalse(notName == nameValue);
            Assert.IsFalse(nameValue == notName);
            Assert.IsTrue(notName != nameValue);
            Assert.IsTrue(nameValue != notName);
        }

        [Test]
        public void TestNullEquality()
        {
            var nameValue = Maybe.From("hi");
            var nullName = Maybe.From((string) null);
            Assert.IsFalse(null == nameValue);
            Assert.IsTrue(null == nullName);
            Assert.IsFalse(nameValue == null);
            Assert.IsTrue(nullName == null);
            Assert.IsTrue(null != nameValue);
            Assert.IsFalse(null != nullName);
            Assert.IsTrue(nameValue != null);
            Assert.IsFalse(nullName != null);
        }

        [Test]
        public void TestEqualityAgainstOtherTypes()
        {
            var notName = Maybe<string>.Not;
            Assert.AreNotEqual(notName, "hi");
            Assert.AreNotEqual("hi", notName);

            var hiName = Maybe.From("hi");
// ReSharper disable once SuspiciousTypeConversion.Global
            Assert.IsTrue(hiName.Equals("hi"));
// ReSharper disable once SuspiciousTypeConversion.Global
            Assert.IsTrue(((object) hiName).Equals("hi"));
            Assert.IsTrue(hiName == "hi");
            Assert.IsFalse(hiName != "hi");
            Assert.IsTrue("hi" == hiName);
            Assert.IsFalse("hi" != hiName);

            var hs = new HashSet<Maybe<int>> {1, 2, Maybe<int>.Not, Maybe.From(1), Maybe.From(2)};
            Assert.AreEqual(3, hs.Count);
            Assert.IsTrue(hs.SequenceEqual(new[] {Maybe.From(1), Maybe.From(2), Maybe<int>.Not}));
        }

        [Test]
        public void TestCovariantMaybeObjectEquality()
        {
            // ReSharper disable SuspiciousTypeConversion.Global
            Assert.IsTrue(Maybe.From<object>(1).Equals(Maybe.From(1)));
            Assert.IsTrue(Maybe.From(1).Equals(Maybe.From<object>(1)));
            Assert.IsTrue(Maybe.From(1) == Maybe.From<object>(1));
            Assert.IsFalse(Maybe.From(1) == Maybe.From<object>(2));
            Assert.IsFalse(Maybe.From(1) != Maybe.From<object>(1));
            Assert.IsTrue(Maybe.From(1) != Maybe.From<object>(2));
            Assert.IsTrue(Maybe.From<object>(1) == Maybe.From(1));
            Assert.IsFalse(Maybe.From<object>(1) != Maybe.From(1));
        }

        [Test]
        public void TestCovariantInheritedClassEquality()
        {
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

        [Test]
        public void TestGetWhenStringBuilding()
        {
            var htmlAttr = new Dictionary<string, object>();
            htmlAttr["class"] = "radio-button" +
                                htmlAttr.GetMaybe("class").Select(a => " " + a);
            Assert.AreEqual("radio-button", htmlAttr["class"]);
            htmlAttr["class"] = "input-field" +
                                htmlAttr.GetMaybe("class").Select(a => " " + a);
            Assert.AreEqual("input-field radio-button", htmlAttr["class"]);
        }

        [Test]
        public void TestGetFromNot()
        {
            Maybe<string> fromInt = Maybe<int>.Not
                .Select(i => i.ToString(CultureInfo.InvariantCulture));
            Assert.IsFalse(fromInt.HasValue);
            Maybe<int> fromString = Maybe<string>.Not
                .Select(int.Parse);
            Assert.IsFalse(fromString.HasValue);
        }

        [Test]
        public void TestGetFromValue()
        {
            Maybe<string> fromInt = Maybe.From(1)
                .Select(i => i.ToString(CultureInfo.InvariantCulture));
            Assert.IsTrue(fromInt.HasValue);
            Assert.AreEqual(Maybe.From("1"), fromInt);
            Maybe<int> fromString = Maybe.From("1")
                .Select(int.Parse);
            Assert.IsTrue(fromString.HasValue);
            Assert.AreEqual(Maybe.From(1), fromString);
        }

        [Test]
        public void TestDoOnNot()
        {
// ReSharper disable RedundantTypeArgumentsOfMethod
            Maybe<int>.Not.Do(FailWithInput<int>);
            Maybe<string>.Not.Do(FailWithInput<string>);
// ReSharper restore RedundantTypeArgumentsOfMethod
            Maybe<int>.Not.Do(i => Assert.Fail());
            Maybe<string>.Not.Do(s => Assert.Fail());
        }

        /// <summary>
        /// This fails, but it also helps us to validate that the appropriate
        /// generic type is being used.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val"></param>
        private void FailWithInput<T>(T val)
        {
            Assert.Fail();
        }


        [Test]
        public void TestDoOnValue()
        {
            CheckDoIsCalledWithValue(0);
            CheckDoIsCalledWithValue(1);
            CheckDoIsCalledWithValue("");
            CheckDoIsCalledWithValue("hello");
        }

        private static void CheckDoIsCalledWithValue<T>(T value)
        {
            bool called = false;
            Maybe.From(value).Do(v =>
            {
                called = true;
                Assert.AreEqual(value, v);
            });
            Assert.IsTrue(called);
        }

        [Test]
        public void TestElseOnNot()
        {
            Assert.AreEqual(0, Maybe<int>.Not.Else(0));
            Assert.AreEqual(1, Maybe<int>.Not.Else(1));
            // TODO: Create ElseNull method?
            //Assert.IsNull(Maybe<string>.Not.Else(null));
            Assert.AreEqual("", Maybe<string>.Not.Else(""));
            Assert.AreEqual("hi", Maybe<string>.Not.Else("hi"));
        }

        [Test]
        public void TestElseOnValue()
        {
            Assert.AreEqual(42, Maybe.From(42).Else(0));
            Assert.AreEqual(42, Maybe.From(42).Else(1));
            // TODO: Create ElseNull Method?
            //Assert.AreEqual("hi", Maybe.From("hi").Else(null));
            Assert.AreEqual("hi", Maybe.From("hi").Else(""));
            Assert.AreEqual("hi", Maybe.From("hi").Else("hi"));
        }

        [Test]
        public void TestElseFuncOnNot()
        {
            Assert.AreEqual(0, Maybe<int>.Not.Else(() => 0));
            Assert.AreEqual(1, Maybe<int>.Not.Else(() => 1));
            Assert.IsNull(Maybe<string>.Not.Else(() => null));
            Assert.AreEqual("", Maybe<string>.Not.Else(() => ""));
            Assert.AreEqual("hi", Maybe<string>.Not.Else(() => "hi"));
        }

        [Test]
        public void TestElseFuncOnValue()
        {
            Assert.AreEqual(42, Maybe.From(42).Else(FailWithReturn<int>));
            Assert.AreEqual("hi", Maybe.From("hi").Else(FailWithReturn<string>));
        }

        /// <summary>
        /// At compile-time, this pretends to return a value, but it'll actually throw an
        /// exception, helping our unit tests to ensure that it isn't called.
        /// </summary>
        public T FailWithReturn<T>()
        {
            Assert.Fail();
            return default(T);
        }

        [Test]
        public void TestMaybeIf()
        {
            Assert.AreEqual(Maybe.From(1), Maybe.If(true, 1));
            Assert.AreEqual(Maybe<int>.Not, Maybe.If(false, 1));
        }

        #region Null Argument Checks

        [Test]
        public void TestDoThrowsExceptionForNullLambda()
        {
            CheckArgumentNullException(() => Maybe<int>.Not.Do(null));
            CheckArgumentNullException(() => Maybe.From(0).Do(null));
            CheckArgumentNullException(() => Maybe<string>.Not.Do(null));
            CheckArgumentNullException(() => Maybe.From("hi").Do(null));
        }

        [Test]
        public void TestElseThrowsExceptionForNullLambda()
        {
            CheckArgumentNullException(() => Maybe<int>.Not.Else(null));
            CheckArgumentNullException(() => Maybe.From(0).Else(null));
            CheckArgumentNullException(() => Maybe<string>.Not.Else((Func<string>) null));
            CheckArgumentNullException(() => Maybe.From("hi").Else((Func<string>) null));
        }

        private void CheckArgumentNullException(Expression<Action> methodCall)
        {
            var methodCallExpr = (MethodCallExpression) methodCall.Body;
            var methodInfo = methodCallExpr.Method;
            var nullArg = methodCallExpr.Arguments
                .Zip(methodInfo.GetParameters(), (a, p) => new {argument = a as ConstantExpression, name = p.Name})
                .Single(c => c.argument != null && c.argument.Value == null);
            var action = methodCall.Compile();
            var e = Assert.Throws<ArgumentNullException>(action.Invoke,
                "{0} did not throw an ArgumentNullException for {1}", methodCall, nullArg.name);
            Assert.AreEqual(e.ParamName, nullArg.name);
        }

        #endregion

        private class Parent
        {
        }

        private class Child : Parent
        {
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

        // TODO: Test hash code distribution with multiple different types of "empty" maybes.
        // (Hint: GetHashCode() needs to be the same any time Equals() would return true.)

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
            Maybe<string> name = null;
            Assert.AreEqual(name, Maybe<string>.Not);
            Assert.AreEqual("0: ", FooDo(0, null));
        }

        [Test]
        public void TestToString()
        {
            Assert.AreEqual("", Maybe<int>.Not.ToString());
            Assert.AreEqual("", Maybe<string>.Not.ToString());
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

        [Test]
        public void TestOptionalInterfaceParameters()
        {
            IOptionalInterface i = new OptionalImplementation();
            Assert.AreEqual(" => 1: ", i.FormatInfo(1));
            Assert.AreEqual("1 => 2: A", i.FormatInfo(2, "A", 1));
        }

        private interface IOptionalInterface
        {
            string FormatInfo(int id,
                Maybe<string> name = default(Maybe<string>),
                Maybe<int> parentId = default(Maybe<int>));
        }

        private class OptionalImplementation : IOptionalInterface
        {
            public string FormatInfo(int id,
                Maybe<string> name, // These are marked optional in the interface, but not here
                Maybe<int> parentId)
            {
                Assert.IsNotNull(name);
                Assert.IsNotNull(parentId);
                return string.Format("{2} => {0}: {1}", id, name, parentId);
            }
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