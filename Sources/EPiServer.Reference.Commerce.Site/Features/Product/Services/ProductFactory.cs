using System.Linq;

using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Reference.Commerce.Domain.Contracts.Factories;
using EPiServer.Reference.Commerce.Domain.Contracts.Models;
using EPiServer.Reference.Commerce.Domain.Contracts.Services;
using EPiServer.Reference.Commerce.Domain.Facades;
using EPiServer.Reference.Commerce.Domain.Models.ViewModels;
using EPiServer.Reference.Commerce.Extensions;
using EPiServer.Reference.Commerce.Site.Features.Product.Models;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;

using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;

using ProductViewModel = EPiServer.Reference.Commerce.Site.Features.Product.Models.ProductViewModel;

namespace EPiServer.Reference.Commerce.Site.Features.Product.Services
{
    [ServiceConfiguration(typeof(IProductFactory), Lifecycle = ServiceInstanceScope.Unique)]
    public class ProductFactory : Domain.Factories.ProductFactory
    {
        public ProductFactory(IPricingService pricingService, AppContextFacade appContext, IPromotionService promotionService, ICurrentMarket currentMarket, ICurrencyService currencyService, IContentLoader contentLoader, UrlResolver urlResolver)
            : base(pricingService, appContext, promotionService, currentMarket, currencyService, contentLoader, urlResolver)
        {
        }

        public override IProductModel Create(ProductContent product, VariationContent variation)
        {
            var market = _currentMarket.GetCurrentMarket();
            var currency = _currencyService.GetCurrentCurrency();
            var originalPrice = _pricingService.GetCurrentPrice(variation.Code);
            var discountPrice = GetDiscountPrice(variation, market, currency, originalPrice);
            var image = variation.GetAssets<IContentImage>(_contentLoader, _urlResolver).FirstOrDefault() ?? "";
            var brand = product is FashionProduct ? ((FashionProduct)product).Brand : string.Empty;

            return new ProductViewModel
            {
                DisplayName = product != null ? product.DisplayName : variation.DisplayName,
                PlacedPrice = originalPrice,
                ExtendedPrice = discountPrice,
                ImageUrl = image,
                Url = variation.GetUrl(),
                Brand = brand
            };
        }
    }
}
