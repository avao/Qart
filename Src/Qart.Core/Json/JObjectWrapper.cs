using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Qart.Core.Json
{
	public abstract class JObjectWrapper
	{
		public string OriginalContent { get; private set; }
		public JObject JObject { get; private set; }

		protected JObjectWrapper(string content)
		{
			OriginalContent = content;
			JObject = JObject.Parse(content);
		}

		protected JObjectWrapper(string content, IEnumerable<string> volatileProperties)
			: this(content)
		{
			foreach (var prop in volatileProperties)
			{
				JObject.Property(prop).Remove();
			}
		}

		public override string ToString()
		{
			return JObject.ToString();
		}
	}
}