using System;
using Common.Logging;

namespace Qart.Testing.Framework
{
	public class TenorTagExpander : ITagExpander
	{
		private static DateTime _referenceTimePoint = DateTime.Now;
		private readonly ILog _logger;
		private const int ShiftAmountField = 0;
		private const int KeyField = 1;
		private const int FormatField = 2;

		public TenorTagExpander(ILog logger)
		{
			_logger = logger;
			_logger.TraceFormat("Using reference time point {0}", _referenceTimePoint);
		}

		DateTime RollReferenceTimePoint(int shiftAmount, string key)
		{
			switch(key)
			{
				case "d": return _referenceTimePoint.AddDays(shiftAmount);
				case "W": return _referenceTimePoint.AddDays(shiftAmount + 7);
				case "M": return _referenceTimePoint.AddMonths(shiftAmount);
				case "Y": return _referenceTimePoint.AddYears(shiftAmount);

				case "h": return _referenceTimePoint.AddHours(shiftAmount);
				case "m": return _referenceTimePoint.AddMinutes(shiftAmount);
				case "s": return _referenceTimePoint.AddSeconds(shiftAmount);
			}
			_logger.ErrorFormat("Key not supported by TenorTagExpander: '{0}'", key);
			throw new NotSupportedException("Key not supported by TenorTagExpander: " + key);
		}

		string[] GetTokens(string value)
		{
			var tokens = value.Split(new[] { '|' });
			if (tokens.Length != 3) 
			{
				_logger.ErrorFormat("Incorrect number of tokens supplied to TenorTagExpander: '{0}'", tokens.Length);
				throw new ArgumentException("Incorrect number of tokens supplied to TenorTagExpander: " + tokens.Length);
			}
			return tokens;
		}

		public string Expand(string value)
		{
			var tokens = GetTokens(value);
			var shiftAmount = Convert.ToInt32(tokens[ShiftAmountField]);
			var key = tokens[KeyField];
			var format = tokens[FormatField];

			var rolled = RollReferenceTimePoint (shiftAmount, key);
			_logger.TraceFormat("Rolled date: {0}", rolled);

			var formatted = rolled.ToString(format);
			_logger.TraceFormat("Formatted date: {0}", formatted);
			return formatted;
		}
	}
}