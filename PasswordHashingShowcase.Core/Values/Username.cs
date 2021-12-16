namespace PasswordHashingShowcase.Core.Values
{
	internal readonly record struct Username
	{
		public readonly string Value;

		public Username(string name)
		{
			Value = name;
		}

		public static implicit operator string(Username username) => username.Value;
		public static implicit operator Username(string username) => new(username);

		override public string ToString() => Value;
	}
}
