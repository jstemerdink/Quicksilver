using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Reference.Commerce.Domain.Contracts.Models;

using Mediachase.Commerce;

namespace EPiServer.Reference.Commerce.Domain.Contracts.Factories
{
    public interface IProductFactory
    {
        IProductModel Create(ProductContent product, VariationContent variation);
    }
}
