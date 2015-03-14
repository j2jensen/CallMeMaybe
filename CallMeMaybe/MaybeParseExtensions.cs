namespace CallMeMaybe
{
	public static class MaybeParseExtensions
	{
		private static readonly MaybeTryParseWrapper<int> Int32Parser = WrapTryParse<int>(int.TryParse);
		private static readonly MaybeTryParseWrapper<long> Int64Parser = WrapTryParse<long>(long.TryParse);
		private static readonly MaybeTryParseWrapper<bool> BooleanParser = WrapTryParse<bool>(bool.TryParse);

		public static Maybe<int> ParseInt32(this Maybe<string> s)
		{
			return s.SelectMany(Int32Parser.Parse);
		}

		public static Maybe<long> ParseInt64(this Maybe<string> s)
		{
			return s.SelectMany(Int64Parser.Parse);
		}

		public static Maybe<bool> ParseBoolean(this Maybe<string> s)
		{
			return s.SelectMany(BooleanParser.Parse);
		}

		private static MaybeTryParseWrapper<T> WrapTryParse<T>(Delegates<T>.TryParse tryParse)
		{
			return new MaybeTryParseWrapper<T>(tryParse);
		}
	}
}