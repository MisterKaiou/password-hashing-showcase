namespace PasswordHashingShowcase.Core.Values
{
	internal readonly record struct Salt
	{
		private static readonly Random _random = new();

		public const int SaltMinimumValue = 11;
		public const int SaltMaximumValue = 99;

		public readonly string Value;

		public Salt()
		{
			Value = _random.Next(SaltMinimumValue, SaltMaximumValue).ToString();
		}

		public Salt(string s)
		{
			Value = s;
		}

		public static implicit operator string(Salt salt) => salt.Value;
		public override string ToString() => Value;
	}
}
