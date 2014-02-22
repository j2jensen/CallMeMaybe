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

        // TODO: Test implicit casting

        // TODO: Test LINQ-style operators (including multiple selectManys)

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
