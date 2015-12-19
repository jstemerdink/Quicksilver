using System.Collections.Generic;

using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Reference.Commerce.Domain.Contracts.Models;

namespace EPiServer.Reference.Commerce.Domain.Contracts.Services
{
    public interface IProductService
    {
        IEnumerable<IProductModel> GetVariationsAndPricesForProducts(IEnumerable<ProductContent> products);
        IProductModel GetProductViewModel(ProductContent product);
        IProductModel GetProductViewModel(VariationContent variation);
        IEnumerable<VariationContent> GetVariations(ProductContent currentContent);
    }
}