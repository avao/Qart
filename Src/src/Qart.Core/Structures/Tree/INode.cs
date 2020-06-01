using System.Collections.Generic;

namespace Qart.Core.Structures.Tree
{
    public interface INode<T>
    {
        T GetData();
        IEnumerable<INode<T>> Children { get; }
    }

    public static class NodeExtensions
    {
        public static IEnumerable<INode<T>> ToEnumerable<T>(this INode<T> root)
        {
            using (var enumerator = new TreeEnumerator<T>(root))
            {
                while (enumerator.MoveNext())
                {
                    yield return enumerator.Current;
                }
            }
        }
    }
}
