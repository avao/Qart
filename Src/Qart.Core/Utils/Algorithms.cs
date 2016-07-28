using System;
using System.Threading;

namespace Qart.Core.Utils
{
	public static class Algorithms
	{
		public static void RepeatUntil(Func<bool> predicateFunc, TimeSpan waitTimeSpan, 
			int maxRetries, string exceptionMessage)
		{
			RepeatUntil(() => true, _ => predicateFunc (), waitTimeSpan, maxRetries, exceptionMessage);
		}

		public static T RepeatUntil<T>(Func<T> action, Func<T, bool> predicateFunc, TimeSpan waitTimeSpan, 
			int maxRetries, string exceptionMessage)
		{
			var result = action();
			while (!predicateFunc (result))
			{
				if (--maxRetries <= 0)
					throw new TimeoutException("Timeout while polling. " + exceptionMessage);
				Thread.Sleep(waitTimeSpan);
				result = action();
			}
			return result;
		}
	}
}

