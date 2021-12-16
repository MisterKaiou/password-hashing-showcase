using PasswordHashingShowcase.Core.Values;

namespace PasswordHashingShowcase.Core.Data
{
    internal abstract class Database<T> where T : Password
    {
        public readonly Dictionary<Username, T> Users = new();
    }

    internal class NormalDatabase : Database<Password>
    {
    }

    internal class SaltedDatabase : Database<SaltedPassword>
    {
    }
}