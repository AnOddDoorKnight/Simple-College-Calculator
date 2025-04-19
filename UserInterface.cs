namespace OVS.SimpleCollegeCalculator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

	public void Start()
	{
		AsciiView = calculator.OperationList.ToString();
		Reprint();
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
