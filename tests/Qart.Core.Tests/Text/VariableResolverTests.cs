using NUnit.Framework;
using Qart.Core.Text;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Qart.Core.Tests.Text
{
    class VariableResolverTests
    {
        private readonly static IDictionary<string, string> _variables = new Dictionary<string, string> { { "var", "value" } };
        private readonly static IDictionary<string, object> _variables2 = new Dictionary<string, object> { { "var", -27 }, { "var2", new DateTime(2022, 2, 19, 13, 50, 34) } };

        [TestCase("abc", ExpectedResult = "abc")]
        [TestCase("abc$", ExpectedResult = "abc$")]
        [TestCase("abc_${var}", ExpectedResult = "abc_value")]
        [TestCase("abc_${var}_", ExpectedResult = "abc_value_")]
        [TestCase("${var}", ExpectedResult = "value")]
        [TestCase("${var}_", ExpectedResult = "value_")]
        [TestCase("${${var}}_", ExpectedResult = "${var}_")]
        public string ResolveSucceeds(string value)
        {
            return VariableResolver.Resolve(value, key => _variables[key]);
        }

        [TestCase("_${var:X}_", ExpectedResult = "_FFFFFFE5_")]
        [TestCase("_${var,15}_", ExpectedResult = "_            -27_")]
        [TestCase("_${var,15:X}_", ExpectedResult = "_       FFFFFFE5_")]
        [TestCase("${var2:yyyyMMMdd HH:mm:ss}_", ExpectedResult = "2022Feb19 13:50:34_")]
        public string ResolveFormatSucceeds(string value)
        {
            return VariableResolver.Resolve(value, key => _variables2[key]);
        }

        [TestCase("abc${")]
        [TestCase("abc${var} ${...")]
        public void ResolveThrows(string value)
        {
            Assert.That(() => VariableResolver.Resolve(value, (key) => _variables[key]), Throws.InstanceOf<ArgumentException>());
        }

        [TestCase("abc${var21}")]
        public void ResolveFuncThrows(string value)
        {
            Assert.That(() => VariableResolver.Resolve(value, (key) => throw new KeyNotFoundException()), Throws.InstanceOf<KeyNotFoundException>());
        }
    }
}
