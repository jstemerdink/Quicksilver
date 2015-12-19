using System.Collections.Generic;

using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Reference.Commerce.Domain.Contracts.Models;

using Mediachase.Commerce;

namespace EPiServer.Reference.Commerce.Domain.Models
{
    public class CartItem : ICartItem
    {
        public string DisplayName { get; set; }
        public string ImageUrl { get; set; }
        public string Url { get; set; }
        public Money ExtendedPrice { get; set; }
        public Money PlacedPrice { get; set; }
        public string Code { get; set; }
        public VariationContent Variant { get; set; }
        public decimal Quantity { get; set; }
        public Money DiscountPrice { get; set; }
        public IEnumerable<IOrderDiscountModel> Discounts { get; set; }
        public IEnumerable<string> AvailableSizes { get; set; }
    }
}