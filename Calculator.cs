// Week 5 - Calculator Assignment
// Name: AnOddDoorKnight
// Submission Date: 20-Apr-2025
// Description: A simple console calculator supporting basic operations and memory

namespace OVS.SimpleCollegeCalculator
{
	using HDAdvance.Mathematics;
	using OVS.SimpleCollegeCalculator.Menus;
	using OVS.SimpleCollegeCalculator.Operators;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public class Calculator
	{
		public static Calculator? CalculatorInstance { get; set; }

		public static void Main(string[] args)
		{
			Console.Title = "AnOddDoorkKnight's OVSCalculator";
			CalculatorInstance = new Calculator();
			Exception? ex = CalculatorInstance.Run();
			if (ex != null)
			{
				Console.Clear();
				Console.WriteLine($"An Error has occured:\n{ex}");
			}
			Console.ReadKey();
		}

		/* --------------------------------------------------------------- */

		public Submenu MainMenu { get; set; }
		public UserInterface UserInterface { get; set; }
		public Memory Memory { get; set; }

		public Calculator()
		{
			MainMenu = new Submenu("Main Menu")
			{
				interactibles = [
					new Submenu("Basic Math Functions") { interactibles = [
						IInteractible.ToIInteractible(new Add()),
						IInteractible.ToIInteractible(new Substract()),
						IInteractible.ToIInteractible(new Multiply()),
						IInteractible.ToIInteractible(new Divide()),
						new BackInteract(),
					]},
					new Submenu("Area Functions") { interactibles = [
						IInteractible.ToIInteractible(new AreaOfCircle()),
						IInteractible.ToIInteractible(new AreaOfRectangle()),
						new BackInteract(),
					]},
					new Submenu("Volume Functions") { interactibles = [
						new BackInteract(),
					]},
					new Submenu("Trig Functions") { interactibles = [
						IInteractible.ToIInteractible(new TriangleTheroem()),
						new BackInteract(),
					]},
					IInteractible.ToIInteractible(new ResetMemory()),
					IInteractible.ToIInteractible(new Quit()),
				],
			};
			UserInterface = new(this);
			Memory = new();
		}

		public Exception? Run()
		{
			try
			{
				while (true) // it will be broken by environment exit
				{
					MainMenu.EnterWithoutAnnouncing();
				}
			}
			catch (Exception ex) when (ex.Message == "Quit")
			{
				return null;
			}
			catch (Exception ex)
			{
				return ex;
			}
		}
	}

	public class Memory
	{
		public float? Value { get; set; }

		public void AddTo(float with) => SetTo((Value ?? 0f) + with);
		public void RemoveTo(float with) => SetTo((Value ?? 0f) - with);
		public void SetTo(float with)
		{
			Value = with;
			Calculator.CalculatorInstance!.UserInterface.Conditions[typeof(Memory).Name.GetHashCode()] =
				$"Memory: {Value?.ToString() ?? "0"}";
		}

		public void CheckUserForInterfaceInteraction(float result)
		{
			bool yes = InputUtility.ReadUserInputKey("Save as memory? (y/n): ",
				key => key.KeyChar is 'y' or 'n') == 'y';
			if (yes)
			{
				SetTo(result);
			}
		}
	}

	public class UserInterface
	{
		public const string SEPARATOR = "======================-";
		public const string SEPARATOR_MINI = "-----------------------";
		public static UserInterface? Instance { get; private set; }

		public string AsciiView { get; set; } = string.Empty;
		public string InputView { get; set; } = string.Empty;
		public Dictionary<int, string> Conditions = new();

		private Calculator calculator;

		public string ConsoleView { get; private set; } = string.Empty;

		public UserInterface(Calculator calculator)
		{
			this.calculator = calculator;
			Instance = this;
		}

		public void Reprint()
		{
			Console.Clear();
			StringBuilder stringBuilder = new($"{AsciiView}\n{SEPARATOR}\n{InputView}");
			if (Conditions.Count > 0)
			{
				stringBuilder.Insert(0, $"{SEPARATOR_MINI}\n");
				using var enumerator = Conditions.GetEnumerator(); // i REFUSE to use a foreach loop >:(
				while (enumerator.MoveNext())
				{
					stringBuilder.Insert(0, $"{enumerator.Current.Value}\n");
				}
			}
			Console.WriteLine(stringBuilder.ToString());
			ConsoleView = stringBuilder.ToString();
		}
	}

