using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using FluentAssertions;
using Xunit;

namespace CallMeMaybe.UnitTests
{
    public class MaybeTests
    {
        [Fact]
        public void TestUninitializedValues()
        {
            var foo = new Foo();
            foo.Number.Should().NotBeNull("Uninitialized values should not be null.");
            foo.Number.HasValue.Should().BeFalse("Uninitialized values should have no value.");
        }

        [Fact]
        public void TestInitializedValues()
        {
            var foo = new Foo
            {
                Name = Maybe.From("Name"),
                Number = Maybe.From(1)
            };

            foo.Name.Should().NotBeNull();
            foo.Name.HasValue.Should().BeTrue();
            foo.Number.Should().NotBeNull();
            foo.Number.HasValue.Should().BeTrue();
        }

        [Fact]
        public void TestDefault()
        {
            Assert.NotNull(default(Maybe<int>));
            Assert.False(default(Maybe<int>).HasValue);
            Assert.Equal(Maybe<int>.Not, default(Maybe<int>));
            Assert.Equal(new Maybe<int>(), default(Maybe<int>));
            Assert.NotNull(default(Maybe<string>));
            Assert.False(default(Maybe<string>).HasValue);
            Assert.Equal(Maybe<string>.Not, default(Maybe<string>));
            Assert.Equal(new Maybe<string>(), default(Maybe<string>));
        }

        [Fact]
        public void TestNullables()
        {
            int? isNull = null;
            int? notNull = 1;
            // ReSharper disable ExpressionIsAlwaysNull
            Assert.False(Maybe.From(isNull).HasValue);
            Assert.True(Maybe.From(notNull).HasValue);
            Assert.IsAssignableFrom<Maybe<int>>(Maybe.From(isNull));
            Assert.IsAssignableFrom<Maybe<int>>(Maybe.From(notNull));
            // ReSharper restore ExpressionIsAlwaysNull
        }


        [Fact]
        public void TestNot()
        {
            var not = Maybe.Not;
            Assert.NotNull(not);
        }

        [Fact]
        public void TestNotEquality()
        {
            var notNumber1 = Maybe<int>.Not;
            var notNumber2 = Maybe<int>.Not;
            var notName = Maybe<string>.Not;

            notNumber1.Should().Be(notNumber2, "Not maybes should be .Equals() with others of the same generic type.");
            notNumber1.Should().NotBe(notName, "Maybes should never be .Equals() with Maybes of other generic types");


            Assert.True(notNumber1 == notNumber2, "All not maybes should be equal, just as null == null");
            Assert.False(notNumber1 != notNumber2, "All not maybes should be equal, just as null == null");
            Assert.False(notNumber1 == null, "maybes should never equal null");
            Assert.True(notNumber1 != null, "maybes should never equal null");
            Assert.False(notNumber1.Equals(null), "maybes should never equal null");
        }

        [Fact]
        public void TestValueEqualityForSameValueType()
        {
            var numberA = Maybe.From(1);
            var numberB = Maybe.From(1);
            var numberC = Maybe.From(2);

            Assert.Equal(numberA, numberA);
            Assert.Equal(numberA, numberB);
            Assert.Equal(numberB, numberA);
            Assert.NotEqual(numberA, numberC);
            Assert.NotEqual(numberC, numberA);
            Assert.NotEqual(numberB, numberC);
            Assert.NotEqual(numberC, numberB);
            Assert.True(numberA == numberB);
            Assert.True(numberB == numberA);
            Assert.False(numberA == numberC);
            Assert.False(numberC == numberA);
        }

        [Fact]
        public void TestValueEqualityForSameReferenceType()
        {
            var nameA = Maybe.From("hi");
            var nameB = Maybe.From(new StringBuilder("hi").ToString());
            var nameC = Maybe.From("hello");

            Assert.Equal(nameA, nameA);
// ReSharper disable once EqualExpressionComparison
            Assert.True(nameA.Equals(nameA));
            Assert.Equal(nameA, nameB);
            Assert.Equal(nameB, nameA);
            Assert.NotEqual(nameA, nameC);
            Assert.NotEqual(nameC, nameA);
            Assert.NotEqual(nameB, nameC);
            Assert.NotEqual(nameC, nameB);
            Assert.True(nameA == nameB);
            Assert.True(nameB == nameA);
            Assert.False(nameA == nameC);
            Assert.False(nameC == nameA);
        }

