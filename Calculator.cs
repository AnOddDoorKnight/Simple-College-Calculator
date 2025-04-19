namespace OVS.SimpleCollegeCalculator;

using HDAdvance.Mathematics;
using System;
using System.Collections.Generic;

public class Calculator
{
	public static Calculator? CalculatorInstance { get; set; }

	public static void Main(string[] args)
	{
		Console.Title = "A Very Cool Calculator - By AnOddDoorKnight";
		CalculatorInstance = new Calculator();
		Exception? ex = CalculatorInstance.Run();
		if (ex != null)
		{
			Console.Clear();
			Console.WriteLine($"An Error has occured:\n{ex}\n...Retry?");
		}
		Console.ReadKey();
	}

	/* --------------------------------------------------------------- */

	public OperationList OperationList { get; set; }
	public UserInterface UserInterface { get; set; }
	public Memory Memory { get; set; }

	public Calculator()
	{
		OperationList = new();
		UserInterface = new(this);
		Memory = new(); 
	}

	public Exception? Run()
	{
		try
		{
		amongUs:
			UserInterface.Start();
			// Error handling
			int choice = int.Parse(InputUtility.ReadUserInputKey("Select a key: ",
				info => char.IsDigit(info.KeyChar)
				&& OperationList.GetRangeFromCollection().InRangeInclusive(int.Parse(info.KeyChar.ToString()) - 1)).ToString());
			float savedFloat = OperationList[choice - 1].Execute(this, Memory.Value);
			if (!float.IsNaN(savedFloat))
				Memory.CheckUserForInterfaceInteraction(savedFloat);
			goto amongUs;
		}
		catch (Exception ex) when (ex.Message == "Quit") { return null; }
		catch (Exception ex) { return ex; }
	}
}