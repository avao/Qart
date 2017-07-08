namespace Qart.Rest.Core.Auth
{
    public interface IAuthorisationManager
    {
        bool Authorise(string user, string password);
    }
}
