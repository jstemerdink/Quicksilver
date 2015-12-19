using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using EPiServer.Core;
using EPiServer.Reference.Commerce.Domain.Contracts.Models;
using EPiServer.Reference.Commerce.Domain.Models.ViewModels;

using Mediachase.Commerce;

namespace EPiServer.Reference.Commerce.Site.Features.Cart.Models
{
    public class MiniCartViewModel : CartViewModel
    {
        public ContentReference CheckoutPage { get; set; }

        public new IEnumerable<CartItem> CartItems { get; set; }
    }
}