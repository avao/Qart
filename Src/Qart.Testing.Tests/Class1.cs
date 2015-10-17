﻿using NUnit.Framework;
using Qart.Core.Io;
using Qart.Testing.FileBased;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Testing.Tests
{
    public class SomeTests
    {
        TestSystem TestSystem = new TestSystem(new DataStore(PathUtils.ResolveRelative(@"TestData\TestCases")));

        [Test]
        public void Ref()
        {
            var testCase = TestSystem.GetTestCase("Ref");
            Assert.AreEqual("<root/>", testCase.GetContent("artifact.xml"));
        }

        [Test]
        public void MisssingArtefact()
        {
            var testCase = TestSystem.GetTestCase("Ref");
            Assert.IsNull(testCase.GetContent("missing"));
        }
    }
}
