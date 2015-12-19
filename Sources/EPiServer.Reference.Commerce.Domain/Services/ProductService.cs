using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.Linking;
using EPiServer.Core;
using EPiServer.Globalization;
using EPiServer.Reference.Commerce.Domain.Contracts.Factories;
using EPiServer.Reference.Commerce.Domain.Contracts.Models;
using EPiServer.Reference.Commerce.Domain.Contracts.Services;
using EPiServer.Reference.Commerce.Domain.Facades;
using EPiServer.Reference.Commerce.Domain.Models;
using EPiServer.Reference.Commerce.Extensions;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;

using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;

namespace EPiServer.Reference.Commerce.Domain.Services
{
    public abstract class ProductService : IProductService
    {
        protected readonly IContentLoader _contentLoader;
        protected readonly IPromotionService _promotionService;
        protected readonly IPricingService _pricingService;
        protected readonly UrlResolver _urlResolver;
        protected readonly LinksRepository _linksRepository;
        protected readonly IRelationRepository _relationRepository;
        protected readonly CultureInfo _preferredCulture;
        protected readonly ICurrentMarket _currentMarket;
        protected readonly ICurrencyService _currencyService;
        protected readonly AppContextFacade _appContext;
        protected readonly ReferenceConverter _referenceConverter;
        protected readonly IProductFactory _productFactory;

        protected ProductService(IContentLoader contentLoader,
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
        {
            this._contentLoader = contentLoader;
            this._promotionService = promotionService;
            this._pricingService = pricingService;
            this._urlResolver = urlResolver;
            this._linksRepository = linksRepository;
            this._relationRepository = relationRepository;
            this._preferredCulture = ContentLanguage.PreferredCulture;
            this._currentMarket = currentMarket;
            this._currencyService = currencyService;
            this._appContext = appContext;
            this._referenceConverter = referenceConverter;
            this._productFactory = productFactory;
        }

        public virtual IEnumerable<VariationContent> GetVariations(ProductContent currentContent)
        {
            return this._contentLoader
                .GetItems(currentContent.GetVariants(this._relationRepository), this._preferredCulture)
                .Cast<VariationContent>()
                .Where(v => v.IsAvailableInCurrentMarket(this._currentMarket));
        }

        public virtual IEnumerable<IProductModel> GetVariationsAndPricesForProducts(IEnumerable<ProductContent> products)
        {
            var variationsToLoad = new Dictionary<ContentReference, ContentReference>();
            var fashionProducts = products.ToList();
            foreach (var product in fashionProducts)
            {
                var relations = this._linksRepository.GetRelationsBySource(product.VariantsReference).OfType<ProductVariation>();
                variationsToLoad.Add(relations.First().Target, product.ContentLink);
            }

            var variations = this._contentLoader.GetItems(variationsToLoad.Select(x => x.Key), this._preferredCulture).Cast<VariationContent>();

            var productModels = new List<IProductModel>();

            foreach (var variation in variations)
            {
                var productContentReference = variationsToLoad.First(x => x.Key == variation.ContentLink).Value;
                var product = fashionProducts.First(x => x.ContentLink == productContentReference);
                productModels.Add(CreateProductViewModel(product, variation));
            }
            return productModels;
        }

        public virtual IProductModel GetProductViewModel(ProductContent product)
        {
            var variations = this._contentLoader.GetItems(product.GetVariants(), this._preferredCulture).
                                            Cast<VariationContent>()
                                           .ToList();

            var variation = variations.FirstOrDefault();
            return this.CreateProductViewModel(product, variation);
        }

        public virtual IProductModel GetProductViewModel(VariationContent variation)
        {
            return this.CreateProductViewModel(null, variation);
        }

        public virtual IProductModel CreateProductViewModel(ProductContent product, VariationContent variation)
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
                productContentReference = variation.GetParentProducts(this._relationRepository).FirstOrDefault();
                
            }

            if (ContentReference.IsNullOrEmpty(productContentReference))
            {
                return null;
            }

            var market = this._currentMarket.GetCurrentMarket();
            var currency = this._currencyService.GetCurrentCurrency();
            var originalPrice = this._pricingService.GetCurrentPrice(variation.Code);
            var discountPrice = this.GetDiscountPrice(variation, market, currency, originalPrice);
            var image = variation.GetAssets<IContentImage>(this._contentLoader, this._urlResolver).FirstOrDefault() ?? "";

            return this._productFactory.Create(product, variation);
        }

        protected Money GetDiscountPrice(VariationContent variation, IMarket market, Currency currency, Money orginalPrice)
        {
            var discountPrice = this._promotionService.GetDiscountPrice(new CatalogKey(this._appContext.ApplicationId, variation.Code), market.MarketId, currency);
            if (discountPrice != null)
            {
                return discountPrice.UnitPrice;
            }

            return orginalPrice;
        }
    }
}