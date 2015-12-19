using System.Collections.Generic;

using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Pricing;

namespace EPiServer.Reference.Commerce.Domain.Contracts.Services
{
    public interface IPricingService
    {
        IList<IPriceValue> GetPriceList(string code, MarketId marketId, PriceFilter priceFilter);
        IList<IPriceValue> GetPriceList(IEnumerable<CatalogKey> catalogKeys, MarketId marketId, PriceFilter priceFilter);
        Money GetCurrentPrice(string code);
       
    }
}
