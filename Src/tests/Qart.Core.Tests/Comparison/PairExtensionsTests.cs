using NUnit.Framework;
using Qart.Core.Comparison;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Qart.Core.Tests.Comparison
{
    [TestFixture]
    public class PairExtensionsTests
    {
        public class Record
        {
            public string StringProp { get; set; }
            public DateTime DateTimeProp { get; set; }
        }


        private static IEnumerable<TestCaseData> EqualityAssessmentTestData
        {
            get
            {
                var dateTimeValue = new DateTime(2021, 09, 14);
                yield return new TestCaseData(new Record { }, new Record { StringProp = "Hey" }, new[] { nameof(Record.StringProp) });
                yield return new TestCaseData(new Record { StringProp = "a" }, new Record { StringProp = "Hey" }, new[] { nameof(Record.StringProp) });
                yield return new TestCaseData(new Record { StringProp = "a", DateTimeProp = dateTimeValue }, new Record { StringProp = "Hey" }, new[] { nameof(Record.StringProp), nameof(Record.DateTimeProp) });
                yield return new TestCaseData(new Record { StringProp = "a", DateTimeProp = dateTimeValue }, new Record { StringProp = "a" }, new[] { nameof(Record.DateTimeProp) });
                yield return new TestCaseData(new Record { DateTimeProp = dateTimeValue }, new Record { }, new[] { nameof(Record.DateTimeProp) });
                yield return new TestCaseData(new Record { DateTimeProp = dateTimeValue }, new Record { DateTimeProp = dateTimeValue }, new string[] { });
            }
        }

        [Test, TestCaseSource(nameof(EqualityAssessmentTestData))]
        public void AssessEquality(Record lhs, Record rhs, IEnumerable<string> expectedDiff)
        {
            var diffs = new List<string>();

            (lhs, rhs).AssessEquality(r => r.StringProp, OnDiff);
            (lhs, rhs).AssessEquality(r => r.DateTimeProp, OnDiff);

            Assert.That(diffs, Is.EquivalentTo(expectedDiff));

            void OnDiff<TM>(Record l, Record r, TM v1, TM v2, MemberInfo memberInfo)
            {
                diffs.Add(memberInfo.Name);
            }
        }
    }
}
