using NUnit.Framework;
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
            AreEqual(fileName, content, false, (expected, actual) => Assert.Fail("Content in the expeced file is different to the provided one",fileName, content));
        }

        public static void AreEqual(string fileName, string content, bool rebase, Action<string, string> failAction)
        {
            if(rebase)
            {
                File.WriteAllText(fileName, content);
            }
            else
            {
                var expected = File.ReadAllText(fileName);
                if(!string.Equals(expected, content))
                {
                    failAction(expected, content);
                }
            }
        }
    }
}
