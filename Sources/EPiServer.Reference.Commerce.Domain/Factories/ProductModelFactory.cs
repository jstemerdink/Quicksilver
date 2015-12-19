using System.Linq;

using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Reference.Commerce.Domain.Contracts.Factories;
using EPiServer.Reference.Commerce.Domain.Contracts.Models;
using EPiServer.Reference.Commerce.Domain.Contracts.Services;
using EPiServer.Reference.Commerce.Domain.Facades;
using EPiServer.Reference.Commerce.Domain.Models.ViewModels;
using EPiServer.Reference.Commerce.Extensions;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;

using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;

namespace EPiServer.Reference.Commerce.Domain.Factories
{
    public abstract class ProductModelFactory : IProductFactory
    {

        protected readonly IPricingService _pricingService;
        protected readonly AppContextFacade _appContext;
        protected readonly IPromotionService _promotionService;
        protected readonly ICurrentMarket _currentMarket;
        protected readonly ICurrencyService _currencyService;
        protected readonly IContentLoader _contentLoader;
        protected readonly UrlResolver _urlResolver;

        protected ProductModelFactory(
           IPricingService pricingService,
           AppContextFacade appContext,
        IPromotionService promotionService,
        ICurrentMarket currentMarket,
       ICurrencyService currencyService,
        IContentLoader contentLoader,
        UrlResolver urlResolver
            )
        {
            this._pricingService = pricingService;
            _appContext = appContext;
            _promotionService = promotionService;
            _currentMarket = currentMarket;
            _currencyService = currencyService;
            _contentLoader = contentLoader;
            _urlResolver = urlResolver;
        }

        public virtual IProductModel Create(ProductContent product, VariationContent variation)
        {
            var market = _currentMarket.GetCurrentMarket();
            var currency = _currencyService.GetCurrentCurrency();
            var originalPrice = _pricingService.GetCurrentPrice(variation.Code);
            var discountPrice = GetDiscountPrice(variation, market, currency, originalPrice);
            var image = variation.GetAssets<IContentImage>(_contentLoader, _urlResolver).FirstOrDefault() ?? "";

            return new ProductViewModel
            {
                DisplayName = product != null ? product.DisplayName : variation.DisplayName,
                PlacedPrice = originalPrice,
                ExtendedPrice = discountPrice,
                ImageUrl = image,
                Url = variation.GetUrl()
            };
        }

        protected Money GetDiscountPrice(VariationContent variation, IMarket market, Currency currency, Money orginalPrice)
        {
            var discountPrice = _promotionService.GetDiscountPrice(new CatalogKey(_appContext.ApplicationId, variation.Code), market.MarketId, currency);
            if (discountPrice != null)
            {
                return discountPrice.UnitPrice;
            }

            return orginalPrice;
        }
    }
}
