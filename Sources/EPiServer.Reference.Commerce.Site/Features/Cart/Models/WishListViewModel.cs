﻿using EPiServer.Reference.Commerce.Site.Features.Cart.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using EPiServer.Reference.Commerce.Domain.Models.ViewModels;

namespace EPiServer.Reference.Commerce.Site.Features.Cart.Models
{
    public class WishListViewModel : CartViewModel 
    {
        public WishListPage CurrentPage { get; set; }

        public new IEnumerable<CartItem> CartItems { get; set; }
    }
}