	public static class InputUtility
	{
		public static string ReadUserInputLine(string initialResponse, params Func<string, bool>[] conditions)
		{
			string input;
			PrintCondition(initialResponse, false);
			while (true)
			{
				input = Console.ReadLine() ?? string.Empty;
				if (conditions.All(con => con(input)))
				{
					break;
				}
				PrintCondition(initialResponse, true);
			}
			return input;
		}
		public static char ReadUserInputKey(string initialResponse, params Func<ConsoleKeyInfo, bool>[] conditions)
		{
			ConsoleKeyInfo info;
			PrintCondition(initialResponse, false);
			while (true)
			{
				info = Console.ReadKey();
				if (conditions.All(con => con(info)))
				{
					break;
				}
				PrintCondition(initialResponse, true);
			}
			return info.KeyChar;
		}
		public static void PrintCondition(string initialResponse, bool retry)
		{
			UserInterface.Instance!.InputView = !retry ? initialResponse
				: $"Invalid Response, try again.\n{initialResponse}";
			UserInterface.Instance.Reprint();
		}
	}
}

namespace OVS.SimpleCollegeCalculator.Menus
{
	using HDAdvance.Mathematics;
	using OVS.SimpleCollegeCalculator.Operators;
	using System;
	using System.Collections.Generic;
	using System.Text;

	public class Submenu(string Name) : IInteractible
	{
		string IInteractible.Name => Name;
		public IReadOnlyList<IInteractible> interactibles = Array.Empty<IInteractible>();
		public void Enter()
		{
			int hash = Random.Shared.Next(int.MinValue, int.MaxValue);
			UserInterface.Instance!.Conditions.Add(hash, $"Submenu: {Name}");
			EnterWithoutAnnouncing();
			UserInterface.Instance!.Conditions.Remove(hash);
		}
		public ReturnType Invoke(Calculator pairedCalculator, out object output)
		{
			Enter();
			output = default;
			return ReturnType.Continue;
		}
		public void EnterWithoutAnnouncing()
		{
			Range<int> range = interactibles.GetRangeFromCollection();
			UserInterface.Instance!.AsciiView = ToInteractiblesString();
			UserInterface.Instance!.Reprint();
			// Error handling
			int choice = int.Parse(InputUtility.ReadUserInputKey("Select a key: ",
				info => char.IsDigit(info.KeyChar)
				&& range.InRangeInclusive(int.Parse(info.KeyChar.ToString()) - 1)).ToString());
			switch (interactibles[choice - 1].Invoke(Calculator.CalculatorInstance!, out object output))
			{
				default:
				case ReturnType.Continue:
				{

				}
				break;
				case ReturnType.Operation:
				{
					float savedFloat = (float)output;
					if (!float.IsNaN(savedFloat))
						Calculator.CalculatorInstance!.Memory.CheckUserForInterfaceInteraction(savedFloat);
				}
				break;
			}
		}


		public string ToInteractiblesString()
		{
			StringBuilder @out = new();
			for (int i = 0; i < interactibles.Count; i++)
			{
				@out.AppendLine($"{i + 1}) {interactibles[i].Name}");
			}
			return @out.ToString();
		}
	}

	public class BackInteract : IInteractible
	{
		public string Name => "Back";

		public ReturnType Invoke(Calculator pairedCalculator, out object output)
		{
			output = default;
			return ReturnType.Continue;
		}
	}

	public interface IInteractible
	{
		private readonly record struct FlexibleInteractible(IOperation Operatioin) : IInteractible
		{
			public string Name => Operatioin.Name;
			public readonly ReturnType Invoke(Calculator pairedCalculator, out object output)
			{
				output = Operatioin.Execute(pairedCalculator, pairedCalculator.Memory.Value);
				return ReturnType.Operation;
			}
		}
		public static IInteractible ToIInteractible(IOperation operation)
			=> new FlexibleInteractible(operation);
		ReturnType Invoke(Calculator pairedCalculator, out object output);
		string Name { get; }
	}
	public enum ReturnType : byte
	{
		Continue,
		Operation
	}
}

namespace OVS.SimpleCollegeCalculator.Operators
{
	using HDAdvance.Mathematics;
	using OVS.SimpleCollegeCalculator.Menus;
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	public interface IOperation
	{
		public static float DefineValueByUser()
		{
			string input = InputUtility.ReadUserInputLine("Get Value: ", value => float.TryParse(value, out _));
			return float.Parse(input);
		}

		string Name => this.GetType().Name;
		float Execute(Calculator parentCalculator, float? firstInput);
	}

