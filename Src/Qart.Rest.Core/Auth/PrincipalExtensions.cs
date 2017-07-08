using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace Qart.Rest.Core.Auth
{
    public static class PrincipalExtensions
    {
        public static string GetName(this IPrincipal principal)
        {
            var identity = (ClaimsIdentity)principal.Identity;
            if (identity.IsAuthenticated)
                return identity.Claims.Single(_ => _.Type == "sub").Value;
            return null;
        }
    }
}
