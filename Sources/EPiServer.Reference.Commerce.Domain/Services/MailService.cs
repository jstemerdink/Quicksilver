using System;
using System.Collections.Specialized;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

using EPiServer.Core;
using EPiServer.Reference.Commerce.Domain.Contracts.Models;
using EPiServer.Reference.Commerce.Domain.Contracts.Services;
using EPiServer.Web.Routing;

using Microsoft.AspNet.Identity;

namespace EPiServer.Reference.Commerce.Domain.Services
{
    public abstract class MailService : IMailService
    {
        protected readonly HttpContextBase _httpContextBase;
        protected readonly UrlResolver _urlResolver;
        protected readonly IContentLoader _contentLoader;
        protected readonly IHtmlDownloader _htmlDownloader;

        protected MailService(HttpContextBase httpContextBase, 
            UrlResolver urlResolver, 
            IContentLoader contentLoader,
            IHtmlDownloader htmlDownloader)
        {
            this._httpContextBase = httpContextBase;
            this._urlResolver = urlResolver;
            this._contentLoader = contentLoader;
            this._htmlDownloader = htmlDownloader;
        }

        public virtual void Send(PageReference mailReference, NameValueCollection nameValueCollection, string toEmail, string language)
        {
            var body = this.GetHtmlBodyForMail(mailReference, nameValueCollection, language);
            var mailPage = this._contentLoader.Get<IMailPage>(mailReference);

            this.Send(mailPage.MailTitle, body, toEmail);
        }

        public virtual string GetHtmlBodyForMail(PageReference mailReference, NameValueCollection nameValueCollection, string language)
        {
            var urlBuilder = new UrlBuilder(this._urlResolver.GetUrl(mailReference, language))
            {
                QueryCollection = nameValueCollection
            };

            string basePath = this._httpContextBase.Request.Url.GetLeftPart(UriPartial.Authority);
            string relativePath = urlBuilder.ToString();
            
            if (relativePath.StartsWith(basePath))
            {
                relativePath = relativePath.Substring(basePath.Length);
            }

            string body = this._htmlDownloader.Download(basePath, relativePath);

            return body;
        }

        public virtual void Send(string subject, string body, string recipientMailAddress)
        {
            MailMessage message = new MailMessage()
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(recipientMailAddress);

            this.Send(message);
        }

        public virtual void Send(MailMessage message)
        {
            using (SmtpClient client = new SmtpClient())
            {
                // The SMTP host, port and sender e-mail address are configured
                // in the system.net section in web.config.
                client.Send(message);
            }
        }

        public virtual Task SendAsync(IdentityMessage message)
        {
            this.Send(message.Subject, message.Body, message.Destination);
            return Task.FromResult(0);
        }
    }
}