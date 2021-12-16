using PasswordHashingShowcase.Core;
using PasswordHashingShowcase.Core.Data;
using PasswordHashingShowcase.Core.Extensions;
using PasswordHashingShowcase.Core.Values;
using System.Diagnostics;
using System.Security.Cryptography;

var _database = new SaltedDatabase();

var presenter = new PresentationHelper<SaltedPassword>(
	AttempToBreakPassword,
	HashPassword,
	_database
);

presenter.BeginInteractiveSession();

static SaltedPassword HashPassword(Password passwordTohash)
{
	var salt = new Salt();
	var saltedBytes = 
		(passwordTohash.Value + salt).GetBytes()
		.Then((bytes) => SHA384.HashData(bytes));

	return new SaltedPassword(saltedBytes, salt);
}

static void AttempToBreakPassword(IEnumerable<(string, string)> data)
{
	Console.WriteLine("Here it starts to become infeasible to try to break the password");
	Console.WriteLine("In a real world scenario, passwords would be much longer and salts way more complex");
	Console.WriteLine();

	var stopwatch = new Stopwatch();
	stopwatch.Start();

	foreach (var (username, password) in data)
	{
		string foundPassword = "";

		for (var i = Salt.SaltMinimumValue; i <= Salt.SaltMaximumValue && foundPassword == ""; i++)
		{
			foundPassword = 
				PasswordBreaker.BreakPassword(
					password, 
					AsciiIntervals.ByNumbers, 
					AsciiIntervals.ByUppercase,
					salt: new Salt(i.ToString())
				);

			if (foundPassword == "")
				Console.WriteLine($"No match found for salt {i}.\n");
		}

		Console.WriteLine($"For user: {username} - Password is: {(foundPassword == "" ? "<NOT FOUND>" : foundPassword)}");
	}

	stopwatch.Stop();
	Console.WriteLine($"\nTotal time taken: {stopwatch.Elapsed}");
}
