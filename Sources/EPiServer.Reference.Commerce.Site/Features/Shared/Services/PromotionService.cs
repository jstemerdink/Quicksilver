﻿using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Reference.Commerce.Site.Features.Cart.Services;
using EPiServer.Reference.Commerce.Site.Infrastructure.Facades;
using EPiServer.ServiceLocation;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Marketing;
using Mediachase.Commerce.Markets;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Pricing;

namespace EPiServer.Reference.Commerce.Site.Features.Shared.Services
{
    [ServiceConfiguration(typeof(IPromotionService), Lifecycle = ServiceInstanceScope.HybridHttpSession)]
    public class PromotionService : IPromotionService
    {
        private readonly IContentLoader _contentLoader;
        private readonly IPricingService _pricingService;
        private readonly IMarketService _marketService;
        private readonly ReferenceConverter _referenceConverter;
        private readonly IPromotionEntryService _promotionEntryService;
        private readonly PromotionHelperFacade _promotionHelper;

        public PromotionService(
            IPricingService pricingService, 
            IMarketService marketService, 
            IContentLoader contentLoader, 
            ReferenceConverter referenceConverter, 
            PromotionHelperFacade promotionHelper,
            IPromotionEntryService promotionEntryService)
        {
            _contentLoader = contentLoader;
            _marketService = marketService;
            _pricingService = pricingService;
            _referenceConverter = referenceConverter;
            _promotionEntryService = promotionEntryService;
            _promotionHelper = promotionHelper;
        }
        
        public IList<IPriceValue> GetDiscountPriceList(IEnumerable<CatalogKey> catalogKeys, MarketId marketId, Currency currency)
        {
            if (_marketService.GetMarket(marketId) == null)
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
            var prices = catalogKeys.SelectMany(x => _pricingService.GetPriceList(x.CatalogEntryCode, marketId, priceFilter));

            return GetDiscountPrices(prices.ToList(), marketId, currency);
        }

        public IPriceValue GetDiscountPrice(CatalogKey catalogKey, MarketId marketId, Currency currency)
        {
            return GetDiscountPriceList(new[] { catalogKey }, marketId, currency).FirstOrDefault();
        }

        public List<Discount> GetAllDiscounts(Mediachase.Commerce.Orders.Cart cart)
        {
           List<Discount> discounts = new List<Discount>();

            foreach (OrderForm form in cart.OrderForms)
            {
                foreach (
                    Discount discount in
                        form.Discounts.Cast<Discount>().Where(x => !string.IsNullOrEmpty(x.DiscountCode)))
                {
                    AddToDiscountList(discount, discounts);
                }

                foreach (LineItem item in form.LineItems)
                {
                    foreach (
                        Discount discount in
                            item.Discounts.Cast<Discount>().Where(x => !string.IsNullOrEmpty(x.DiscountCode)))
                    {
                        AddToDiscountList(discount, discounts);
                    }
                }

                foreach (Shipment shipment in form.Shipments)
                {
                    foreach (
                        Discount discount in
                            shipment.Discounts.Cast<Discount>().Where(x => !string.IsNullOrEmpty(x.DiscountCode)))
                    {
                        AddToDiscountList(discount, discounts);
                    }
                }
            }
            return discounts;
        }


        private static void AddToDiscountList(Discount discount, List<Discount> discounts)
        {
            if (!discounts.Exists(x => x.DiscountCode.Equals(discount.DiscountCode)))
            {
                discounts.Add(discount);
            }
        }

        private IList<IPriceValue> GetDiscountPrices(IList<IPriceValue> prices, MarketId marketId, Currency currency)
        {
            currency = GetCurrency(currency, marketId);

            var priceValues = new List<IPriceValue>();
            
            _promotionHelper.Reset();
            
            foreach (var entry in GetEntries(prices))
            {
                var price = prices
                    .OrderBy(x => x.UnitPrice.Amount)
                    .FirstOrDefault(x => x.CatalogKey.CatalogEntryCode.Equals(entry.Code) &&
                        x.UnitPrice.Currency.Equals(currency));
                if (price == null)
                {
                    continue;
                }

                priceValues.Add(_promotionEntryService.GetDiscountPrice(
                    price, entry, currency, _promotionHelper));
                
            }
            return priceValues;
        }
       
        private Currency GetCurrency(Currency currency, MarketId marketId)
        {
            return currency == Currency.Empty ? _marketService.GetMarket(marketId).DefaultCurrency : currency;
        }

        private IEnumerable<EntryContentBase> GetEntries(IEnumerable<IPriceValue> prices)
        {
            return prices.GroupBy(x => x.CatalogKey.CatalogEntryCode)
                .Select(x => x.First())
                .Select(x => _contentLoader.Get<EntryContentBase>(
                    _referenceConverter.GetContentLink(x.CatalogKey.CatalogEntryCode, CatalogContentType.CatalogEntry)));
        }
    }
}