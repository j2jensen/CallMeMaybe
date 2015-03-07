using System.Globalization;
using CallMeMaybe;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMeMaybe
{
    [TestClass]
    public class MaybeParserTest
    {
        [TestMethod]
        public void TestIntParse()
        {
            var parser = Maybe.Parsers.FromTryParse<int>(int.TryParse);
            Assert.AreEqual(Maybe<int>.Not, parser.Parse(null));
            Assert.AreEqual(Maybe<int>.Not, parser.Parse(""));
            Assert.AreEqual(Maybe<int>.Not, parser.Parse("hi"));
            Assert.AreEqual(Maybe.From(42), parser.Parse("42"));
            Assert.AreNotEqual(Maybe.From(66), parser.Parse("42"));
            Assert.AreEqual(Maybe.Not, parser.Parse("2A"));
        }

        [TestMethod]
        public void TestIntParseWithFormat()
        {
            var parser = Maybe.Parsers.FromTryParse(((string source, out int value) =>
                int.TryParse(source, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value)));
            Assert.AreEqual(Maybe<int>.Not, parser.Parse(null));
            Assert.AreEqual(Maybe<int>.Not, parser.Parse(""));
            Assert.AreEqual(Maybe<int>.Not, parser.Parse("hi"));
            Assert.AreNotEqual(Maybe.From(42), parser.Parse("42"));
            Assert.AreEqual(Maybe.From(66), parser.Parse("42"));
            Assert.AreEqual(Maybe.From(42), parser.Parse("2A"));
        }

        [TestMethod]
        public void TestStandardParsers()
        {
            Assert.AreEqual(Maybe.From(42), Maybe.Parsers.Int32.Parse("42"));
            Assert.AreEqual(Maybe.Not, Maybe.Parsers.Int32.Parse(long.MaxValue.ToString(CultureInfo.InvariantCulture)));
            Assert.AreEqual(Maybe.From(long.MaxValue), Maybe.Parsers.Int64.Parse(long.MaxValue.ToString(CultureInfo.InvariantCulture)));
        }

    }
}