	public sealed class Add : IOperation
	{
		string IOperation.Name => "Addition";
		public string MakeAsciiArt(float? A, float? B) =>
	$@"
	  {(A is null ? "__" : A)}
	+ {(B is null ? "__" : B)}
	─────
	{((A is null || B is null) ? "" : A + B)}
";
		public float Execute(Calculator parentCalculator, float? firstInput)
		{
			UserInterface.Instance!.AsciiView = MakeAsciiArt(null, null); 
			UserInterface.Instance.Reprint();
			float A = firstInput ?? IOperation.DefineValueByUser();
			UserInterface.Instance!.AsciiView = MakeAsciiArt(A, null); 
			UserInterface.Instance.Reprint();
			float B = IOperation.DefineValueByUser();
			UserInterface.Instance!.AsciiView = MakeAsciiArt(A, B); 
			UserInterface.Instance.Reprint();
			return A + B;
		}
	}
	public sealed class Substract : IOperation
	{
		string IOperation.Name => "Subtraction";
		public string MakeAsciiArt(float? A, float? B) =>
	$@"
	  {(A is null ? "__" : A)}
	- {(B is null ? "__" : B)}
	─────
	{((A is null || B is null) ? "" : A - B)}
";
		public float Execute(Calculator parentCalculator, float? firstInput)
		{
			UserInterface.Instance!.AsciiView = MakeAsciiArt(null, null); 
			UserInterface.Instance.Reprint();
			float A = firstInput ?? IOperation.DefineValueByUser();
			UserInterface.Instance!.AsciiView = MakeAsciiArt(A, null); 
			UserInterface.Instance.Reprint();
			float B = IOperation.DefineValueByUser();
			UserInterface.Instance!.AsciiView = MakeAsciiArt(A, B); 
			UserInterface.Instance.Reprint();
			return A - B;
		}
	}
	public sealed class Multiply : IOperation
	{
		string IOperation.Name => "Multiplication";
		public string MakeAsciiArt(float? A, float? B) =>
	$@"
	  {(A is null ? "__" : A)}
	* {(B is null ? "__" : B)}
	─────
	{((A is null || B is null) ? "" : A * B)}
";
		public float Execute(Calculator parentCalculator, float? firstInput)
		{
			UserInterface.Instance!.AsciiView = MakeAsciiArt(null, null); 
			UserInterface.Instance.Reprint();
			float A = firstInput ?? IOperation.DefineValueByUser();
			UserInterface.Instance!.AsciiView = MakeAsciiArt(A, null); 
			UserInterface.Instance.Reprint();
			float B = IOperation.DefineValueByUser();
			UserInterface.Instance!.AsciiView = MakeAsciiArt(A, B);
			UserInterface.Instance.Reprint();
			return A * B;
		}
	}
	/// <summary>
	/// Resets memory in the menu as an operation
	/// </summary>
	public sealed class ResetMemory : IOperation
	{
		string IOperation.Name => "Reset Memory";
		public float Execute(Calculator parentCalculator, float? firstInput)
		{
			if (parentCalculator.Memory.Value == null)
			{
				return float.NaN;
			}
			parentCalculator.Memory.Value = null;
			UserInterface.Instance!.Conditions.Remove(typeof(Memory).Name.GetHashCode());
			return float.NaN;
		}
	}
	/// <summary>
	/// Quick quit.
	/// </summary>
	public sealed class Quit : IOperation
	{
		public float Execute(Calculator parentCalculator, float? firstInput)
		{
			Environment.Exit(0);
			throw new Exception();
		}
	}

	/// <summary>
	/// Divides and visualizes the division of one number by another.
	/// </summary>
	public sealed class Divide : IOperation
	{
		string IOperation.Name => "Division";
		/// <summary>
		/// Returns a <see langword="string"/> designed for being print out by 
		/// <see cref="UserInterface"/>. Visualizes as a long division problem.
		/// </summary>
		/// <param name="numerator">The Numerator</param>
		/// <param name="denominator">The Denominator</param>
		/// <returns></returns>
		public static string MakeAsciiArt(float? numerator, float? denominator)
		{
			string StrA = numerator is null ? "__" : numerator.ToString()!,
				StrB = denominator is null ? "__" : denominator.ToString()!,
				StrC = (numerator is null || denominator is null) ? "" : (numerator.Value / denominator.Value).ToString();
			return $@"
	{AddInitialLength()}   {StrC}
	{AddInitialLength()} _{string.Join("", Enumerable.Repeat('_', Math.Max(StrA.Length, StrC.Length)))}
	{StrB}/{StrA}
";
			string AddInitialLength() => string.Join("", Enumerable.Repeat(' ', StrB.Length));
		}


