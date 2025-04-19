namespace OVS.SimpleCollegeCalculator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



public sealed class AreaOfRectangle : IOperation
{
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
		UserInterface.Instance!.AsciiView = MakeAsciiArt(null, null); UserInterface.Instance.Reprint();
		float A = firstInput ?? IOperation.DefineValueByUser();
		UserInterface.Instance!.AsciiView = MakeAsciiArt(A, null); UserInterface.Instance.Reprint();
		float B = IOperation.DefineValueByUser();
		UserInterface.Instance!.AsciiView = MakeAsciiArt(A, B); UserInterface.Instance.Reprint();
		return A * B;
	}
}

public sealed class AreaOfCircle : IOperation
{
	public string MakeAsciiArt(float? A, float? B) =>
$@"
	  {(A is null ? "__" : A)}
	* {(B is null ? "__" : B)}
	─────
	{((A is null || B is null) ? "" : A * B)}
";
	public float Execute(Calculator parentCalculator, float? firstInput)
	{
		UserInterface.Instance!.AsciiView = MakeAsciiArt(null, null); UserInterface.Instance.Reprint();
		float A = firstInput ?? IOperation.DefineValueByUser();
		UserInterface.Instance!.AsciiView = MakeAsciiArt(A, null); UserInterface.Instance.Reprint();
		float B = IOperation.DefineValueByUser();
		UserInterface.Instance!.AsciiView = MakeAsciiArt(A, B); UserInterface.Instance.Reprint();
		return A * B;
	}
}

public sealed class TriangleTheroem : IOperation
{
	public string MakeAsciiArt(float? A, float? B) =>
$@"
	  {(A is null ? "__" : A)}
	* {(B is null ? "__" : B)}
	─────
	{((A is null || B is null) ? "" : A * B)}
";
	public float Execute(Calculator parentCalculator, float? firstInput)
	{
		UserInterface.Instance!.AsciiView = MakeAsciiArt(null, null); UserInterface.Instance.Reprint();
		float A = firstInput ?? IOperation.DefineValueByUser();
		UserInterface.Instance!.AsciiView = MakeAsciiArt(A, null); UserInterface.Instance.Reprint();
		float B = IOperation.DefineValueByUser();
		UserInterface.Instance!.AsciiView = MakeAsciiArt(A, B); UserInterface.Instance.Reprint();
		return A * B;
	}
}