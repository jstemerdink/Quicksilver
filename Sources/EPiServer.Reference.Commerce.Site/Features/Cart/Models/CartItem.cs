using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Reference.Commerce.Site.Features.Checkout.Models;
using Mediachase.Commerce;
using System.Collections.Generic;

using EPiServer.Reference.Commerce.Domain.Contracts.Models;

namespace EPiServer.Reference.Commerce.Site.Features.Cart.Models
{
    public class CartItem : Domain.Models.CartItem
    {
        public string Brand { get; set; }
    }
}