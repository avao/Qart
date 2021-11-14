using Qart.Core.Validation;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Qart.Core.Structures.Tree
{
    public class TreeEnumerator<T> : IEnumerator<INode<T>>
    {
        private readonly INode<T> _rootNode;

        private readonly Stack<(INode<T> Node, IEnumerator<INode<T>> Enumerator)> _stack;

        public INode<T> Current => _stack.Peek().Node;
        object IEnumerator.Current => Current;

        public IEnumerable<INode<T>> Path => _stack.Select(item => item.Node).Skip(1);

        public TreeEnumerator(INode<T> rootNode)
        {
            Require.NotNull(rootNode, "rootNode cannot be null.");

            _rootNode = rootNode;
            _stack = new Stack<(INode<T> Node, IEnumerator<INode<T>> Enumerator)>();

            Reset();
        }

        public bool MoveNext()
        {
            while (_stack.Count > 0)
            {
                var current = _stack.Pop();
                if (current.Enumerator == null)
                {
                    var enumerator = current.Node.Children == null
                        ? Enumerable.Empty<INode<T>>().GetEnumerator()
                        : current.Node.Children.GetEnumerator();

                    _stack.Push((current.Node, enumerator));

                    return true;
                }
                else if (current.Enumerator.MoveNext())
                {
                    _stack.Push(current);
                    _stack.Push((current.Enumerator.Current, null));
                }
                continue;
            }
            return false;
        }

        public void Reset()
        {
            _stack.Clear();
            _stack.Push((_rootNode, null));
        }

        public void Dispose()
        {
            _stack.Clear();
        }
    }
}