        [Fact]
        public void TestEqualityForDifferentMaybeTypes()
        {
            var nameA = Maybe.From("hi");
            var numberA = Maybe.From(1);
            nameA.Should().NotBe(numberA);
            numberA.Should().NotBe(nameA);
        }

        [Fact]
        public void TestNotAndNonNotEquality()
        {
            var nameValue = Maybe.From("hi");
            var notName = Maybe<string>.Not;
            Assert.NotEqual(nameValue, notName);
            Assert.NotEqual(notName, nameValue);
            Assert.False(notName == nameValue);
            Assert.False(nameValue == notName);
            Assert.True(notName != nameValue);
            Assert.True(nameValue != notName);
        }

        [Fact]
        public void TestNullEquality()
        {
            var nameValue = Maybe.From("hi");
            var nullName = Maybe.From((string) null);
            Assert.False(null == nameValue);
            Assert.True(null == nullName);
            Assert.False(nameValue == null);
            Assert.True(nullName == null);
            Assert.True(null != nameValue);
            Assert.False(null != nullName);
            Assert.True(nameValue != null);
            Assert.False(nullName != null);
        }

        [Fact]
        public void TestEqualityAgainstOtherTypes()
        {
            var notName = Maybe<string>.Not;
            Assert.NotEqual(notName, "hi");
            Assert.NotEqual("hi", notName);

            var hiName = Maybe.From("hi");
            // ReSharper disable SuspiciousTypeConversion.Global
            Assert.False(hiName.Equals("hi"));
            Assert.False("hi".Equals(hiName));
            Assert.False(((object) hiName).Equals("hi"));
            // ReSharper restore SuspiciousTypeConversion.Global
            Assert.True(hiName == "hi");
            Assert.False(hiName != "hi");
            Assert.True("hi" == hiName);
            Assert.False("hi" != hiName);

            var hs = new HashSet<Maybe<int>> {1, 2, Maybe<int>.Not, Maybe.From(1), Maybe.From(2)};
            Assert.Equal(3, hs.Count);
            Assert.True(hs.SequenceEqual(new[] {Maybe.From(1), Maybe.From(2), Maybe<int>.Not}));
        }

        [Fact]
        public void TestCovariantMaybeObjectEquality()
        {
            // ReSharper disable SuspiciousTypeConversion.Global
            Assert.False(Maybe.From<object>(1).Equals(Maybe.From(1)));
            Assert.False(Maybe.From(1).Equals(Maybe.From<object>(1)));
            // ReSharper restore SuspiciousTypeConversion.Global
            Assert.True(Maybe.From(1) == Maybe.From<object>(1));
            Assert.False(Maybe.From(1) == Maybe.From<object>(2));
            Assert.False(Maybe.From(1) != Maybe.From<object>(1));
            Assert.True(Maybe.From(1) != Maybe.From<object>(2));
            Assert.True(Maybe.From<object>(1) == Maybe.From(1));
            Assert.False(Maybe.From<object>(1) != Maybe.From(1));
        }

        [Fact]
        public void TestCovariantInheritedClassEquality()
        {
            var child = new Child();
// ReSharper disable SuspiciousTypeConversion.Global
            Assert.False(Maybe.From<Parent>(child).Equals(Maybe.From(child)));
            Assert.False(Maybe.From(child).Equals(Maybe.From<Parent>(child)));
// ReSharper restore SuspiciousTypeConversion.Global
            // Limitation: any attempt to do a covariant equality check results in a compiler error.
            /*
            Assert.IsTrue(Maybe.From<Child>(child) == Maybe.From<Parent>(child));
            Assert.IsFalse(Maybe.From<Child>(child) == Maybe.From<Parent>(new Child()));
            Assert.IsFalse(Maybe.From<Child>(child) != Maybe.From<Parent>(child));
            Assert.IsTrue(Maybe.From<Child>(child) != Maybe.From<Parent>(new Child()));
            */
        }

        [Fact]
        public void TestSelectWhenStringBuilding()
        {
            var htmlAttr = new Dictionary<string, object>();
            htmlAttr["class"] = "radio-button" +
                                htmlAttr.GetMaybe("class").Select(a => " " + a);
            Assert.Equal("radio-button", htmlAttr["class"]);
            htmlAttr["class"] = "input-field" +
                                htmlAttr.GetMaybe("class").Select(a => " " + a);
            Assert.Equal("input-field radio-button", htmlAttr["class"]);
        }

