using PasswordHashingShowcase.Core;
using PasswordHashingShowcase.Core.Data;
using PasswordHashingShowcase.Core.Extensions;
using PasswordHashingShowcase.Core.Values;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

var _database = new NormalDatabase();

var presenter = new PresentationHelper<Password>(
    AttempToBreakPassword,
    HashPassword,
    _database
);

presenter.BeginInteractiveSession();

static HashedPassword HashPassword(Password passwordTohash)
{
    return
        passwordTohash.Value.GetBytes()
        .Then((bytes) => SHA384.HashData(bytes));
}

static void AttempToBreakPassword(IEnumerable<(string, string)> data)
{
    Console.WriteLine("This is a bit more tricky. Since its hashed we have to find out what password would generate that hash.");
	Console.WriteLine("But that's not a problem either.");

	var stopwatch = new Stopwatch();
	stopwatch.Start();

	foreach (var (username, password) in data)
	{
		var foundPassword = PasswordBreaker.BreakPassword(password, AsciiIntervals.ByNumbers, AsciiIntervals.ByUppercase);

		Console.WriteLine();
		Console.WriteLine($"For user: {username} - Password is: {foundPassword}");
	}

	stopwatch.Stop();
	Console.WriteLine($"\nTotal time taken: {stopwatch.Elapsed}");
}
