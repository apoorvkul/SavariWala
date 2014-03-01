using System;
using System.Collections.Generic;

namespace SavariWala
{
	// Holder for extentsion methods improving programming fluency
	static public class FluencyHelper
	{
		// Adding functional foreach to IEnumerable
		static void ForEach<Type>(this IEnumerable<Type> enumerable, Action<Type> action)
		{
			foreach (var item in enumerable)
				action (item);
		}
	}
}