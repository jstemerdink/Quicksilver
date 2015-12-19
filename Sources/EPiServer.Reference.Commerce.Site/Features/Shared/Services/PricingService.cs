using EPiServer.ServiceLocation;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Pricing;
using System;
using System.Collections.Generic;
using System.Linq;

using EPiServer.Reference.Commerce.Domain.Contracts.Services;
using EPiServer.Reference.Commerce.Domain.Facades;
using EPiServer.Reference.Commerce.Site.Infrastructure.Facades;

namespace EPiServer.Reference.Commerce.Site.Features.Shared.Services
{
    [ServiceConfiguration(typeof(IPricingService), Lifecycle = ServiceInstanceScope.Singleton)]
    public class PricingService : Domain.Services.PricingService
    {
       public PricingService(IPriceService priceService, ICurrentMarket currentMarket, ICurrencyService currencyService, AppContextFacade appContext)
            : base(priceService, currentMarket, currencyService, appContext)
        {
        }
    }
}