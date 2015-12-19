using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Reference.Commerce.Domain.Contracts.Services;
using EPiServer.Reference.Commerce.Domain.Facades;
using EPiServer.Reference.Commerce.Site.Infrastructure.Facades;
using EPiServer.ServiceLocation;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Marketing;
using Mediachase.Commerce.Markets;
using Mediachase.Commerce.Pricing;

namespace EPiServer.Reference.Commerce.Site.Features.Shared.Services
{
    [ServiceConfiguration(typeof(IPromotionService), Lifecycle = ServiceInstanceScope.Singleton)]
    public class PromotionService : Domain.Services.PromotionService
    {
        public PromotionService(IPricingService pricingService, IMarketService marketService, IContentLoader contentLoader, ReferenceConverter referenceConverter, PromotionHelperFacade promotionHelper, IPromotionEntryService promotionEntryService)
            : base(pricingService, marketService, contentLoader, referenceConverter, promotionHelper, promotionEntryService)
        {
        }
    }
}