namespace Qart.Testing.Context
{
    public interface IItemProvider
    {
        bool TryGetItem<T>(string itemKey, out T item);
    }
}
