
namespace Qart.Testing
{
    public interface IStreamTransformerResolver
    {
        IStreamTransformer ResolveTransformer(string name);
    }
}
