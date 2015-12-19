using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EPiServer.Commerce.Catalog.ContentTypes;

using Mediachase.Commerce;

namespace EPiServer.Reference.Commerce.Domain.Contracts.Models
{
    public interface ICartItem : IProductModel
    {
        VariationContent Variant { get; set; }
        decimal Quantity { get; set; }
        Money DiscountPrice { get; set; }
        IEnumerable<IOrderDiscountModel> Discounts { get; set; }
    }
}
