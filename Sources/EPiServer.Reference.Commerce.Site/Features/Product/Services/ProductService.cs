using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.Linking;
using EPiServer.Core;
using EPiServer.Globalization;
using EPiServer.Reference.Commerce.Site.Features.Product.Models;
using EPiServer.Reference.Commerce.Site.Features.Shared.Extensions;
using EPiServer.Reference.Commerce.Site.Infrastructure.Facades;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using EPiServer.Reference.Commerce.Domain.Contracts.Factories;
using EPiServer.Reference.Commerce.Domain.Contracts.Models;
using EPiServer.Reference.Commerce.Domain.Contracts.Services;
using EPiServer.Reference.Commerce.Domain.Facades;
using EPiServer.Reference.Commerce.Extensions;

using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;

namespace EPiServer.Reference.Commerce.Site.Features.Product.Services
{
    [ServiceConfiguration(typeof(IProductService), Lifecycle = ServiceInstanceScope.Singleton)]
    public class ProductService : Domain.Services.ProductService
    {
        public ProductService(
            IContentLoader contentLoader,
            IPromotionService promotionService,
            IPricingService pricingService,
            UrlResolver urlResolver,
            LinksRepository linksRepository,
            IRelationRepository relationRepository,
            ICurrentMarket currentMarket,
            ICurrencyService currencyService,
            AppContextFacade appContext,
            ReferenceConverter referenceConverter,
            IProductFactory productFactory)
            : base(
                contentLoader,
                promotionService,
                pricingService,
                urlResolver,
                linksRepository,
                relationRepository,
                currentMarket,
                currencyService,
                appContext,
                referenceConverter,
                productFactory)
        {
        }

        public override IProductModel CreateProductViewModel(ProductContent product, VariationContent variation)
        {
            if (variation == null)
            {
                return null;
            }

            ContentReference productContentReference;
            if (product != null)
            {
                productContentReference = product.ContentLink;
            }
            else
            {
                productContentReference = variation.GetParentProducts(_relationRepository).FirstOrDefault();
                if (ContentReference.IsNullOrEmpty(productContentReference))
                {
                    return null;
                }
            }
            
            return this._productFactory.Create(product, variation) as ProductViewModel;

        }
    }
}