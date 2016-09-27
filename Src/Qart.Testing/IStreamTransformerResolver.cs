
namespace Qart.Testing
{
    public interface IStreamTransformerResolver
    {
        IStreamTransformer Create(string name);

        void Release(IStreamTransformer streamTransformer);
    }
}
