namespace OVS.SimpleCollegeCalculator;
using System;
using System.Collections.Generic;
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

public sealed class OperationList : List<IOperation>
{
	public static OperationList Instance { get; } = new();

	public OperationList()
	{
		// CurrentList
		this.AddRange(
		[
			new Add(),
			new Substract(),
			new Multiply(),
			new Divide(),
			new AreaOfRectangle(),
			new AreaOfCircle(),
			new TriangleTheroem(),
		]);
		// Select anything thats missing
		IEnumerable<Type> leftOverTypes = AppDomain.CurrentDomain.GetAssemblies()
		.SelectMany(assembly => assembly.GetTypes()
			.Where(obj => typeof(IOperation).IsAssignableFrom(obj) && obj != typeof(IOperation)));
		Type[] enumerator = leftOverTypes.ToArray(); // would be ienumerator, but it didnt reset to 0.
		for (int i = 0; i < enumerator.Length; i++)
		{
			if (this.Any(op => op.GetType() == enumerator[i]))
			{
				continue;
			}
			Add((IOperation)Activator.CreateInstance(enumerator[i])!);
		}
	}

	public override string ToString()
	{
		return string.Join("\n", this.Select((e, i) => $"{i + 1}) {e.Name}"));
	}
}