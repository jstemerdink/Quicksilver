using System.Collections.Generic;
using System.Linq;

using EPiServer.Reference.Commerce.Domain.Contracts.Services;
using EPiServer.Reference.Commerce.Domain.Services;
using EPiServer.ServiceLocation;
using Mediachase.Commerce;

namespace EPiServer.Reference.Commerce.Site.Features.Market.Services
{
    [ServiceConfiguration(typeof(ICurrencyService), Lifecycle = ServiceInstanceScope.Singleton)]
    public class CurrencyService : Domain.Services.CurrencyService
    {
        public CurrencyService(ICurrentMarket currentMarket, CookieService cookieService)
            : base(currentMarket, cookieService)
        {
        }
    }
}