using NUnit.Framework;
using Qart.Core.Text;
using System;
using System.Collections.Generic;

namespace Qart.Core.Tests.Text
{
    class VariableResolverTests
    {
        private readonly static IDictionary<string, string> _variables = new Dictionary<string, string> { { "var", "value" } };

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
