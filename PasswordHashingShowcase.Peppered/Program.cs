using PasswordHashingShowcase.Core;
using PasswordHashingShowcase.Core.Data;
using PasswordHashingShowcase.Core.Extensions;
using PasswordHashingShowcase.Core.Values;
using System.Diagnostics;
using System.Security.Cryptography;

var _database = new SaltedDatabase();
var _pepper = new Salt();

var presenter = new PresentationHelper<SaltedPassword>(
	AttempToBreakPassword,
	HashPassword,
	_database
);

presenter.BeginInteractiveSession();

SaltedPassword HashPassword(Password passwordTohash)
{
	var salt = new Salt();
	var saltedBytes =
		(passwordTohash.Value + salt + _pepper).GetBytes()
		.Then((bytes) => SHA384.HashData(bytes));

	return new SaltedPassword(saltedBytes, salt);
}

static void AttempToBreakPassword(IEnumerable<(string, string)> data)
{
	Console.WriteLine("With a pepper, it is dozens of thousands of times harder to break a password even if salted");
	Console.WriteLine("In a real world scenario a pepper would be as strong or stronger then your salt.");
	Console.WriteLine("I believe you know now where this is going.");
	Console.WriteLine();

	var stopwatch = new Stopwatch();
	stopwatch.Start();

	foreach (var (username, password) in data)
	{
		string foundPassword = "";

		for (var s = Salt.SaltMinimumValue; s <= Salt.SaltMaximumValue && foundPassword == ""; s++)
		{
			for (var p = Salt.SaltMinimumValue; p <= Salt.SaltMaximumValue && foundPassword == ""; p++)
			{
				foundPassword =
					PasswordBreaker.BreakPassword(
						password,
						AsciiIntervals.ByNumbers,
						AsciiIntervals.ByUppercase,
						salt: new Salt(s.ToString()),
						pepper: new Salt(p.ToString())
					);

				if (foundPassword == "")
					Console.WriteLine($"No match found for pepper {p}.");

			}

			if (foundPassword == "")
				Console.WriteLine($"No match found for salt {s}.\n");
		}

		Console.WriteLine($"For user: {username} - Password is: {(foundPassword == "" ? "<NOT FOUND>" : foundPassword)}");
	}

	stopwatch.Stop();
	Console.WriteLine($"\nTotal time taken: {stopwatch.Elapsed}");
}
