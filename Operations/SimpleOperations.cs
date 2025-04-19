namespace OVS.SimpleCollegeCalculator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

public sealed class Add : IOperation
{
	public string MakeAsciiArt(float? A, float? B) =>
$@"
	  {(A is null ? "__" : A)}
	+ {(B is null ? "__" : B)}
	─────
	{((A is null || B is null) ? "" : A+B)}
";
	public float Execute(Calculator parentCalculator, float? firstInput)
	{
		UserInterface.Instance!.AsciiView = MakeAsciiArt(null, null); UserInterface.Instance.Reprint();
		float A = firstInput ?? IOperation.DefineValueByUser();
		UserInterface.Instance!.AsciiView = MakeAsciiArt(A, null); UserInterface.Instance.Reprint();
		float B = IOperation.DefineValueByUser();
		UserInterface.Instance!.AsciiView = MakeAsciiArt(A, B); UserInterface.Instance.Reprint();
		return A + B;
	}
}
public sealed class Substract : IOperation
{
	public string MakeAsciiArt(float? A, float? B) =>
$@"
	  {(A is null ? "__" : A)}
	- {(B is null ? "__" : B)}
	─────
	{((A is null || B is null) ? "" : A - B)}
";
	public float Execute(Calculator parentCalculator, float? firstInput)
	{
		UserInterface.Instance!.AsciiView = MakeAsciiArt(null, null); UserInterface.Instance.Reprint();
		float A = firstInput ?? IOperation.DefineValueByUser();
		UserInterface.Instance!.AsciiView = MakeAsciiArt(A, null); UserInterface.Instance.Reprint();
		float B = IOperation.DefineValueByUser();
		UserInterface.Instance!.AsciiView = MakeAsciiArt(A, B); UserInterface.Instance.Reprint();
		return A - B;
	}
}
public sealed class Multiply : IOperation
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
public sealed class Quit : IOperation
{
	public float Execute(Calculator parentCalculator, float? firstInput)
	{
		Environment.Exit(0);
		throw new Exception();
	}
}