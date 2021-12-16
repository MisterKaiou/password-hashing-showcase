using PasswordHashingShowcase.Core.Data;
using PasswordHashingShowcase.Core.Extensions;
using PasswordHashingShowcase.Core.Values;
using System.Diagnostics;
using System.Text;

namespace PasswordHashingShowcase.Core
{
	internal class PresentationHelper<TPassword> where TPassword : Password
	{
		private readonly Database<TPassword> _database;
		private readonly Func<Password, TPassword>? _passwordHashingStep;
		private readonly Action<IEnumerable<(string, string)>> _attempToBreakPasswordAction;

        public PresentationHelper(Action<IEnumerable<(string, string)>> attempToBreakPasswordAction,
                                  Func<Password, TPassword>? additionalUserAdditionStep, 
								  Database<TPassword> database)
        {
			_attempToBreakPasswordAction = attempToBreakPasswordAction;
            _passwordHashingStep = additionalUserAdditionStep;
            _database = database;
        }

        public void BeginInteractiveSession(bool clearConsole = false)
		{
			if (clearConsole)
				Console.Clear();

			Console.WriteLine("Read the repository README file if you have any questions.\n");

			Console.WriteLine("Options:");
			Console.WriteLine("1 - List database entries.");
			Console.WriteLine("2 - Add a new entry.");
			Console.WriteLine("3 - Change an entry password.");
			Console.WriteLine("4 - Attempt to break passwords.");
			Console.WriteLine("5 - Exit.");
			Console.Write("\nChoice: ");

			var selection = Console.ReadKey().KeyChar;

			if (int.TryParse(new ReadOnlySpan<char>(new[] { selection }), System.Globalization.NumberStyles.None, null, out var number) == false)
				number = -1;

			switch (number)
			{
				case -1:
					Console.WriteLine("\n\nNot a valid number");
					break;

				case 1:
					InterpretActionIfHasData(() => PrintDatabaseEntries(_database));
					break;

				case 2:
					AddUserToDatabase();
					break;

				case 3:
					InterpretActionIfHasData(() => ChangeAnEntryPassword());
					break;

				case 4:
					InterpretActionIfHasData(
						() => _attempToBreakPasswordAction(_database.Users.Select(kp => (kp.Key.Value, kp.Value.Value))));
					break;

				case 5:
					return;

				default:
					Console.WriteLine("\n\nUnknown option...");
					break;
			}

			Console.WriteLine();
			Console.Write("Press any key...");
			Console.ReadKey();
			BeginInteractiveSession(true);
		}

		private static void PrintDatabaseEntries(Database<TPassword> database)
		{
			var usernameColumnTitle = " Username";
			var passwordColumnTitle = " Password";
			var saltColumnTitle = " Salt";

			int usernameColumnSpaces;
			int passwordColumnSpaces;
			int saltColumnSpaces = 0;

			usernameColumnSpaces =
				database.Users.Select(kp => kp.Key.Value)
				.Concat(new[] { usernameColumnTitle })
				.Then(CalculateSpaceAmmount);

			passwordColumnSpaces =
				database.Users.Select(kp => kp.Value.Value)
				.Concat(new[] { passwordColumnTitle })
				.Then(CalculateSpaceAmmount);

			var header = $"|{usernameColumnTitle.PadRight(usernameColumnSpaces)}|{passwordColumnTitle.PadRight(passwordColumnSpaces)}|";

			if (typeof(TPassword) == typeof(SaltedPassword))
            {
				saltColumnSpaces =
					database.Users.Select(kp => (kp.Value as SaltedPassword).Salt.ToString())
                    .Concat(new[] { passwordColumnTitle })
                    .Then(CalculateSpaceAmmount);

                header += $"{saltColumnTitle.PadRight(saltColumnSpaces)}|";
            }

            Console.WriteLine(header);
			Console.WriteLine(new StringBuilder().Append('_', header.Length).ToString());

			foreach (var (username, password) in database.Users)
			{
				var line = $"| {username.Value.PadRight(usernameColumnSpaces - 1)}| {password.Value.PadRight(passwordColumnSpaces - 1)}|";

				if (typeof(TPassword) == typeof(SaltedPassword))
					line += $" {(password as SaltedPassword).Salt.Value.PadRight(saltColumnSpaces - 1)}";

				Console.WriteLine(line);
			}
		}

		static int CalculateSpaceAmmount(IEnumerable<string> strings)
		{
			var tabAmmount = 0;
			var tabCharTotalSpacing = 8; //8 is the equivalent number of characters in 1 tab.

			foreach (var s in strings)
			{
				var fitTimes = (s.Length + 1) / tabCharTotalSpacing;

				int ammountToAllowFit = fitTimes == 0 ? 1 : fitTimes + 1;

				if (tabAmmount < ammountToAllowFit)
					tabAmmount = ammountToAllowFit;
			}

			return tabAmmount * 8;
		}

		private void InterpretActionIfHasData(Action exec)
		{
			Console.WriteLine("\n");
			if (_database.Users.Any())
			{
				exec();
				return;
			}

			Console.WriteLine("Database is empty");
		}

		private void AddUserToDatabase()
		{
			Console.WriteLine("\n");

			Console.Write("Username: ");
			var usernameInput = Console.ReadLine();

			if (IsValidInput(usernameInput) == false)
				return;

			if (_database.Users.ContainsKey(usernameInput))
			{
				Console.WriteLine("User already exists");
				return;
			}

			Console.Write("Password: ");
			var passwordInput = Console.ReadLine();

			if (IsValidInput(passwordInput) == false)
				return;

			Debug.Assert(passwordInput is not null, "Password input is null");
			Debug.Assert(usernameInput is not null, "Username input is null");

			_database.Users.Add(usernameInput, GetPasswordHashingWhenSupported(passwordInput));

			Console.WriteLine("\nUser added.");
		}

		private void ChangeAnEntryPassword()
		{
			Console.Write("Which user password would you like to change? ");
			var username = Console.ReadLine();

			if (IsValidInput(username) == false)
				return;

			if (_database.Users.ContainsKey(username) == false)
			{
				Console.WriteLine("Username not found");
				return;
			}

			Console.Write("\nWhat password would you like to set? ");
			var newPassword = Console.ReadLine();

			if (IsValidInput(newPassword) == false)
				return;

			Debug.Assert(newPassword != null, "username was null");
			var hashedNewPassword = GetPasswordHashingWhenSupported(newPassword);

			Debug.Assert(username != null, "username was null");
			_database.Users[username] = hashedNewPassword;
		}

		private TPassword GetPasswordHashingWhenSupported(string input)
		{
			if (_passwordHashingStep != null)
				return _passwordHashingStep?.Invoke(input);
			else if (typeof(TPassword) != typeof(Password))
					throw new ArgumentNullException("Hashing step must be present when password type is not default");

			return new Password(input) as TPassword;
		}

		private static bool IsValidInput(string? input)
		{
			if (string.IsNullOrWhiteSpace(input))
			{
				Console.WriteLine("Invalid input.");
				return false;
			}

			return true;
		}
	}
}
