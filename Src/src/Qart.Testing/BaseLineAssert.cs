using Newtonsoft.Json;
using Qart.Core.Io;
using System;
using System.IO;

namespace Qart.Testing
{
    public class BaseLineAssert
    {
        public static void AreEqual(string filePath, string actualContent)
        {
            AreEqual(filePath, actualContent, false);
        }

        public static void AreEqual(string filePath, string actualContent, bool rebase)
        {
            AreEqual(filePath, actualContent, rebase, (expected, actual) => throw new AssertException("Content in the expected file is different to the provided one"));
        }

        public static void AreEqual(string filePath, string actualContent, bool rebase, Action<string, string> failAction)
        {
            string expectedContent = null;
            if (File.Exists(filePath))
            {
                expectedContent = File.ReadAllText(filePath);
            }

            if (rebase)
            {
                FileUtils.WriteAllText(filePath, actualContent);
            }

            if (!string.Equals(expectedContent, actualContent))
            {
                failAction(expectedContent, actualContent);
            }
        }

        public static void AreEqual<T>(string filePath, T value, bool rebase)
        {
            string content = JsonConvert.SerializeObject(value);
            AreEqual(filePath, content, rebase);
        }
    }
}
