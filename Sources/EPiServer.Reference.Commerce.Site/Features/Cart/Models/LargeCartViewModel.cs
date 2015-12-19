using System;
using System.Collections.Generic;
using System.Linq;

using EPiServer.Reference.Commerce.Domain.Contracts.Models;
using EPiServer.Reference.Commerce.Domain.Models.ViewModels;

using Mediachase.Commerce;

namespace EPiServer.Reference.Commerce.Site.Features.Cart.Models
{
    public class LargeCartViewModel : CartViewModel
    {
        public Money TotalDiscount { get; set; }

        public new IEnumerable<CartItem> CartItems { get; set; }
    }
}