		public float Execute(Calculator parentCalculator, float? firstInput)
		{
			UserInterface.Instance!.AsciiView = MakeAsciiArt(null, null);
			UserInterface.Instance.Reprint();
			float A = firstInput ?? IOperation.DefineValueByUser();
			UserInterface.Instance!.AsciiView = MakeAsciiArt(A, null); 
			UserInterface.Instance.Reprint();
			float B = IOperation.DefineValueByUser();
			if (B == 0f)
			{
				ThrowDivideByZeroError();
				Environment.Exit(0); // Just in case, not an official way to return
				return float.NaN;
			}
			UserInterface.Instance!.AsciiView = MakeAsciiArt(A, B); 
			UserInterface.Instance.Reprint();
			return A / B;
		}

		/// <summary>
		/// Implements custom graphics sort of like the screen glitching whenever
		/// the user tries to divide by 0 for any reason. Calls <see cref="Environment.Exit(int)"/>
		/// </summary>
		public void ThrowDivideByZeroError()
		{
			const int horizontal = 100, vertical = 30;
			char[] allJunkChars = ['@', '#', '%', '&'];
			List<string> view = [.. UserInterface.Instance!.ConsoleView.Split("\n")];
			for (int i = 0; i < vertical; i++)
			{
				if (view.Count <= i)
				{
					view.Add(AddSpaces(null));
				}
				else
				{
					view[i] = AddSpaces(view[i]);
				}
			}
			StringBuilder modifiableView = new(string.Join("\n", view));
			Stopwatch stopwatch = Stopwatch.StartNew(); 
			long stopAt = (long)TimeSpan.FromSeconds(5D).TotalMilliseconds;
			Range<double> range = new(0F, 5F);
			for (int modify = 1; stopwatch.ElapsedMilliseconds < stopAt; modify++)
			{
				Console.Clear();
				for (int i = 0; i < modify; i++)
				{
					int randomIndex = Random.Shared.Next(modifiableView.Length);
					if (modifiableView[randomIndex] == '\n')
					{
						continue;
					}
					modifiableView.Remove(randomIndex, 1);
					modifiableView.Insert(randomIndex, allJunkChars[Random.Shared.Next(allJunkChars.Length)]);
				}
				Console.Write(modifiableView.ToString());
				double waitDelay = 120D - Math.Pow(1D, 1D + (range.ToNormalize(stopwatch.Elapsed.TotalSeconds) * 1000D));
				Task.Delay((int)waitDelay).Wait();
			}
			Environment.Exit(0);

			static string AddSpaces(string? text) => 
				$"{text ?? ""}{string.Join("", Enumerable.Repeat(' ', horizontal - (text?.Length ?? 0)))}";
		}
	}

	// A length * B length
	public sealed class AreaOfRectangle : IOperation
	{
		string IOperation.Name => "Area Of Rectangle";
		public string MakeAsciiArt(float? A, float? B)
		{
			int X = FindVisualLength(A, 1.3f), Y = FindVisualLength(B, 2.0f);
			string answer = (A is null || B is null) ? "" : (A! * B!).ToString()!;
			if (answer.Length > X)
				X = answer.Length;

			StringBuilder sb = new($"{GetBorderLine()}\n");
			for (int i = 0; i < Y; i++)
			{
				StringBuilder line = new(GetMiddleLine());
				if (i == (Y / 2))
				{
					int removeFrom = answer.Length;
					int middleIndex = (line.Length + 4) / 2;
					line = line.Remove(middleIndex / 2, removeFrom).Insert(middleIndex / 2, answer);
					line.Append($" {(B is null ? "__" : B)}");
				}
				sb.AppendLine(line.ToString());
			}
			sb.AppendLine(GetBorderLine());
			sb.AppendLine($"\t{string.Join("", Enumerable.Repeat(' ', X / 2))}{(A is null ? "__" : A)}");
			return sb.ToString();

			// Work on getting it exponential
			static int FindVisualLength(float? value, float @base = 1.5f, int min = 2)
			{
				if (value is null || value <= 0)
					return min;
				return (int)MathF.Log(value.Value, @base);
			}

			string GetMiddleLine() => $"\t|{string.Join("", Enumerable.Repeat(' ', X))}|";
			string GetBorderLine() => $"\t+{string.Join("", Enumerable.Repeat('-', X))}+";
		}

