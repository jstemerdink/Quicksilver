using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Routing;

using EPiServer.Core;
using EPiServer.Globalization;

using Mediachase.Commerce;

namespace EPiServer.Reference.Commerce.Domain.Services
{
    public class LanguageService : IUpdateCurrentLanguage
    {
        private const string LanguageCookie = "Language";
        private readonly ICurrentMarket _currentMarket;
        private readonly CookieService _cookieService;
        private readonly IUpdateCurrentLanguage _defaultUpdateCurrentLanguage;
        private readonly RequestContext _requestContext;

        public LanguageService(ICurrentMarket currentMarket, CookieService cookieService, IUpdateCurrentLanguage defaultUpdateCurrentLanguage, RequestContext requestContext)
        {
            this._currentMarket = currentMarket;
            this._cookieService = cookieService;
            this._defaultUpdateCurrentLanguage = defaultUpdateCurrentLanguage;
            this._requestContext = requestContext;
        }

        public virtual IEnumerable<CultureInfo> GetAvailableLanguages()
        {
            return this.CurrentMarket.Languages;
        }
        
        public virtual CultureInfo GetCurrentLanguage()
        {
            CultureInfo cultureInfo;
            return this.TryGetLanguage(this._cookieService.Get(LanguageCookie), out cultureInfo)
                ? cultureInfo
                : this.CurrentMarket.DefaultLanguage;
        }

        public virtual bool SetCurrentLanguage(string language)
        {
            CultureInfo cultureInfo;
            if (!this.TryGetLanguage(language, out cultureInfo))
            {
                return false;
            }

            this._defaultUpdateCurrentLanguage.UpdateLanguage(language);
            this._cookieService.Set(LanguageCookie, language);
            return true;
        }

        private bool TryGetLanguage(string language, out CultureInfo cultureInfo)
        {
            cultureInfo = null;

            if (language == null)
            {
                return false;
            }

            try
            {
                var culture = CultureInfo.GetCultureInfo(language);
                cultureInfo = this.GetAvailableLanguages().FirstOrDefault(c => c.Name == culture.Name);
                return cultureInfo != null;
            }
            catch (CultureNotFoundException)
            {
                return false;
            }
        }

        private IMarket CurrentMarket
        {
            get { return this._currentMarket.GetCurrentMarket(); }
        }

        public virtual void UpdateLanguage(string languageId)
        {

            if (this._requestContext.HttpContext != null && this._requestContext.HttpContext.Request.Url != null && this._requestContext.HttpContext.Request.Url.AbsolutePath == "/")
            {
                var languageCookie = this._cookieService.Get(LanguageCookie);
                if (languageCookie != null)
                {
                    this._defaultUpdateCurrentLanguage.UpdateLanguage(languageCookie);
                    return;
                }

                var currentMarket = this._currentMarket.GetCurrentMarket();
                if (currentMarket != null && currentMarket.DefaultLanguage != null)
                {
                    this._defaultUpdateCurrentLanguage.UpdateLanguage(currentMarket.DefaultLanguage.Name);
                    return;
                }
            }

            this._defaultUpdateCurrentLanguage.UpdateLanguage(languageId);
        }

        public virtual void UpdateReplacementLanguage(IContent currentContent, string requestedLanguage)
        {
            this._defaultUpdateCurrentLanguage.UpdateReplacementLanguage(currentContent, requestedLanguage);
        }
    }
}