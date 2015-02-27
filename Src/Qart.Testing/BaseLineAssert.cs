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
            AreEqual(fileName, content, false);
        }

        public static void AreEqual(string fileName, string content, bool rebase)
        {
            if(rebase)
            {
                File.WriteAllText(fileName, content);
            }
            else
            {
                var expected = File.ReadAllText(fileName);
                Assert.AreEqual(expected, content);
            }
        }
    }
}