		public float Execute(Calculator parentCalculator, float? firstInput)
		{
			UserInterface.Instance!.AsciiView = MakeAsciiArt(null, null); 
			UserInterface.Instance.Reprint();
			float A = firstInput ?? IOperation.DefineValueByUser();
			UserInterface.Instance!.AsciiView = MakeAsciiArt(A, null); 
			UserInterface.Instance.Reprint();
			float B = IOperation.DefineValueByUser();
			UserInterface.Instance!.AsciiView = $"{A} * {B} = {A * B}\n" + MakeAsciiArt(A, B); 
			UserInterface.Instance.Reprint();
			return A * B;
		}
	}

	// radius * radius * Pi
	public sealed class AreaOfCircle : IOperation
	{
		string IOperation.Name => "Area Of Circle";
		public string MakeAsciiArt(float? A) =>
	$@"
	radius: {(A is null ? "__" : A)}
	     ***
	  **     **
	 *         *
	*           *
	*     ●     * 
	*           *
	 *         *
	  **     **
	     ***
	{((A is null) ? "__" : CalcAnswer(A.Value))} x^2
";
		public float Execute(Calculator parentCalculator, float? firstInput)
		{
			UserInterface.Instance!.AsciiView = MakeAsciiArt(null); 
			UserInterface.Instance.Reprint();
			float radius = firstInput ?? IOperation.DefineValueByUser();
			UserInterface.Instance!.AsciiView = $"{radius}^2 * Pi = {CalcAnswer(radius)}\n" + MakeAsciiArt(radius); 
			UserInterface.Instance.Reprint();
			return CalcAnswer(radius);
		}

		public static float CalcAnswer(float radius) =>
			radius * radius * MathF.PI;
	}

	// A^2 + B^2 = C^2
	public sealed class TriangleTheroem : IOperation
	{
		string IOperation.Name => "Calculate Phythagoreans Theorem";
		public static float CalculateHypotenuse(float A, float B)
			=> MathF.Sqrt(MathF.Pow(A, 2) + MathF.Pow(B, 2));
		public string MakeAsciiArt(float? A, float? B)
		{
			string AStr = A is null ? "__" : A.Value.ToString(),
				BStr = B is null ? "__" : B.Value.ToString(),
				CStr = (A is null || B is null) ? "" : CalculateHypotenuse(A.Value, B.Value).ToString();
			return $@"
	{AddSpacing(CStr)}       /|
	{AddSpacing(CStr)}      / |
	{AddSpacing(CStr)}     /  |
	{CStr}    /   | {AStr}
	{AddSpacing(CStr)}   /    |
	{AddSpacing(CStr)}  /     |
	{AddSpacing(CStr)} /      |
	{AddSpacing(CStr)}/_______|
	  {AddSpacing(CStr)}{BStr}
	
";
		}

		internal string AddSpacing(string resultText) => string.Join("", Enumerable.Repeat(' ', resultText.Length));
		public float Execute(Calculator parentCalculator, float? firstInput)
		{
			UserInterface.Instance!.AsciiView = MakeAsciiArt(null, null); 
			UserInterface.Instance.Reprint();
			float A = firstInput ?? IOperation.DefineValueByUser();
			UserInterface.Instance!.AsciiView = MakeAsciiArt(A, null); 
			UserInterface.Instance.Reprint();
			float B = IOperation.DefineValueByUser();
			UserInterface.Instance!.AsciiView = 
				$"√({A}^2 + {B}^2)=√({A * A} + {B * B})=√({(A * A) + (B * B)})={CalculateHypotenuse(A, B)}\n" + 
				MakeAsciiArt(A, B); 
			UserInterface.Instance.Reprint();
			return CalculateHypotenuse(A, B);
		}
	}
}

// Pulled out of this from an external library, used for the operators
namespace HDAdvance.Mathematics
{
	using System;
	using System.Collections.Generic;
	using System.Numerics;

