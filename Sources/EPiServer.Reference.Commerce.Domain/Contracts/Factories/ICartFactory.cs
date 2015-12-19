using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Reference.Commerce.Domain.Contracts.Models;

using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Website.Helpers;

namespace EPiServer.Reference.Commerce.Domain.Contracts.Factories
{
    public interface ICartFactory
    {
        ICartItem Create(LineItem lineItem, VariationContent variant, ProductContent product, CartHelper cartHelper);
    }
}
