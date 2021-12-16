using PasswordHashingShowcase.Core;
using PasswordHashingShowcase.Core.Data;
using PasswordHashingShowcase.Core.Values;

var _database = new NormalDatabase();

var presenter = new PresentationHelper<Password>(
	AttempToBreakPassword,
	null,
	_database
);

presenter.BeginInteractiveSession();

static void AttempToBreakPassword(IEnumerable<(string, string)> data)
{
	Console.WriteLine("There is hashing so its pretty straight forward.\n");

    foreach (var (username, password) in data)
    {
		Console.WriteLine($"For user: {username} - Password is: {password}");
    }
}
