using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.Linking;
using EPiServer.Core;
using EPiServer.Globalization;
using EPiServer.Reference.Commerce.Site.Features.Cart.Models;
using EPiServer.Reference.Commerce.Site.Features.Checkout.Models;
using EPiServer.Reference.Commerce.Site.Features.Product.Models;
using EPiServer.Reference.Commerce.Site.Features.Shared.Extensions;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Commerce.Pricing;
using Mediachase.Commerce.Website.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using EPiServer.Reference.Commerce.Domain.Contracts.Factories;
using EPiServer.Reference.Commerce.Domain.Contracts.Services;


namespace EPiServer.Reference.Commerce.Site.Features.Cart.Services
{
    [ServiceConfiguration(typeof(ICartService), Lifecycle = ServiceInstanceScope.Unique)]
    public class CartService : Domain.Services.CartService
    {
        public CartService(Func<string, CartHelper> cartHelper, IContentLoader contentLoader, ReferenceConverter referenceConverter, UrlResolver urlResolver, IProductService productService, IPricingService pricingService, ICartFactory cartFactory)
            : base(cartHelper, contentLoader, referenceConverter, urlResolver, productService, pricingService, cartFactory)
        {
        }
    }
}