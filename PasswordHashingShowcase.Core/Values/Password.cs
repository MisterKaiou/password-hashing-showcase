using PasswordHashingShowcase.Core.Extensions;

namespace PasswordHashingShowcase.Core.Values
{
	internal record class Password
	{
		public string Value { get; }

		public Password(string value)
		{
			Value = value;
		}

		public static implicit operator string(Password password) => password.Value;
		public static implicit operator Password(string value) => new(value);

		public virtual bool Equals(Password? other)
        {
			if (other == null)
				return false;
			if (ReferenceEquals(this, other))
				return true;
			return Value == other.Value;
        }

		override public string ToString() => Value;
	}

	internal record class HashedPassword : Password
	{
		public byte[] Bytes { get; }

		public HashedPassword(byte[] hash) : base(hash.GetBase64String())
		{
			Bytes = hash;
		}

		public static implicit operator byte[](HashedPassword password) => password.Bytes;
		public static implicit operator HashedPassword(byte[] hash) => new(hash);

		public override string ToString() => base.ToString();
	}

	internal record class SaltedPassword : HashedPassword
	{
		public Salt Salt { get; }

		public SaltedPassword(byte[] bytes, Salt salt) : base(bytes)
        {
            Salt = salt;
        }

		override public string ToString() => base.ToString();
	}
}
