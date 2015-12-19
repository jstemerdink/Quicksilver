using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Specialized;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

using EPiServer.Reference.Commerce.Domain.Contracts.Services;

namespace EPiServer.Reference.Commerce.Shared.Services
{
    [ServiceConfiguration(typeof(IMailService), Lifecycle = ServiceInstanceScope.PerRequest)]
    public class MailService : Domain.Services.MailService
    {
        public MailService(HttpContextBase httpContextBase, UrlResolver urlResolver, IContentLoader contentLoader, IHtmlDownloader htmlDownloader)
            : base(httpContextBase, urlResolver, contentLoader, htmlDownloader)
        {
        }
    }
}