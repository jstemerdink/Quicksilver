using System;
using System.Linq;

using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Reference.Commerce.Domain.Contracts.Factories;
using EPiServer.Reference.Commerce.Domain.Contracts.Models;
using EPiServer.Reference.Commerce.Domain.Contracts.Services;
using EPiServer.Reference.Commerce.Domain.Models;
using EPiServer.Reference.Commerce.Extensions;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;

using Mediachase.Commerce;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Website.Helpers;

namespace EPiServer.Reference.Commerce.Domain.Factories
{
    public abstract  class CartFactory : ICartFactory
    {
        protected readonly IContentLoader _contentLoader;
        protected readonly UrlResolver _urlResolver;
        protected readonly IProductService _productService;

        protected CartFactory(
            IContentLoader contentLoader,
            UrlResolver urlResolver,
            IProductService productService
            )
        {
            this._contentLoader = contentLoader;
            this._urlResolver = urlResolver;
            this._productService = productService;
        }

        public virtual ICartItem Create(LineItem lineItem, VariationContent variant, ProductContent product, CartHelper cartHelper)
        {

            CartItem item = new CartItem
            {
                Code = lineItem.Code,
                DisplayName = lineItem.DisplayName,
                ImageUrl = variant.GetAssets<IContentImage>(this._contentLoader, this._urlResolver).FirstOrDefault() ?? "",
                ExtendedPrice = lineItem.ToMoney(lineItem.ExtendedPrice + lineItem.OrderLevelDiscountAmount),
                PlacedPrice = lineItem.ToMoney(lineItem.PlacedPrice),
                DiscountPrice = lineItem.ToMoney(Math.Round(((lineItem.PlacedPrice * lineItem.Quantity) - lineItem.Discounts.Cast<LineItemDiscount>().Sum(x => x.DiscountValue)) / lineItem.Quantity, 2)),
                Quantity = lineItem.Quantity,
                Url = lineItem.GetUrl(),
                Variant = variant,
                Discounts = lineItem.Discounts.Cast<LineItemDiscount>().Select(x => new OrderDiscountModel
                {
                    Discount = new Money(x.DiscountAmount, new Currency(cartHelper.Cart.BillingCurrency)),
                    Displayname = x.DisplayMessage
                })
            };

            return item;
        }
    }
}