	/// <summary>
	/// Shows a range between two values.
	/// </summary>
	[Serializable]
	public struct Range<TNumber> where TNumber : INumber<TNumber>
	{
		public static Range<TNumber> operator -(Range<TNumber> number, TNumber with)
		{
			return new Range<TNumber>(number.Min - with, number.Max - with);
		}
		public static Range<TNumber> operator +(Range<TNumber> number, TNumber with)
		{
			return new Range<TNumber>(number.Min + with, number.Max + with);
		}
		public static Range<TNumber> operator *(Range<TNumber> number, TNumber with)
		{
			return new Range<TNumber>(number.Min * with, number.Max * with);
		}
		public static Range<TNumber> operator /(Range<TNumber> number, TNumber with)
		{
			return new Range<TNumber>(number.Min / with, number.Max / with);
		}


		/// <summary>
		/// The Minimum Value of the range
		/// </summary>
		public TNumber Min
		{
			readonly get => _min;
			set => _min = value;
		}
		private TNumber _min;
		/// <summary>
		/// The Maximum Value of the range
		/// </summary>
		public TNumber Max
		{
			readonly get => _max;
			set => _max = value;
		}
		private TNumber _max;
		/// <summary>
		/// The Difference between <see cref="Max"/> and <see cref="Min"/>
		/// </summary>
		public readonly TNumber Difference => Max - Min;

		public Range()
		{
			_min = TNumber.Zero;
			_max = TNumber.Zero;
		}
		public Range(TNumber min, TNumber max) : this()
		{
			this.Min = min;
			this.Max = max;
		}

		/// <summary>
		/// If the value specified is within range of <see cref="Min"/> and
		/// <see cref="Max"/>.
		/// </summary>
		/// <param name="value"> The value to check if it is in range. </param>
		public readonly bool InRangeExclusive(TNumber value)
		{
			return value > Min && value < Max;
		}
		/// <summary>
		/// If the value specified is within range of <see cref="Min"/> and
		/// <see cref="Max"/>.
		/// </summary>
		/// <param name="value"> The value to check if it is in range. </param>
		public readonly bool InRangeInclusive(TNumber value)
		{
			return value >= Min && value <= Max;
		}
		/// <summary>
		/// Converts a raw value into a normalized value. If in range, returns 0.
		/// However if out of the range, it will return the normalized value. Negative
		/// if its less than, greater than 0 if greater than.
		/// </summary>
		/// <param name="input"> A raw value. </param>
		public readonly TNumber OverNormalize(TNumber input)
		{
			if (InRangeExclusive(input))
				return TNumber.Zero;
			TNumber normalized = ToNormalize(input);
			if (normalized > TNumber.Zero)
				normalized -= TNumber.One;
			return normalized;
		}
		/// <summary>
		/// Converts a raw value into a normalized value. If in range, it will be
		/// in between 0 and 1.
		/// </summary>
		/// <param name="input"> A raw value. </param>
		public readonly TNumber ToNormalize(TNumber input)
		{
			TNumber difference = input - Min;
			difference /= Difference;
			return difference;
		}
		/// <summary>
		/// Converts a normalized value into a value that is determined by the range.
		/// 0 typically only returns <see cref="Min"/>, and 1 returns <see cref="Max"/>
		/// </summary>
		/// <param name="percentage"> A normalized Value. </param>
		public readonly TNumber FromNormalize(TNumber percentage)
		{
			return Min + (Difference * percentage);
		}
		public readonly TNumber Clamp(TNumber input)
		{
			return TNumber.Clamp(input, Min, Max);
		}

		/// <summary>
		/// Converts a raw value into a normalized value. it will always be
		/// in between 0 and 1.
		/// </summary>
		/// <param name="input"> A raw value. </param>
		public readonly TNumber ToNormalize01(TNumber input)
		{
			if (input < Min)
				return TNumber.Zero;
			if (input > Max)
				return TNumber.One;
			return ToNormalize(input);
		}
		/// <summary>
		/// Converts a normalized value into a value that is determined by the range.
		/// 0 and below only returns <see cref="Min"/>, and 1 and above returns <see cref="Max"/>
		/// </summary>
		/// <param name="percentage"> A normalized Value. </param>
		public readonly TNumber FromNormalize01(TNumber percentage)
		{
			if (percentage <= TNumber.Zero)
				return Min;
			if (percentage >= TNumber.One)
				return Max;
			return FromNormalize(percentage);
		}
	}

	public static class RangeUtility
	{
		public static Range<int> GetRangeFromCollection<T>(this IReadOnlyList<T> collection)
		{
			return new Range<int>(0, collection.Count - 1);
		}
	}
}