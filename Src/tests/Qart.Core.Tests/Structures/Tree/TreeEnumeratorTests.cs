using NUnit.Framework;
using Qart.Core.Structures.Tree;
using System.Collections.Generic;
using System.Linq;

namespace Qart.Core.Tests.Structures.Tree
{
    class TreeEnumeratorTests
    {
        private interface INodeData
        {
            string Id { get; }
        }

        private class Node : INode<INodeData>, INodeData
        {
            private readonly IReadOnlyCollection<Node> _children;
            public IEnumerable<INode<INodeData>> Children => _children;

            public string Id { get; set; }

            public Node(string id)
                : this(id, null)
            {
            }

            public Node(string id, IReadOnlyCollection<Node> children)
            {
                Id = id;
                _children = children;
            }

            public INodeData GetData()
            {
                return this;
            }
        }

        [TestCase]
        public void SingleNodeNullChildrenIterationSucceeds()
        {
            var root = new Node("root", null);
            AssertEnumeration(root, new[] { "root" });
        }

        [TestCase]
        public void SingleNodeEmptyChildrenIterationSucceeds()
        {
            var root = new Node("root", new List<Node> { });
            AssertEnumeration(root, new[] { "root" });
        }

        [TestCase]
        public void MultyNodeIterationSucceeds()
        {
            var root = new Node("root", new List<Node> {
                           new Node("node1")
                        });
            AssertEnumeration(root, new[] { "root", "node1" });
        }

        [TestCase]
        public void MultyNodeIteration2Succeeds()
        {
            var root = new Node("root", new List<Node> {
                            new Node("node1"),
                            new Node("node2")
                        });
            AssertEnumeration(root, new[] { "root", "node1", "node2" });
        }

        [TestCase]
        public void MultyNodeIteration3Succeeds()
        {
            var root = new Node("root", new List<Node> {
                           new Node("node1", new List<Node> {
                               new Node("node11"),
                               new Node("node12", new List<Node>{ }) }),
                           new Node("node2", new List<Node> {
                               new Node("node21", new List<Node> {
                                    new Node("node211") })
                           })
                        });

            AssertEnumeration(root, new[] { "root", "node1", "node11", "node12", "node2", "node21", "node211" });
        }

        private static void AssertEnumeration(Node rootNode, IEnumerable<string> expectation)
        {
            Assert.That(rootNode.ToEnumerable().Select(node => node.GetData().Id), Is.EquivalentTo(expectation));
        }
    }
}