        [Fact]
        public void TestSelectFromNot()
        {
            Maybe<string> fromInt = Maybe<int>.Not
                .Select(i => i.ToString(CultureInfo.InvariantCulture));
            Assert.False(fromInt.HasValue);
            Maybe<int> fromString = Maybe<string>.Not
                .Select(int.Parse);
            Assert.False(fromString.HasValue);
        }

        [Fact]
        public void TestSelectFromValue()
        {
            Maybe<string> fromInt = Maybe.From(1)
                .Select(i => i.ToString(CultureInfo.InvariantCulture));
            Assert.True(fromInt.HasValue);
            Assert.Equal(Maybe.From("1"), fromInt);
            Maybe<int> fromString = Maybe.From("1")
                .Select(int.Parse);
            Assert.True(fromString.HasValue);
            Assert.Equal(Maybe.From(1), fromString);
        }

        [Fact]
        public void TestIsLambda()
        {
            Assert.False(Maybe.From(1).Is(i => i%2 == 0));
            Assert.True(Maybe.From(1).Is(i => i%2 == 1));
            Assert.False(Maybe.From("hi").Is(s => s.Length == 0));
            Assert.True(Maybe.From("hi").Is(s => s.Length == 2));

            Assert.False(Maybe.From("hi").Is(string.IsNullOrEmpty));
            Assert.True(Maybe.From("").Is(string.IsNullOrEmpty));

            Assert.False(Maybe.From((string) null).Is(s =>
            {
                Assert.True(false);
                return true;
            }));
            Assert.False(Maybe.From((int?) null).Is(i =>
            {
                Assert.True(false);
                return true;
            }));
            ArgumentChecker.For(Maybe.From("hi")).AssertReferenceParametersDisallowNullValues(m => m.Is(t => true));
        }

        [Fact]
        public void TestIsLambdaDisallowsNullLambda()
        {
        }

        [Fact]
        public void TestIsValue()
        {
            Assert.False(Maybe.From(1).Is(2));
            Assert.True(Maybe.From(1).Is(1));
            Assert.False(Maybe.From((int?) null).Is(1));
            Assert.False(Maybe.From("hi").Is("bye"));
            Assert.True(Maybe.From("hi").Is("hi"));
            Assert.False(Maybe.From((string) null).Is("hi"));
            Assert.False(Maybe.From("hi").Is((string) null));
            Assert.False(Maybe.From((string) null).Is((string) null));
            Assert.False(ReferenceEquals("hi", new StringBuilder("hi").ToString()),
                "Assumption: StringBuilder allows us to avoid using an interned string reference.");
            Assert.True(Maybe.From("hi").Is(new StringBuilder("hi").ToString()),
                "Object's overridden Equals method should be used to determine equality.");
        }


        [Fact]
        public void TestDoOnNot()
        {
// ReSharper disable RedundantTypeArgumentsOfMethod
            Maybe<int>.Not.Do(FailWithInput<int>);
            Maybe<string>.Not.Do(FailWithInput<string>);
// ReSharper restore RedundantTypeArgumentsOfMethod
            Maybe<int>.Not.Do(i => Assert.True(false));
            Maybe<string>.Not.Do(s => Assert.True(false));
        }

        /// <summary>
        /// This fails, but it also helps us to validate that the appropriate
        /// generic type is being used.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val"></param>
        private void FailWithInput<T>(T val)
        {
            Assert.True(false);
        }
        
