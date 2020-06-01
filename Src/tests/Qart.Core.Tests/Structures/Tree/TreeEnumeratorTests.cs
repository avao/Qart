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
        public void MultiNodeIterationSucceeds()
        {
            var root = new Node("root", new List<Node> {
                           new Node("node1")
                        });
            AssertEnumeration(root, new[] { "root", "node1" });
        }

        [TestCase]
        public void MultiNodeIteration2Succeeds()
        {
            var root = new Node("root", new List<Node> {
                            new Node("node1"),
                            new Node("node2")
                        });
            AssertEnumeration(root, new[] { "root", "node1", "node2" });
        }

        [TestCase]
        public void MultiNodeIteration3Succeeds()
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


        [TestCase]
        public void MultiNodeIterationWIthPathSucceeds()
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

            AssertEnumerationWithPath(root, new[] {("root", ""),
                                                        ("node1", "root"),
                                                            ("node11", "node1->root"),
                                                            ("node12", "node1->root"),
                                                        ("node2", "root"),
                                                            ("node21", "node2->root"),
                                                                ("node211", "node21->node2->root")});
        }

        private static void AssertEnumeration(Node rootNode, IEnumerable<string> expectation)
        {
            Assert.That(rootNode.ToEnumerable().Select(node => node.GetData().Id), Is.EquivalentTo(expectation));
        }

        private static void AssertEnumerationWithPath(Node rootNode, IEnumerable<(string, string)> expectation)
        {
            Assert.That(rootNode.ToEnumerableWithPath().Select(item => (item.Node.GetData().Id, string.Join("->", item.Path.Select(pathItem => pathItem.GetData().Id)))), Is.EquivalentTo(expectation));
        }
    }
}
