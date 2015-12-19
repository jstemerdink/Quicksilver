using System.Web.Security;

using EPiServer.Security;

namespace EPiServer.Reference.Commerce.Domain.Facades
{
    public class UrlAuthorizationFacade
    {
        public virtual bool CheckUrlAccessForPrincipal(string path)
        {
            return UrlAuthorizationModule.CheckUrlAccessForPrincipal(path, PrincipalInfo.CurrentPrincipal, "GET");
        }
    }
}