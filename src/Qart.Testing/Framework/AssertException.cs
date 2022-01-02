using System;
using System.Collections.Generic;

namespace Qart.Testing.Framework
{
    [Serializable]
    public class AssertException : Exception
    {
        public static string CategoriesKey = "Categories";

        public AssertException(string message)
            : this(message, Array.Empty<string>())
        { }

        public AssertException(string message, IReadOnlyCollection<string> categories)
            : base(message)
        {
            if (categories.Count > 0)
            {
                Data.Add(CategoriesKey, categories);
            }
        }

        public AssertException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public bool TryGetCategories(out IReadOnlyCollection<string> categories)
        {
            if (Data.Contains(CategoriesKey))
            {
                categories = (IReadOnlyCollection<string>)Data[CategoriesKey];
                return true;
            }
            categories = default;
            return false;
        }
    }
}
