using System;
using System.Collections.Generic;
using System.Linq;

using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Reference.Commerce.Domain.Contracts.Services;
using EPiServer.Reference.Commerce.Domain.Facades;
using EPiServer.ServiceLocation;

using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Markets;
using Mediachase.Commerce.Pricing;

namespace EPiServer.Reference.Commerce.Domain.Services
{
    public abstract class PromotionService : IPromotionService
    {
        protected readonly IContentLoader _contentLoader;
        protected readonly IPricingService _pricingService;
        protected readonly IMarketService _marketService;
        protected readonly ReferenceConverter _referenceConverter;
        protected readonly IPromotionEntryService _promotionEntryService;
        protected readonly PromotionHelperFacade _promotionHelper;

        protected PromotionService(
            IPricingService pricingService, 
            IMarketService marketService, 
            IContentLoader contentLoader, 
            ReferenceConverter referenceConverter, 
            PromotionHelperFacade promotionHelper,
            IPromotionEntryService promotionEntryService)
        {
            this._contentLoader = contentLoader;
            this._marketService = marketService;
            this._pricingService = pricingService;
            this._referenceConverter = referenceConverter;
            this._promotionEntryService = promotionEntryService;
            this._promotionHelper = promotionHelper;
        }
        
        public virtual IList<IPriceValue> GetDiscountPriceList(IEnumerable<CatalogKey> catalogKeys, MarketId marketId, Currency currency)
        {
            if (this._marketService.GetMarket(marketId) == null)
            {
                throw new ArgumentException(string.Format("market '{0}' does not exist", marketId));
            }

            var priceFilter = new PriceFilter
            {
                CustomerPricing = new[] { CustomerPricing.AllCustomers },
                Quantity = 1,
                ReturnCustomerPricing = true,
            };
            if (currency != Currency.Empty)
            {
                priceFilter.Currencies = new[] { currency };
            }
            var prices = catalogKeys.SelectMany(x => this._pricingService.GetPriceList(x.CatalogEntryCode, marketId, priceFilter));

            return this.GetDiscountPrices(prices.ToList(), marketId, currency);
        }

        public virtual IPriceValue GetDiscountPrice(CatalogKey catalogKey, MarketId marketId, Currency currency)
        {
            return this.GetDiscountPriceList(new[] { catalogKey }, marketId, currency).FirstOrDefault();
        }

        private IList<IPriceValue> GetDiscountPrices(IList<IPriceValue> prices, MarketId marketId, Currency currency)
        {
            currency = this.GetCurrency(currency, marketId);

            var priceValues = new List<IPriceValue>();
            
            this._promotionHelper.Reset();
            
            foreach (var entry in this.GetEntries(prices))
            {
                var price = prices
                    .OrderBy(x => x.UnitPrice.Amount)
                    .FirstOrDefault(x => x.CatalogKey.CatalogEntryCode.Equals(entry.Code) &&
                        x.UnitPrice.Currency.Equals(currency));
                if (price == null)
                {
                    continue;
                }

                priceValues.Add(this._promotionEntryService.GetDiscountPrice(
                    price, entry, currency, this._promotionHelper));
                
            }
            return priceValues;
        }
       
        private Currency GetCurrency(Currency currency, MarketId marketId)
        {
            return currency == Currency.Empty ? this._marketService.GetMarket(marketId).DefaultCurrency : currency;
        }

        private IEnumerable<EntryContentBase> GetEntries(IEnumerable<IPriceValue> prices)
        {
            return prices.GroupBy(x => x.CatalogKey.CatalogEntryCode)
                .Select(x => x.First())
                .Select(x => this._contentLoader.Get<EntryContentBase>(
                    this._referenceConverter.GetContentLink(x.CatalogKey.CatalogEntryCode, CatalogContentType.CatalogEntry)));
        }
    }
}