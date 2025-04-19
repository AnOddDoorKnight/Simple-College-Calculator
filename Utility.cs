namespace OVS.SimpleCollegeCalculator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class InputUtility
{
	public static string ReadUserInputLine(string initialResponse, params Func<string, bool>[] conditions)
	{
		string input;
		UserInterface.Instance!.InputView = initialResponse;
		UserInterface.Instance.Reprint();
		while (true)
		{
			input = Console.ReadLine() ?? string.Empty;
			if (conditions.All(con => con(input)))
			{
				break;
			}
			UserInterface.Instance.InputView = $"Invalid Response, try again.\n{initialResponse}";
			UserInterface.Instance.Reprint();
		}
		return input;
	}
	public static char ReadUserInputKey(string initialResponse, params Func<ConsoleKeyInfo, bool>[] conditions)
	{
		ConsoleKeyInfo info;
		UserInterface.Instance!.InputView = initialResponse;
		UserInterface.Instance.Reprint();
		while (true)
		{
			info = Console.ReadKey();
			if (conditions.All(con => con(info)))
			{
				break;
			}
			UserInterface.Instance.InputView = $"Invalid Response, try again.\n{initialResponse}";
			UserInterface.Instance.Reprint();
		}
		return info.KeyChar;
	}
}
