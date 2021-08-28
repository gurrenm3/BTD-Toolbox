using System;
using System.Collections.Generic;

namespace BTDToolbox.Extensions
{
	public static class ActionExtensions
	{
		public static void InvokeAll(this List<Action> actions)
		{
			actions.ForEach(action => action?.Invoke());
		}

		public static void InvokeAll<T>(this List<Action<T>> actions, T arg)
		{
			actions.ForEach(action => action?.Invoke(arg));
		}
	}
}