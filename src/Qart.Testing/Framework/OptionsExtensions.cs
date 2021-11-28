using Qart.Core.Collections;
using System;
using System.Collections.Generic;

namespace Qart.Testing.Framework
{
    public static class OptionsExtensions
    {
        public static bool IsRebaseline(this IDictionary<string, string> options)
        {
            return options.GetOptionalValue("ct.rebase", false, bool.Parse);
        }

        public static string GetDirectory(this IDictionary<string, string> options)
        {
            return options.GetOptionalValue("ct.dir", null);
        }

        public static bool GetDeferExceptions(this IDictionary<string, string> options)
        {
            return options.GetOptionalValue("ct.deferExceptions", false, bool.Parse);
        }

        public static IReadOnlyCollection<string> GetIncludeTags(this IDictionary<string, string> options)
        {
            return GetCsv(options, "includeTags");
        }

        public static IReadOnlyCollection<string> GetExcludeTags(this IDictionary<string, string> options)
        {
            return GetCsv(options, "excludeTags");
        }

        public static int GetWorkersCount(this IDictionary<string, string> options)
        {
            return options.GetOptionalValue("ct.workersCount", 1, int.Parse);
        }

        private static IReadOnlyCollection<string> GetCsv(IDictionary<string, string> options, string tagName)
        {
            return options.GetOptionalValue(tagName, Array.Empty<string>(), t => t.ToLower().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
