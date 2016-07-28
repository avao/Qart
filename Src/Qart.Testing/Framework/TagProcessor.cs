using System.Collections.Generic;
using Common.Logging;

namespace Qart.Testing.Framework
{
	public class TagProcessor
	{
		private readonly Dictionary<string, ITagExpander> _tagExpanders = new Dictionary<string, ITagExpander>();
		private readonly ILog _logger;
		private const int TagExpanderNameField = 0;
		private const int TagExpanderArgumentsField = 1;
		private static readonly string StartMarker = "{{";
		private static readonly string EndMarker = "}}";

		public TagProcessor(ILog logger)
		{
			_tagExpanders.Add("tenor", new TenorTagExpander(logger));
			_logger = logger;
		}

		private static bool HasTagSyntax(string value)
		{
			return value.StartsWith(StartMarker, System.StringComparison.Ordinal) && 
				value.EndsWith(EndMarker, System.StringComparison.Ordinal);
		}

		private static string RemoveMarkers(string value)
		{
			return value.Replace(StartMarker, string.Empty).Replace(EndMarker, string.Empty);
		}

		public string Process(string original)
		{
			if (string.IsNullOrWhiteSpace(original) || !HasTagSyntax(original))
				return original;

			_logger.TraceFormat("Found a tag '{0}'", original);
			var withoutMarkers = RemoveMarkers(original);

			// split by only the first token, leting Tag Expanders handle the rest
			var tokens = withoutMarkers.Split(new[] { '|' }, 2);
			if (tokens.Length == 0)
			{
				_logger.WarnFormat("No tokens found. Original value '{0}'", original);
				return original;
			}

			var tagExpanderName = tokens[TagExpanderNameField];
			ITagExpander tagExpander;
			_tagExpanders.TryGetValue(tagExpanderName, out tagExpander);
			if (tagExpander == null)
			{
				_logger.WarnFormat("No tag expander found for tag: '{0}'", tagExpanderName);
				return original;
			}

			string expanded = tagExpander.Expand(tokens[TagExpanderArgumentsField]);
			_logger.TraceFormat ("Expanded value from '{0}' to '{1}' using expander '{2}'", 
				original, expanded, tagExpanderName);

			return expanded;
		}
	}
}