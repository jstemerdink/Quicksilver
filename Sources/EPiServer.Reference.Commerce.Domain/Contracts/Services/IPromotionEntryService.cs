using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Reference.Commerce.Domain.Facades;

using Mediachase.Commerce;
using Mediachase.Commerce.Pricing;

namespace EPiServer.Reference.Commerce.Domain.Contracts.Services
{
    public interface IPromotionEntryService
    {
        IPriceValue GetDiscountPrice(IPriceValue price, EntryContentBase entry, Currency currency,
            PromotionHelperFacade promotionHelper);
        
    }
}
