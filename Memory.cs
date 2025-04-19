namespace OVS.SimpleCollegeCalculator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
