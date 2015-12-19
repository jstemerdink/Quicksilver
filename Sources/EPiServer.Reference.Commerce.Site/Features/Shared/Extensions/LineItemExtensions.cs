using EPiServer.Commerce.Catalog;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Reference.Commerce.Site.Features.Cart.Models;
using EPiServer.ServiceLocation;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Orders;
using System;
using System.Web;

using EPiServer.Reference.Commerce.Domain.Models;
using EPiServer.Reference.Commerce.Extensions;

using CartItem = EPiServer.Reference.Commerce.Site.Features.Cart.Models.CartItem;

namespace EPiServer.Reference.Commerce.Site.Features.Shared.Extensions
{
    public static class LineItemExtensions
    {

        public static string GetThumbnailUrl(this CartItem cartItem)
        {
            return Commerce.Extensions.LineItemExtensions.GetThumbnailUrl(cartItem.Code);
        }
    }
}