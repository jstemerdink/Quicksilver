using System;
using System.Collections.Generic;
using System.Linq;

using EPiServer.Reference.Commerce.Domain.Contracts.Services;
using EPiServer.Reference.Commerce.Domain.Facades;
using EPiServer.ServiceLocation;

using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Pricing;

namespace EPiServer.Reference.Commerce.Domain.Services
{
    public abstract class PricingService : IPricingService
    {
        protected readonly IPriceService _priceService;
        protected readonly ICurrentMarket _currentMarket;
        protected readonly ICurrencyService _currencyService;
        protected readonly AppContextFacade _appContext;

        protected PricingService(IPriceService priceService,
            ICurrentMarket currentMarket, 
            ICurrencyService currencyService,
            AppContextFacade appContext)
        {
            this._priceService = priceService;
            this._currentMarket = currentMarket;
            this._currencyService = currencyService;
            this._appContext = appContext;
        }

        public virtual IList<IPriceValue> GetPriceList(string code, MarketId marketId, PriceFilter priceFilter)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException("code");
            }

            var catalogKey = new CatalogKey(this._appContext.ApplicationId, code);

            return this._priceService.GetPrices(marketId, DateTime.Now, catalogKey, priceFilter)
                .OrderBy(x => x.UnitPrice.Amount)
                .ToList();
        }

        public virtual IList<IPriceValue> GetPriceList(IEnumerable<CatalogKey> catalogKeys, MarketId marketId, PriceFilter priceFilter)
        {
            if (catalogKeys == null)
            {
                throw new ArgumentNullException("catalogKeys");
            }

            if (!catalogKeys.Any())
            {
                return Enumerable.Empty<IPriceValue>().ToList();
            }

            return this._priceService.GetPrices(marketId, DateTime.Now, catalogKeys, priceFilter)
                .OrderBy(x => x.UnitPrice.Amount)
                .ToList();
        }

        public virtual Money GetCurrentPrice(string code)
        {
            var market = this._currentMarket.GetCurrentMarket();
            var currency = this._currencyService.GetCurrentCurrency();
            var prices = this.GetPriceList(code, market.MarketId,
                new PriceFilter
                {
                    Currencies = new[] { currency }
                });

            return prices.Any() ? prices.First().UnitPrice : new Money(0, currency);
        }
    }
}