        [Fact]
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
                Assert.Equal(value, v);
            });
            Assert.True(called);
        }

        [Fact]
        public void TestElseDoOnNot()
        {
            CheckElseDoIsCalled<int>();
            CheckElseDoIsCalled<string>();
        }


        [Fact]
        public void TestElseDoOnValue()
        {
            Maybe.From(0).ElseDo(() => Assert.True(false));
            Maybe.From(1).ElseDo(() => Assert.True(false));
            Maybe.From("").ElseDo(() => Assert.True(false));
            Maybe.From("hello").ElseDo(() => Assert.True(false));
        }

        private static void CheckElseDoIsCalled<T>()
        {
            var called = false;
            Maybe<T>.Not.ElseDo(() => called = true);
            Assert.True(called);
        }

        [Fact]
        public void TestElseOnNot()
        {
            Assert.Equal(0, Maybe<int>.Not.Else(0));
            Assert.Equal(1, Maybe<int>.Not.Else(1));
            Assert.Null(Maybe<string>.Not.Else(() => null));
            Assert.Equal("", Maybe<string>.Not.Else(""));
            Assert.Equal("hi", Maybe<string>.Not.Else("hi"));
        }

        [Fact]
        public void TestElseOnValue()
        {
            Assert.Equal(42, Maybe.From(42).Else(0));
            Assert.Equal(42, Maybe.From(42).Else(1));
            Assert.Equal("hi", Maybe.From("hi").Else(() => null));
            Assert.Equal("hi", Maybe.From("hi").Else(""));
            Assert.Equal("hi", Maybe.From("hi").Else("hi"));
        }

        [Fact]
        public void TestElseFuncOnNot()
        {
            Assert.Equal(0, Maybe<int>.Not.Else(() => 0));
            Assert.Equal(1, Maybe<int>.Not.Else(() => 1));
            Assert.Null(Maybe<string>.Not.Else(() => null));
            Assert.Equal("", Maybe<string>.Not.Else(() => ""));
            Assert.Equal("hi", Maybe<string>.Not.Else(() => "hi"));
        }

        [Fact]
        public void TestElseFuncOnValue()
        {
            Assert.Equal(42, Maybe.From(42).Else(FailWithReturn<int>));
            Assert.Equal("hi", Maybe.From("hi").Else(FailWithReturn<string>));
        }

        /// <summary>
        /// At compile-time, this pretends to return a value, but it'll actually throw an
        /// exception, helping our unit tests to ensure that it isn't called.
        /// </summary>
        public T FailWithReturn<T>()
        {
            Assert.True(false);
            return default(T);
        }

        [Fact]
        public void TestMaybeIf()
        {
            Assert.Equal(Maybe.From(1), Maybe.If(true, 1));
            Assert.Equal(Maybe<int>.Not, Maybe.If(false, 1));
        }

        #region Null Argument Checks

        [Fact]
        public void TestDoThrowsExceptionForNullLambda()
        {
            CheckArgumentNullException(() => Maybe<int>.Not.Do(null));
            CheckArgumentNullException(() => Maybe.From(0).Do(null));
            CheckArgumentNullException(() => Maybe<string>.Not.Do(null));
            CheckArgumentNullException(() => Maybe.From("hi").Do(null));
        }

        [Fact]
        public void TestElseDoThrowsExceptionForNullLambda()
        {
            CheckArgumentNullException(() => Maybe<int>.Not.ElseDo(null));
            CheckArgumentNullException(() => Maybe.From(0).ElseDo(null));
            CheckArgumentNullException(() => Maybe<string>.Not.ElseDo(null));
            CheckArgumentNullException(() => Maybe.From("hi").ElseDo(null));
        }

        [Fact]
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
            var e = Assert.Throws<ArgumentNullException>(action);
            Assert.Equal(e.ParamName, nullArg.name);
        }

        #endregion

        private class Parent
        {
        }

        private class Child : Parent
        {
        }


        [Fact]
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
            Assert.True(hashCodes1.SequenceEqual(hashCodes2), "The same values should produce the same hash codes.");

            var maxModThirteenCollisions = hashCodes1.GroupBy(c => c%13).Max(c => c.Count());
            // We can accept as many as twice the ideal number of collisions.
            Assert.True(maxModThirteenCollisions < (max/13)*2,
                "GetHashCode should avoid producing likely collisions: " + maxModThirteenCollisions);
        }

        [Fact]
        public void TestEmptyEqualsBehavior()
        {
            // Empty values of any type should be equal to other empty values of 
            // the same type.
// ReSharper disable EqualExpressionComparison
            Assert.True(Maybe.Not.Equals(Maybe.Not));
            Assert.True(Maybe<int>.Not.Equals(Maybe<int>.Not));
            Assert.True(Maybe<string>.Not.Equals(Maybe<string>.Not));
// ReSharper restore EqualExpressionComparison
            // Users are discouraged from casting Maybe<> values as objects, or
            // otherwise using them in collections with other Maybe<> types.
            // However, if they do, the best thing we can do is be consistent.
            // We cannot create parity to make object.Equals(null, m) return true,
            // so it doesn't make sense to make object.Equals(m, null) return true.
            // Each type of Maybe<> can only be considered equal with another Maybe<>
            // of the same type and value.
// ReSharper disable SuspiciousTypeConversion.Global
            Assert.False(Maybe.Not.Equals(Maybe<int>.Not));
            Assert.False(Maybe<int>.Not.Equals(Maybe.Not));
            Assert.False(Maybe<string>.Not.Equals(Maybe.Not));
            Assert.False(Maybe.Not.Equals(Maybe<string>.Not));
            Assert.False(Maybe<string>.Not.Equals(Maybe<int>.Not));
            Assert.False(Maybe<int>.Not.Equals(Maybe<string>.Not));
            // Value types cannot be equal to null, so Maybe<T> for value types can't 
            // either.
            Assert.False(null == Maybe<int>.Not);
            Assert.False(Maybe<int>.Not == null);
            // null can be implicitly cast to a reference type, and then implicitly
            // cast into a Maybe<T> of a reference type. This actually works out well
            // because if someone was using `return null;` in their method, an then
            // checked `if (value != null)` in a calling method, converting that method
            // to return a Maybe<> will "just work".
            Assert.True(null == Maybe<string>.Not);
            Assert.True(Maybe<string>.Not == null);
            // ReSharper restore SuspiciousTypeConversion.Global
            // This works because Maybe.Not gets implicitly cast to a Maybe<int>.
            Assert.True(Maybe<int>.Not == Maybe.Not);
            Assert.True(Maybe.Not == Maybe<int>.Not);
        }

        [Fact]
        public void TestEmptyGetHashCode()
        {
            var baseHash = Maybe.Not.GetHashCode();
            Assert.NotEqual(baseHash, Maybe<int>.Not.GetHashCode());
            Assert.NotEqual(baseHash, Maybe<string>.Not.GetHashCode());
            Assert.NotEqual(baseHash, Maybe<bool>.Not.GetHashCode());
        }

        [Fact]
        public void TestHashSetBehavior()
        {
            /* Each type of Maybe<> value is its own distinct value, not the same as null, and
             * not the same as any other Maybe<> type, even if they're based on the same value.
             */
            var variousValues = new object[]
            {Maybe.From(1), Maybe.Not, Maybe.From("1"), new Maybe<IEnumerable<char>>("1".AsEnumerable()), 1, null, "1"};
            Assert.True(new HashSet<object>(variousValues).SequenceEqual(variousValues));
            Assert.True(new HashSet<object>(variousValues.Reverse()).SequenceEqual(variousValues.Reverse()));

            // Maybe<>s of the same type and value should still be treated as the same value.
            var dupeValues = variousValues.Concat(variousValues).ToArray();
            Assert.True(new HashSet<object>(dupeValues).SequenceEqual(variousValues));
            Assert.True(new HashSet<object>(dupeValues.Reverse()).SequenceEqual(variousValues.Reverse()));
        }

        [Fact]
        public void TestImplicitCasting()
        {
            Maybe<int> number = 1;
            Assert.True(number.HasValue);
            FooDo(0, "hi");
        }

        [Fact]
        public void TestNullImplicitCasting()
        {
            Maybe<string> name = null;
            Assert.Equal(name, Maybe<string>.Not);
            Assert.Equal("0: ", FooDo(0, null));
        }

        //[Fact]
        //public void TestOfType()
        //{
            
        //    Assert.IsInstanceOf<Maybe<object>>(Maybe.From("").OfType<object>());
        //    Assert.IsTrue(Maybe.From("").HasValue);
        //    var child = new ChildClass();
        //    Assert.IsNotInstanceOf<Maybe<object>>(Maybe.From(child).OfType<ParentClass>());
        //    Assert.IsInstanceOf<Maybe<ParentClass>>(Maybe.From(child).OfType<ParentClass>());
        //    Assert.IsTrue(Maybe.From(child).OfType<ParentClass>().HasValue);
        //    Assert.IsNotInstanceOf<Maybe<ParentClass>>(Maybe.From(child).OfType<ParentClass>().OfType<ChildClass>());
        //    Assert.IsInstanceOf<Maybe<ChildClass>>(Maybe.From(child).OfType<ParentClass>().OfType<ChildClass>());
        //    Assert.IsTrue(Maybe.From(child).OfType<ParentClass>().OfType<ChildClass>().HasValue);
        //    Assert.AreEqual(child, Maybe.From(child).OfType<ParentClass>().OfType<ChildClass>().Single());

        //    Assert.IsNotInstanceOf<Maybe<object>>(Maybe.From(child).OfType<IChildClassInterface>());
        //    Assert.IsInstanceOf<Maybe<IChildClassInterface>>(Maybe.From(child).OfType<IChildClassInterface>());
        //    Assert.IsTrue(Maybe.From(child).OfType<IChildClassInterface>().HasValue);

        //    var parent = new ParentClass();
        //    Assert.IsNotInstanceOf<Maybe<ParentClass>>(Maybe.From(parent).OfType<ChildClass>());
        //    Assert.IsInstanceOf<Maybe<ChildClass>>(Maybe.From(parent).OfType<ChildClass>());
        //    Assert.IsFalse(Maybe.From(parent).OfType<ChildClass>().HasValue);

        //    Assert.IsNotInstanceOf<Maybe<object>>(Maybe.From(parent).OfType<IChildClassInterface>());
        //    Assert.IsInstanceOf<Maybe<IChildClassInterface>>(Maybe.From(parent).OfType<IChildClassInterface>());
        //    Assert.IsFalse(Maybe.From(parent).OfType<IChildClassInterface>().HasValue);
        //}

        public class ParentClass
        {
        }

        public class ChildClass : ParentClass, IChildClassInterface
        {
        }

        public interface IChildClassInterface
        {
        }


        [Fact]
        public void TestToString()
        {
            Assert.Equal("", Maybe<int>.Not.ToString());
            Assert.Equal("", Maybe<string>.Not.ToString());
            Assert.Equal("1", Maybe.From(1).ToString());
            Assert.Equal("hi", Maybe.From("hi").ToString());
        }

        [Fact]
        public void TestIf()
        {
            Assert.Equal(Maybe.From("hi"), Maybe.If(true, () => "hi"));
            Assert.Equal(Maybe.From("hi"), Maybe.If(true, "hi"));
            Assert.False(Maybe.If(false, "hi").HasValue);
            Assert.False(Maybe.If(true, () => (string) null).HasValue);
            Maybe.If<string>(false, () => { throw new Exception("This should not get invoked"); });
            Assert.Equal(Maybe.From(1), Maybe.If(true, () => 1));
            Assert.Equal(Maybe.From(1), Maybe.If(true, 1));
            Assert.False(Maybe.If(false, () => 1).HasValue);
            Assert.False(Maybe.If(false, () => (int?) 1).HasValue);
            Assert.False(Maybe.If(true, () => (int?) null).HasValue);
            Assert.False(Maybe.If(true, (int?) null).HasValue);
            Maybe.If<int>(false, () => { throw new Exception("This should not get invoked"); });
            CheckArgumentNullException(() => Maybe.If(true, (Func<string>) null));
            CheckArgumentNullException(() => Maybe.If(true, (Func<int?>) null));
            CheckArgumentNullException(() => Maybe.If(false, (Func<string>) null));
            CheckArgumentNullException(() => Maybe.If(false, (Func<int?>) null));
        }

        [Fact]
        public void TestOptionalParameters()
        {
            Assert.Equal(" => 1: ", FormatInfo(1));
            Assert.Equal("1 => 2: A", FormatInfo(2, "A", 1));
        }

        private string FormatInfo(int id,
            Maybe<string> name = default(Maybe<string>),
            Maybe<int> parentId = default(Maybe<int>))
        {
            Assert.NotNull(name);
            Assert.NotNull(parentId);
            return string.Format("{2} => {0}: {1}", id, name, parentId);
        }

        [Fact]
        public void TestOptionalInterfaceParameters()
        {
            IOptionalInterface i = new OptionalImplementation();
            Assert.Equal(" => 1: ", i.FormatInfo(1));
            Assert.Equal("1 => 2: A", i.FormatInfo(2, "A", 1));
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
                Assert.NotNull(name);
                Assert.NotNull(parentId);
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