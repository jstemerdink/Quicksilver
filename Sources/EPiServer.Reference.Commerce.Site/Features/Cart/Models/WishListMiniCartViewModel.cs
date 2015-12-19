using EPiServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using EPiServer.Reference.Commerce.Domain.Models.ViewModels;

namespace EPiServer.Reference.Commerce.Site.Features.Cart.Models
{
    public class WishListMiniCartViewModel : CartViewModel
    {
        public ContentReference WishListPage { get; set; }

        public new IEnumerable<CartItem> CartItems { get; set; }
    }
}