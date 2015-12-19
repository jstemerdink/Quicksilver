using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Reference.Commerce.Domain.Contracts.Factories;
using EPiServer.Reference.Commerce.Domain.Contracts.Models;
using EPiServer.Reference.Commerce.Domain.Contracts.Services;
using EPiServer.Reference.Commerce.Domain.Models;
using EPiServer.Reference.Commerce.Extensions;
using EPiServer.Reference.Commerce.Site.Features.Product.Models;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;

using Mediachase.Commerce;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Website.Helpers;

namespace EPiServer.Reference.Commerce.Site.Features.Cart.Services
{
    [ServiceConfiguration(typeof(ICartFactory), Lifecycle = ServiceInstanceScope.Unique)]
    public class CartItemFactory : Domain.Factories.CartItemFactory
    {
        public CartItemFactory(IContentLoader contentLoader, UrlResolver urlResolver, IProductService productService)
            : base(contentLoader, urlResolver, productService)
        {
        }

        public override ICartItem Create(LineItem lineItem, VariationContent variant, ProductContent product, CartHelper cartHelper)
        {

            Models.CartItem item = new Models.CartItem
            {
                Code = lineItem.Code,
                DisplayName = lineItem.DisplayName,
                ImageUrl = variant.GetAssets<IContentImage>(_contentLoader, _urlResolver).FirstOrDefault() ?? "",
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

            if (product is FashionProduct)
            {
                var fashionProduct = (FashionProduct)product;
                var fashionVariant = (FashionVariant)variant;
                item.Brand = fashionProduct.Brand;
                var variations = _productService.GetVariations(fashionProduct);
                item.AvailableSizes = variations.Cast<FashionVariant>().Where(x => x.Color == fashionVariant.Color).Select(x => x.Size);
            }

            return item;
        }
    }
}