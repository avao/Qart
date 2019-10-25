namespace Qart.Testing
{
    public interface IItemProvider
    {
        bool TryGetItem<T>(string itemKey, out T item) where T : class;
    }
}
