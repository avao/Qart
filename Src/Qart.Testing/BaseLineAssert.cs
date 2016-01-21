using NUnit.Framework;
using Qart.Core.Io;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Testing
{
    public class BaseLineAssert
    {
        public static void AreEqual(string fileName, string content)
        {
            AreEqual(fileName, content, false, (expected, actual) => Assert.Fail("Content in the expeced file is different to the provided one", fileName, content));
        }

        public static void AreEqual(string fileName, string content, bool rebase, Action<string, string> failAction)
        {
            string expectedContent = null;
            if (File.Exists(fileName))
            {
                expectedContent = File.ReadAllText(fileName);
            }

            if (rebase)
            {
                FileUtils.WriteAllText(fileName, content);
            }

            if (!string.Equals(expectedContent, content))
            {
                failAction(expectedContent, content);
            }
        }
    }
}
