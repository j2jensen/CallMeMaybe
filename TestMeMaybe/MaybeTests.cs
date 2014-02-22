using System;
using CallMeMaybe;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMeMaybe
{
    [TestClass]
    public class MaybeTests
    {
        [TestMethod]
        public void TestUninitializedValues()
        {
            var foo = new Foo();
            Assert.IsNotNull(foo.Number, "Uninitialized values should not be null.");
            Assert.IsNotNull(foo.Number, "Uninitialized values should not be null.");
            Assert.IsFalse(foo.Number.HasValue);
        }


        public class Foo
        {
            public Maybe<int> Number { get; set; }
            public Maybe<string> Name { get; set; }
        }
    }
}
