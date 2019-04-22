using Microsoft.Owin.Security.OAuth;

namespace Qart.Rest.Core.Auth
{
    public interface IAuthorisationManager
    {
        bool Authorise(OAuthGrantResourceOwnerCredentialsContext context);
    }
}
