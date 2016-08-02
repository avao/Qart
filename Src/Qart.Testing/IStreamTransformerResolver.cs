
namespace Qart.Testing
{
    public interface IStreamTransformerResolver
    {
        IStreamTransformer GetTransformer(string name);
    }
}
