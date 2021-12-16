using PasswordHashingShowcase.Core.Extensions;
using PasswordHashingShowcase.Core.Values;
using System.Security.Cryptography;
using System.Text;

namespace PasswordHashingShowcase.Core
{
	internal static class PasswordBreaker
	{
		public static string BreakPassword(
			Password password, 
			AsciiIntervals startPoint, 
			AsciiIntervals endPoint, 
			Salt? salt = null,
			Salt? pepper = null)
		{
			int MinAscii = SetAsciiLimit(startPoint);
			int MaxAscii = SetAsciiLimit(endPoint);

			const int MaxSize = 4;

			int currentLength = 0;
			char[] currentGuess = new char[MaxSize];
			var sb = new StringBuilder();
			var match = false;

			currentGuess[0] = (char)(MinAscii - 1);

			bool NextGuess()
			{
				if (currentLength >= MaxSize)
					return false;

				IncrementDigit(currentLength);
				return true;
			}

			void IncrementDigit(int digitIndex)
			{
				if (digitIndex < 0)
					AddCharacter();

				else
				{
					if (currentGuess[digitIndex] == (char)MaxAscii)
					{
						currentGuess[digitIndex] = (char)MinAscii;
						IncrementDigit(digitIndex - 1);
					}
					else
						currentGuess[digitIndex]++;
				}
			}

			void AddCharacter()
			{
				currentLength++;

				if (currentLength >= MaxSize)
					return;

				Console.WriteLine($"Incresing guess current length. Guessing for passwords of size {currentLength + 1}{(salt is null ? "" : $" for salt {salt}")}");
				currentGuess[currentLength] = (char)MinAscii;
			}


			while (match is false)
			{
				if (NextGuess() == false)
					break;

				Password passwordGuess =
					sb.Append(currentGuess)
					.ToString().Split('\0', 2)[0]
					.Then(rawGuess => string.Concat(rawGuess, salt, pepper))
					.GetBytes()
					.Then((bytes) => SHA384.HashData(bytes))
					.GetBase64String();

				if (passwordGuess == password)
					match = true;

				else
					sb.Clear();
			}

			return sb.ToString();
		}

		private static int SetAsciiLimit(AsciiIntervals interval)
		{
			return interval switch
			{
				AsciiIntervals.BySpecialCharacters => 33,
				AsciiIntervals.ByNumbers => 48,
				AsciiIntervals.ByUppercase => 65,
				AsciiIntervals.ByLowercase => 97,
				_ => throw new NotImplementedException(),
			};
		}

	}
}
