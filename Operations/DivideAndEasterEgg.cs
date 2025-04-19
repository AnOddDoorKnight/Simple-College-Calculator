namespace OVS.SimpleCollegeCalculator;

using HDAdvance.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public sealed class Divide : IOperation
{
	public string MakeAsciiArt(float? A, float? B) =>
$@"
	  {(A is null ? "__" : A)}
	/ {(B is null ? "__" : B)}
	─────
	{((A is null || B is null) ? "" : A / B)}
";

	public float Execute(Calculator parentCalculator, float? firstInput)
	{
		UserInterface.Instance!.AsciiView = MakeAsciiArt(null, null); UserInterface.Instance.Reprint();
		float A = firstInput ?? IOperation.DefineValueByUser();
		UserInterface.Instance!.AsciiView = MakeAsciiArt(A, null); UserInterface.Instance.Reprint();
		float B = IOperation.DefineValueByUser();
		if (B == 0f)
		{
			ThrowDivideByZeroError();
			Environment.Exit(0); // Just in case, not an official way to return
			return float.NaN;
		}
		UserInterface.Instance!.AsciiView = MakeAsciiArt(A, B); UserInterface.Instance.Reprint();
		return A / B;
	}

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
		Stopwatch stopwatch = Stopwatch.StartNew(); long stopAt = (long)TimeSpan.FromSeconds(5).TotalMilliseconds;
		Range<double> range = new(0f, 5f);
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
			double waitDelay = 120d - Math.Pow(1d, 1d + (range.ToNormalize(stopwatch.Elapsed.TotalSeconds) * 1000d));
			Task.Delay((int)waitDelay).Wait();
		}
		Environment.Exit(0);

		static string AddSpaces(string? text) => $"{text ?? ""}{string.Join("", Enumerable.Repeat(' ', horizontal - (text?.Length ?? 0)))}";
	}
}