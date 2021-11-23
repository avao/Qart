using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Qart.Core.Activation;
using System;
using System.Collections.Generic;

namespace Qart.Core.Tests.Activation
{
    public class ServiceProviderExtensionsTests
    {
        private class A { }

        private class B
        {
            public IDictionary<string, object> Parameters { get; }
            public B(int p1, string p2, object p3 = null, int p4 = 1)
            {
                Parameters = new Dictionary<string, object> {
                    { nameof(p1), p1 },
                    { nameof(p2), p2 },
                    { nameof(p3), p3 },
                    { nameof(p4), p4 },
                };
            }

            public B(IDependency p1, int p2)
            {
                Parameters = new Dictionary<string, object> {
                    { nameof(p1), p1 },
                    { nameof(p2), p2 }
                };
            }
        }

        private interface IDependency { }
        private class Dependency : IDependency { public static Dependency Instance = new Dependency(); }

        private class C<T>
        {
            public T Value { get; }
            public C(T value)
            {
                Value = value;
            }
        }

        [TestCase]
        public void DefaultConstructorWithNoParametersSucceeds()
        {
            var instance = GetServiceProvider().CreateInstance<A>(new Dictionary<string, object> { });
            Assert.That(instance, Is.Not.Null);
        }

        [TestCase]
        public void DefaultConstructorWithNullParametersSucceeds()
        {
            var instance = GetServiceProvider().CreateInstance<A>((IDictionary<string, object>)null);
            Assert.That(instance, Is.Not.Null);
        }

        [TestCase]
        public void ExtraParameterValuesThrows()
        {
            var exception = Assert.Throws<NotSupportedException>(() => GetServiceProvider().CreateInstance<A>(new Dictionary<string, object> { { "p1", 3 } }));
            Assert.That(exception.Message, Is.EqualTo("Could not create Qart.Core.Tests.Activation.ServiceProviderExtensionsTests+A with parameter overrides p1"));
        }

        [TestCase]
        public void NotEnoughParameterValuesThrows()
        {
            var serviceProvider = GetServiceProvider();

            var exception = Assert.Throws<NotSupportedException>(() => serviceProvider.CreateInstance<B>(new Dictionary<string, object> { { "p1", 3 } }));
            Assert.That(exception.Message, Is.EqualTo("Could not create Qart.Core.Tests.Activation.ServiceProviderExtensionsTests+B with parameter overrides p1"));
        }


        private static IEnumerable<TestCaseData> NonDefaultConstructorSucceedsData
        {
            get
            {
                yield return new TestCaseData(new Dictionary<string, object> { { "p1", 3 }, { "P2", "abc" }, { "p3", "def" } },
                                              new Dictionary<string, object> { { "p1", 3 }, { "p2", "abc" }, { "p3", "def" }, { "p4", 1 } });

                yield return new TestCaseData(new Dictionary<string, object> { { "P1", 3 }, { "p2", "abc" } },
                                              new Dictionary<string, object> { { "p1", 3 }, { "p2", "abc" }, { "p3", null }, { "p4", 1 } });

                yield return new TestCaseData(new Dictionary<string, object> { { "p2", 3 } },
                                              new Dictionary<string, object> { { "p1", Dependency.Instance }, { "p2", 3 } });

                yield return new TestCaseData(new Dictionary<string, object> { { "p1", null }, { "p2", 3 } },
                                              new Dictionary<string, object> { { "p1", null }, { "p2", 3 } });

                yield return new TestCaseData(new Dictionary<string, object> { { "p1", null }, { "p2", "3" } },
                                              new Dictionary<string, object> { { "p1", null }, { "p2", 3 } });
            }
        }

        [Test, TestCaseSource(nameof(NonDefaultConstructorSucceedsData))]
        public void NonDefaultConstructorSucceeds(IDictionary<string, object> input, IDictionary<string, object> expected)
        {
            var instance = GetServiceProvider().CreateInstance<B>(input);
            Assert.That(instance.Parameters, Is.EquivalentTo(expected));
        }

        [Test]
        public void TypeConversionSucceeds()
        {
            ExecuteValueTypeConversion("true", true);
            ExecuteValueTypeConversion("false", false);
            ExecuteValueTypeConversion("3", 3);
            ExecuteValueTypeConversion("-645", -645);
            ExecuteValueTypeConversion("3.2", 3.2);
            ExecuteValueTypeConversion("3.2", 3.2M);
            ExecuteValueTypeConversion("2021-09-05T10:59:55", new DateTime(2021, 09, 05, 10, 59, 55));
        }

        private static void ExecuteValueTypeConversion<T>(object input, T expected)
             where T : struct
        {
            ExecuteTypeConversion<T>(input, expected);
            ExecuteTypeConversion<T?>(input, expected);
            ExecuteTypeConversion<T?>(null, null);
        }

        private static void ExecuteTypeConversion<T>(object input, T expected)
        {
            var instance = GetServiceProvider().CreateInstance<C<T>>(new Dictionary<string, object> { { "value", input } });
            Assert.That(instance.Value, Is.EqualTo(expected));
        }

        private static IServiceProvider GetServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IDependency>(Dependency.Instance);
            return services.BuildServiceProvider();
        }
    }
}
