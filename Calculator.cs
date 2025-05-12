// Week 7 - Calculator Assignment
// Name: AnOddDoorKnight
// Submission Date: 12-May-2025
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
	using System.Threading;

	/// <summary>
	/// Main interface, structured like a singleton and treated as such so keep
	/// in mind.
	/// </summary>
	public class Calculator
	{
		public static Calculator? CalculatorInstance { get; set; }

		public static void Main()
		{
			Console.Title = "AnOddDoorkKnight's OVSCalculator";
			CalculatorInstance = new Calculator();
			try
			{
				CalculatorInstance.MainMenu.Enter();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"\n\n{ex}\n\nPress any key to continue.");
				Console.ReadKey();
			}
		}

		/* --------------------------------------------------------------- */

		/// <summary>
		/// Main menu, stores all the submenus and some misc functions
		/// </summary>
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
						IInteractible.ToIInteractible(new Factorial()),
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
						new TriangleTheoremSet(),
						new BackInteract(),
					]},
					IInteractible.ToIInteractible(new ResetMemory()),
					new Quit(),
				],
				ShowAsSubmenu = false,
			};
			UserInterface = new();
			Memory = new();
		}
	}

	/// <summary>
	/// Stores variables to quickly be applied as the first thing for most
	/// <see cref="IOperation"/>s.
	/// </summary>
	public class Memory
	{
		public float? Value { get; set; }

		public void AddTo(float with) => SetTo((Value ?? 0f) + with);
		public void RemoveTo(float with) => SetTo((Value ?? 0f) - with);
		/// <summary>
		/// Sets the info and displays the information to <see cref="UserInterface"/>
		/// </summary>
		/// <param name="with">variable to set to.</param>
		public void SetTo(float with)
		{
			with = MathF.Round(with);
			Value = with;
			Calculator.CalculatorInstance!.UserInterface.Conditions[typeof(Memory).Name.GetHashCode()] =
				$"Memory: {Value?.ToString() ?? "0"}";
		}

		/// <summary>
		/// Quick method to quickly allow the user to save any resulting float
		/// to memory.
		/// </summary>
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


	/// <summary>
	/// The calculator's way of displaying information. Separates views and
	/// info to be easily read by the end user.
	/// </summary>
	public class UserInterface
	{
		public const string SEPARATOR = "======================-";
		public const string SEPARATOR_MINI = "-----------------------";
		public static UserInterface? Instance { get; private set; }

		/// <summary>
		/// Main view for operations and such. Simply a block of text that has
		/// to be manually set.
		/// </summary>
		public string AsciiView { get; set; } = string.Empty;
		/// <summary>
		/// The input view by the user below <see cref="AsciiView"/>.
		/// </summary>
		public string InputView { get; set; } = string.Empty;
		/// <summary>
		/// A bunch of conditions and information to display through the string
		/// provided. Can use hashcodes to differentiate and easily grab and modify
		/// the desired condition.
		/// </summary>
		public Dictionary<int, string> Conditions = [];

		/// <summary>
		/// The actual view of the console, after its done reprinting.
		/// </summary>
		public string ConsoleView { get; private set; } = string.Empty;

		public UserInterface()
		{
			Instance = this;
		}

		/// <summary>
		/// Clears and reprints all the information to the console; Effectively
		/// as the 'next frame'.
		/// </summary>
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
		/// <summary>
		/// Fancy way of writing <see cref="Console.ReadLine"/>.
		/// </summary>
		/// <param name="initialResponse">Initial response as for input.</param>
		/// <param name="conditions">Any additional in-house conditions to check</param>
		/// <returns>The now valid <see langword="string"/> from the user.</returns>
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

		/// <summary>
		/// Fancy way of writing <see cref="Console.ReadKey()"/>.
		/// </summary>
		/// <param name="initialResponse">Initial response as for input.</param>
		/// <param name="conditions">Any additional in-house conditions to check</param>
		/// <returns>The now valid <see langword="char"/> from the user.</returns>
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
		/// <summary>
		/// Prints the extra info to make the input thingy look pretty. Considers
		/// including 'try again' when <paramref name="retry"/> is <see langword="true"/>.
		/// </summary>
		/// <param name="initialResponse">The messasge to display.</param>
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

	/// <summary>
	/// A collection of various <see cref="IInteractible"/>s or <see cref="IOperation"/>s
	/// that the user can interact with, separated by a numerical system that itself
	/// handles.
	/// </summary>
	/// <param name="Name">The name of the submenu that is displayed when appropriate</param>
	public class Submenu(string Name) : IInteractible
	{
		string IInteractible.Name => Name;
		public bool Repeat { get; set; } = true;
		public bool ShowAsSubmenu { get; set; } = true;
		public IReadOnlyList<IInteractible> interactibles = [];
		/// <summary>
		/// Similar to <see cref="Enter"/>, but stores its submenu as a 
		/// <see cref="UserInterface.Conditions"/> for user clarity
		/// </summary>
		public ReturnType Enter()
		{
			int hash = Random.Shared.Next(int.MinValue, int.MaxValue);
			if (ShowAsSubmenu)
			{
				UserInterface.Instance!.Conditions.Add(hash, $"Submenu: {Name}");
			}

			Range<int> range = interactibles.GetRangeFromCollection();
			UserInterface.Instance!.AsciiView = ToInteractiblesString();
			UserInterface.Instance!.Reprint();
			// Error handling
			int choice = int.Parse(InputUtility.ReadUserInputKey("Select a key: ",
				info => char.IsDigit(info.KeyChar)
				&& range.InRangeInclusive(int.Parse(info.KeyChar.ToString()) - 1)).ToString());
			object? @out = interactibles[choice - 1].Invoke();
			Type type = @out?.GetType() ?? typeof(object);
			if (type == typeof(float))
			{
				float savedFloat = (float)@out!;
				if (!float.IsNaN(savedFloat))
				{
					Calculator.CalculatorInstance!.Memory.CheckUserForInterfaceInteraction(savedFloat);
				}
			}
			ReturnType @return = type == typeof(ReturnType) ? (ReturnType)@out! : ReturnType.Continue;

			if (ShowAsSubmenu)
			{
				UserInterface.Instance!.Conditions.Remove(hash);
			}

			return @return;
		}
		// for main loop specific interaction.
		object? IInteractible.Invoke()
		{
			bool loop = Repeat;
			bool exit = false;
			do
			{
				ReturnType type = Enter();
				if (type > 0)
					loop = false;
				if (type == ReturnType.Exit)
					exit = true;
			} while (loop);
			return exit ? ReturnType.Exit : null;
		}

		/// <summary>
		/// Converts <see cref="interactibles"/> as a string to print out into
		/// the console.
		/// </summary>
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

	/// <summary>
	/// Does nothing, exploits that <see cref="Submenu"/>s end after finishing 
	/// the operation. Used to go back from the submenu to a larger or main menu.
	/// </summary>
	public class BackInteract : IInteractible
	{
		public string Name => "Back";

		public object? Invoke() => ReturnType.Back;
	}

	/// <summary>
	/// A broad Iinteractible system. Very similar to a delegate in interface
	/// form.
	/// </summary>
	public interface IInteractible
	{
		private readonly record struct FlexibleInteractible : IInteractible
		{
			public FlexibleInteractible(IOperation operation)
			{
				Name = operation.Name;
				activateDel = () => operation.Execute(Calculator.CalculatorInstance!, Calculator.CalculatorInstance!.Memory.Value);
			}
			public FlexibleInteractible(string name, Func<object?> @delegate)
			{
				Name = name;
				activateDel = @delegate;
			}
			private readonly Func<object?> activateDel;
			public string Name { get; }
			public readonly object? Invoke() => activateDel.Invoke();
		}
		public static IInteractible ToIInteractible(IOperation operation)
			=> new FlexibleInteractible(operation);
		public static IInteractible ToIInteractible(string name, Func<object?> operation)
			=> new FlexibleInteractible(name, operation) { };
		object? Invoke();
		string Name { get; }
	}
	/// <summary>
	/// Return types, some things like operations get special treatment due to
	/// their output that can be saved with <see cref="Memory"/>
	/// </summary>
	public enum ReturnType : byte
	{
		Continue = 0,
		Back = 1,
		Exit = 2,
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

	/// <summary>
	/// Allows the user to do mathematical functions in an interactive way... mostly.
	/// </summary>
	public interface IOperation
	{
		/// <summary>
		/// Quickly gets the <see langword="float"/> from the user. Very similar
		/// to the normal <see cref="InputUtility.ReadUserInputLine(string, Func{string, bool}[])"/>
		/// and parsing it.
		/// </summary>
		public static float DefineValueByUser(string customName = "Value")
		{
			float output = 0f;
			InputUtility.ReadUserInputLine($"Get {customName}: ", value => float.TryParse(value, out output));
			return MathF.Round(output);
		}

		/// <summary>
		/// The name of the operation for displaying through <see cref="Submenu"/>s.
		/// </summary>
		string Name => this.GetType().Name;
		/// <summary>
		/// Executes the operation. Typically handles the <see cref="UserInterface.AsciiView"/>
		/// and <see cref="UserInterface.InputView"/>.
		/// </summary>
		/// <param name="parentCalculator">Deprecated, but still here to get the calculator</param>
		/// <param name="firstInput">Memory, <see langword="null"/> if none is stored.</param>
		/// <returns>The ruturn variable from the operation.</returns>
		float Execute(Calculator parentCalculator, float? firstInput);
	}

	#region Basic

	/// <summary>
	/// Adds 2 variables into one variable.
	/// </summary>
	public sealed class Add : IOperation
	{
		string IOperation.Name => "Addition";
		public static string MakeAsciiArt(float? Left, float? Right) =>
	$@"
	  {(Left is null ? "__" : Left)}
	+ {(Right is null ? "__" : Right)}
	─────
	{((Left is null || Right is null) ? "" : Left + Right)}
";
		public float Execute(Calculator parentCalculator, float? firstInput)
		{
			UserInterface.Instance!.AsciiView = MakeAsciiArt(null, null); 
			UserInterface.Instance.Reprint();
			float Left = firstInput ?? IOperation.DefineValueByUser("Top Value");
			UserInterface.Instance!.AsciiView = MakeAsciiArt(Left, null); 
			UserInterface.Instance.Reprint();
			float Right = IOperation.DefineValueByUser("Bottom Value");
			UserInterface.Instance!.AsciiView = MakeAsciiArt(Left, Right); 
			UserInterface.Instance.Reprint();
			return Left + Right;
		}
	}
	/// <summary>
	/// Removes a portion of a variable with another and displays the result.
	/// </summary>
	public sealed class Substract : IOperation
	{
		string IOperation.Name => "Subtraction";
		public static string MakeAsciiArt(float? Subtractive, float? Subtractor) =>
	$@"
	  {(Subtractive is null ? "__" : Subtractive)}
	- {(Subtractor is null ? "__" : Subtractor)}
	─────
	{((Subtractive is null || Subtractor is null) ? "" : Subtractive - Subtractor)}
";
		public float Execute(Calculator parentCalculator, float? firstInput)
		{
			UserInterface.Instance!.AsciiView = MakeAsciiArt(null, null); 
			UserInterface.Instance.Reprint();
			float Subtractive = firstInput ?? IOperation.DefineValueByUser("Value to Subtract");
			UserInterface.Instance!.AsciiView = MakeAsciiArt(Subtractive, null); 
			UserInterface.Instance.Reprint();
			float Subtractor = IOperation.DefineValueByUser("Value to Subtract With");
			UserInterface.Instance!.AsciiView = MakeAsciiArt(Subtractive, Subtractor); 
			UserInterface.Instance.Reprint();
			return Subtractive - Subtractor;
		}
	}
	/// <summary>
	/// Multiplies a variable with another, simple.
	/// </summary>
	public sealed class Multiply : IOperation
	{
		string IOperation.Name => "Multiplication";
		public static string MakeAsciiArt(float? MultLeft, float? MultRight) =>
	$@"
	  {(MultLeft is null ? "__" : MultLeft)}
	* {(MultRight is null ? "__" : MultRight)}
	─────
	{((MultLeft is null || MultRight is null) ? "" : MultLeft * MultRight)}
";
		public float Execute(Calculator parentCalculator, float? firstInput)
		{
			UserInterface.Instance!.AsciiView = MakeAsciiArt(null, null); 
			UserInterface.Instance.Reprint();
			float MultLeft = firstInput ?? IOperation.DefineValueByUser("Top Value");
			UserInterface.Instance!.AsciiView = MakeAsciiArt(MultLeft, null); 
			UserInterface.Instance.Reprint();
			float MultRight = IOperation.DefineValueByUser("Bottom Value");
			UserInterface.Instance!.AsciiView = MakeAsciiArt(MultLeft, MultRight);
			UserInterface.Instance.Reprint();
			return MultLeft * MultRight;
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
	/// Quits the program via <see cref="Environment.Exit(int)"/>. Throws a exception
	/// if it fails.
	/// </summary>
	public sealed class Quit : IInteractible
	{
		public string Name => "Quit";
		public object? Invoke() => ReturnType.Exit;
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
			float dividend = firstInput ?? IOperation.DefineValueByUser("Dividend");
			UserInterface.Instance!.AsciiView = MakeAsciiArt(dividend, null); 
			UserInterface.Instance.Reprint();
			float devisor = IOperation.DefineValueByUser("Devisor");
			if (devisor == 0f)
			{
				ThrowDivideByZeroError();
				return float.NaN;
			}
			UserInterface.Instance!.AsciiView = MakeAsciiArt(dividend, devisor); 
			UserInterface.Instance.Reprint();
			return dividend / devisor;
		}

		/// <summary>
		/// Implements custom graphics sort of like the screen glitching whenever
		/// the user tries to divide by 0 for any reason. 
		/// </summary>
		public static void ThrowDivideByZeroError()
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
			throw new DivideByZeroException();

			static string AddSpaces(string? text) => 
				$"{text ?? ""}{string.Join("", Enumerable.Repeat(' ', horizontal - (text?.Length ?? 0)))}";
		}
	}

	public class Factorial : IOperation
	{
		string IOperation.Name => "Factorial";
		/// <summary>
		/// Returns a <see langword="string"/> designed for being print out by 
		/// <see cref="UserInterface"/>. Visualizes as a long division problem.
		/// </summary>
		/// <param name="value">The Numerator</param>
		/// <param name="denominator">The Denominator</param>
		/// <returns></returns>
		public static string MakeAsciiArt(float? value, float? factorial, float? result)
		{
			return $"[{(value is null ? "__" : value)}]^[{(factorial is null ? "__" : factorial)}] {(result is null ? "" : $" = {result}")}";
		}
		float IOperation.Execute(Calculator parentCalculator, float? firstInput)
		{
			UserInterface.Instance!.AsciiView = MakeAsciiArt(null, null, null);
			UserInterface.Instance.Reprint();
			float value = firstInput ?? IOperation.DefineValueByUser("Base");
			UserInterface.Instance!.AsciiView = MakeAsciiArt(value, null, null);
			UserInterface.Instance.Reprint();
			float factorial = IOperation.DefineValueByUser("Power"),
				result = value;
			// TODO: Change this to a math.pow later!
			for (int i = 1; i < factorial; i++)
			{
				result *= value;
			}
			UserInterface.Instance!.AsciiView = MakeAsciiArt(value, factorial, result);
			UserInterface.Instance.Reprint();
			return result;
		}
	}
	#endregion
	#region Volume

	/// <summary>
	/// Grabs the area of a rectangle by 'A length * B length'.
	/// </summary>
	public sealed class AreaOfRectangle : IOperation
	{
		string IOperation.Name => "Area Of Rectangle";
		/// <summary>
		/// Prints a varying length rectangle based on a number of calculations.
		/// </summary>
		/// <param name="LongBoi"></param>
		/// <param name="WideBoi"></param>
		/// <returns>The resulting string to print.</returns>
		public static string MakeAsciiArt(float? LongBoi, float? WideBoi)
		{
			int X = FindVisualLength(LongBoi, 1.3f), Y = FindVisualLength(WideBoi, 2.0f);
			string answer = (LongBoi is null || WideBoi is null) ? "" : (LongBoi! * WideBoi!).ToString()!;
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
					line.Append($" {(WideBoi is null ? "__" : WideBoi)}");
				}
				sb.AppendLine(line.ToString());
			}
			sb.AppendLine(GetBorderLine());
			sb.AppendLine($"\t{string.Join("", Enumerable.Repeat(' ', X / 2))}{(LongBoi is null ? "__" : LongBoi)}");
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
			float wideBoi = firstInput ?? IOperation.DefineValueByUser("Width");
			UserInterface.Instance!.AsciiView = MakeAsciiArt(wideBoi, null); 
			UserInterface.Instance.Reprint();
			float longBoi = IOperation.DefineValueByUser("Length");
			UserInterface.Instance!.AsciiView = $"{wideBoi} * {longBoi} = {wideBoi * longBoi}\n" + MakeAsciiArt(wideBoi, longBoi); 
			UserInterface.Instance.Reprint();
			return wideBoi * longBoi;
		}
	}

	/// <summary>
	/// Prints the ascii art of the area of a circle, which is 'radius^2 * Pi'.
	/// </summary>
	public sealed class AreaOfCircle : IOperation
	{
		string IOperation.Name => "Area Of Circle";
		public static string MakeAsciiArt(float? radius) =>
	$@"
	radius: {(radius is null ? "__" : radius)}
	     ***
	  **     **
	 *         *
	*           *
	*     ●     * 
	*           *
	 *         *
	  **     **
	     ***
	{((radius is null) ? "__" : CalcAnswer(radius.Value))} x^2
";
		public float Execute(Calculator parentCalculator, float? firstInput)
		{
			UserInterface.Instance!.AsciiView = MakeAsciiArt(null); 
			UserInterface.Instance.Reprint();
			float radius = firstInput ?? IOperation.DefineValueByUser("Radius");
			UserInterface.Instance!.AsciiView = $"{radius}^2 * Pi = {CalcAnswer(radius)}\n" + MakeAsciiArt(radius); 
			UserInterface.Instance.Reprint();
			return CalcAnswer(radius);
		}

		public static float CalcAnswer(float radius) =>
			radius * radius * MathF.PI;
	}
	#endregion
	#region Trig


	public sealed class TriangleTheoremSet : Submenu
	{
		public TriangleTheoremSet() : base("Calculate Triangle Theorem")
		{
			interactibles = 
			[
				IInteractible.ToIInteractible("Calculate Hypotenuse", CalcHypo),
				IInteractible.ToIInteractible("Calculate Side A", CalcA),
				IInteractible.ToIInteractible("Calculate Side B", CalcB),
			];
			Repeat = false;
		}


		public static float CalculateHypotenuse(float A, float B)
		{
			return MathF.Sqrt(MathF.Pow(A, 2) + MathF.Pow(B, 2));
		}

		private static string GetString(string A = "__", string B = "__", string C = "__") => $@"
	{AddSpacing(C)}       /|
	{AddSpacing(C)}      / |
	{AddSpacing(C)}     /  |
	{C}    /   | A: {A}
	{AddSpacing(C)}   /    |
	{AddSpacing(C)}  /     |
	{AddSpacing(C)} /      |
	{AddSpacing(C)}/_______|
	  {AddSpacing(C)} B: {B}
	
";
		internal static string AddSpacing(string resultText) => string.Join("", Enumerable.Repeat(' ', resultText.Length));
		internal static float? GetFirstInput() => Calculator.CalculatorInstance?.Memory.Value;
		public static object? CalcA()
		{
			UserInterface.Instance!.AsciiView = GetString();
			UserInterface.Instance.Reprint();
			float C = GetFirstInput() ?? IOperation.DefineValueByUser("Hypotenuse");
			UserInterface.Instance!.AsciiView = GetString(C: C.ToString());
			UserInterface.Instance.Reprint();
			float B = IOperation.DefineValueByUser("Side B");
			float A = MathF.Sqrt((C * C) - (B * B));
			UserInterface.Instance!.AsciiView =
				$"√({C}^2 - {B}^2)=√({C * C} - {B * B})=√({(C * C) - (B * B)})={A}\n" +
				GetString(A.ToString(), B.ToString(), C.ToString());
			UserInterface.Instance.Reprint();
			return A;
		}

		public static object? CalcB()
		{
			UserInterface.Instance!.AsciiView = GetString();
			UserInterface.Instance.Reprint();
			float C = GetFirstInput() ?? IOperation.DefineValueByUser("Hypotenuse");
			UserInterface.Instance!.AsciiView = GetString(C: C.ToString());
			UserInterface.Instance.Reprint();
			float A = IOperation.DefineValueByUser("Side A");
			float B = MathF.Sqrt((C * C) - (A * A));
			UserInterface.Instance!.AsciiView =
				$"√({C}^2 - {A}^2)=√({C * C} - {A * A})=√({(C * C) - (A * A)})={B}\n" +
				GetString(A.ToString(), B.ToString(), C.ToString());
			UserInterface.Instance.Reprint();
			return B;
		}
		public static object? CalcHypo()
		{
			UserInterface.Instance!.AsciiView = GetString();
			UserInterface.Instance.Reprint();
			float A = GetFirstInput() ?? IOperation.DefineValueByUser("Side A");
			UserInterface.Instance!.AsciiView = GetString(A.ToString());
			UserInterface.Instance.Reprint();
			float B = IOperation.DefineValueByUser("Side B");
			float C = CalculateHypotenuse(A, B);
			UserInterface.Instance!.AsciiView =
				$"√({A}^2 + {B}^2)=√({A * A} + {B * B})=√({(A * A) + (B * B)})={C}\n" +
				GetString(A.ToString(), B.ToString(), C.ToString());
			UserInterface.Instance.Reprint();
			return C;
		}
	}
	#endregion

	
}

// Pulled out of this from an external library, used for the operators.
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