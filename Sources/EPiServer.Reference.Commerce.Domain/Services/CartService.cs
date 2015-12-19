using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Globalization;
using EPiServer.Reference.Commerce.Domain.Contracts.Factories;
using EPiServer.Reference.Commerce.Domain.Contracts.Models;
using EPiServer.Reference.Commerce.Domain.Contracts.Services;
using EPiServer.Reference.Commerce.Domain.Models;
using EPiServer.Reference.Commerce.Extensions;
using EPiServer.Web.Routing;

using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Commerce.Website.Helpers;

namespace EPiServer.Reference.Commerce.Domain.Services
{
    public abstract class CartService : ICartService
    {
        protected readonly Func<string, CartHelper> _cartHelper;
        protected readonly IContentLoader _contentLoader;
        protected readonly ReferenceConverter _referenceConverter;
        protected readonly CultureInfo _preferredCulture;
        protected string _cartName = Cart.DefaultName;
        protected readonly UrlResolver _urlResolver;
        protected readonly IProductService _productService;
        protected readonly IPricingService _pricingService;
        protected readonly ICartFactory _cartFactory;

        protected CartService(Func<string, CartHelper> cartHelper, 
            IContentLoader contentLoader, 
            ReferenceConverter referenceConverter, 
            UrlResolver urlResolver, 
            IProductService productService,
            IPricingService pricingService, 
            ICartFactory cartFactory)
        {
            this._cartHelper = cartHelper;
            this._contentLoader = contentLoader;
            this._referenceConverter = referenceConverter;
            this._preferredCulture = ContentLanguage.PreferredCulture;
            this._urlResolver = urlResolver;
            this._productService = productService;
            this._pricingService = pricingService;
            this._cartFactory = cartFactory;
        }

        public virtual void InitializeAsWishList()
        {
            this._cartName = CartHelper.WishListName;
        }

        public virtual decimal GetLineItemsTotalQuantity()
        {
            return this.CartHelper.Cart.GetAllLineItems().Sum(x => x.Quantity);
        }

        public virtual IEnumerable<ICartItem> GetCartItems()
        {
            if (this.CartHelper.IsEmpty)
            {
                return Enumerable.Empty<ICartItem>();
            }

            var cartItems = new List<ICartItem>();
            var lineItems = this.CartHelper.Cart.GetAllLineItems();

            // In order to show the images for the items in the cart, we need to load the variants
            var variants = this._contentLoader.GetItems(lineItems.Select(x => this._referenceConverter.GetContentLink(x.Code)),
                this._preferredCulture).OfType<VariationContent>().ToList();

            foreach (var lineItem in lineItems)
            {
                VariationContent variant = variants.FirstOrDefault(x => x.Code == lineItem.Code);
                ProductContent product = _contentLoader.Get<ProductContent>(variant.GetParentProducts().FirstOrDefault());
                ICartItem item = this._cartFactory.Create(lineItem, variant, product, this.CartHelper);

                //ICartItem item = new CartItem
                //{
                //    Code = lineItem.Code,
                //    DisplayName = lineItem.DisplayName,
                //    ImageUrl = variant.GetAssets<IContentImage>(this._contentLoader, this._urlResolver).FirstOrDefault() ?? "",
                //    ExtendedPrice = lineItem.ToMoney(lineItem.ExtendedPrice + lineItem.OrderLevelDiscountAmount),
                //    PlacedPrice = lineItem.ToMoney(lineItem.PlacedPrice),
                //    DiscountPrice = lineItem.ToMoney(Math.Round(((lineItem.PlacedPrice * lineItem.Quantity) - lineItem.Discounts.Cast<LineItemDiscount>().Sum(x => x.DiscountValue)) / lineItem.Quantity, 2)),
                //    Quantity = lineItem.Quantity,
                //    Url = lineItem.GetUrl(),
                //    Variant = variant,
                //    Discounts = lineItem.Discounts.Cast<LineItemDiscount>().Select(x => new OrderDiscountModel
                //    {
                //        Discount = new Money(x.DiscountAmount, new Currency(this.CartHelper.Cart.BillingCurrency)),
                //        Displayname = x.DisplayMessage
                //    })
                //};

                cartItems.Add(item);
            }

            return cartItems;
        }

        public virtual Money GetTotal()
        {
            if (this.CartHelper.IsEmpty)
            {
                return this.ConvertToMoney(0);
            }

            return this.ConvertToMoney(this.CartHelper.Cart.Total);
        }

        public virtual Money GetTotalDiscount()
        {
            decimal amount = 0;

            if (this.CartHelper.IsEmpty)
            {
                return this.ConvertToMoney(amount);
            }

            amount = this.CartHelper.Cart.GetAllLineItems().Sum(x => x.LineItemDiscountAmount);

            return this.ConvertToMoney(amount);
        }

        public virtual bool AddToCart(string code, out string warningMessage)
        {
            var entry = CatalogContext.Current.GetCatalogEntry(code);
            this.CartHelper.AddEntry(entry);
            this.CartHelper.Cart.ProviderId = "frontend"; // if this is not set explicitly, place price does not get updated by workflow
            this.ValidateCart(out warningMessage);

            return this.CartHelper.LineItems.Select(x => x.Code).Contains(code);
        }

        public virtual void UpdateLineItemSku(string oldCode, string newCode, decimal quantity)
        {
            //merge same sku's
            var newLineItem = this.CartHelper.Cart.GetLineItem(newCode);
            if (newLineItem != null)
            {
                newLineItem.Quantity += quantity;
                this.RemoveLineItem(oldCode);
                newLineItem.AcceptChanges();
                this.ValidateCart();
                return;
            }

            var lineItem = this.CartHelper.Cart.GetLineItem(oldCode);
            var entry = CatalogContext.Current.GetCatalogEntry(newCode, 
                new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.Variations));
            
            lineItem.Code = entry.ID;
            lineItem.MaxQuantity = entry.ItemAttributes.MaxQuantity;
            lineItem.MinQuantity = entry.ItemAttributes.MinQuantity;
            lineItem.InventoryStatus = (int)entry.InventoryStatus;

            var price = this._pricingService.GetCurrentPrice(newCode);
            lineItem.ListPrice = price.Amount;
            lineItem.PlacedPrice = price.Amount;

            this.ValidateCart();
            lineItem.AcceptChanges();
        }

        public virtual void ChangeQuantity(string code, decimal quantity)
        {
            if (quantity == 0)
            {
                this.RemoveLineItem(code);
            }
            var lineItem = this.CartHelper.Cart.GetLineItem(code);
            if (lineItem != null)
            {
                lineItem.Quantity = quantity;
                this.ValidateCart();
                this.AcceptChanges();
            }
        }

        public virtual void RemoveLineItem(string code)
        {
            var lineItem = this.CartHelper.Cart.GetLineItem(code);
            if (lineItem != null)
            {
                PurchaseOrderManager.RemoveLineItemFromOrder(this.CartHelper.Cart, lineItem.LineItemId);
                this.ValidateCart();
                this.AcceptChanges();
            }
        }

        private void ValidateCart()
        {
            string warningMessage = null;
            this.ValidateCart(out warningMessage);
        }

        private void ValidateCart(out string warningMessage)
        {
            if (this._cartName == CartHelper.WishListName)
            {
                warningMessage = null;
                return;
            }

            var workflowResult = OrderGroupWorkflowManager.RunWorkflow(this.CartHelper.Cart, OrderGroupWorkflowManager.CartValidateWorkflowName);
            var warnings = OrderGroupWorkflowManager.GetWarningsFromWorkflowResult(workflowResult).ToArray();
            warningMessage = warnings.Any() ? String.Join(" ", warnings) : null;
        }

        public virtual Money ConvertToMoney(decimal amount)
        {
            return new Money(amount, new Currency(this.CartHelper.Cart.BillingCurrency));
        }

        public virtual Money GetSubTotal()
        {
            decimal amount = this.CartHelper.Cart.SubTotal + this.CartHelper.Cart.OrderForms.SelectMany(x => x.Discounts.Cast<OrderFormDiscount>()).Sum(x => x.DiscountAmount);

            return this.ConvertToMoney(amount);
        }

        public virtual Money GetShippingSubTotal()
        {
            decimal shippingTotal = this.CartHelper.Cart.OrderForms.SelectMany(x => x.Shipments).Sum(x => x.ShippingSubTotal);
        
            return this.ConvertToMoney(shippingTotal);
        }

        public virtual Money GetShippingTotal()
        {
            return this.ConvertToMoney(this.CartHelper.Cart.ShippingTotal);
        }

        public virtual Money GetTaxTotal()
        {
            return this.ConvertToMoney(this.CartHelper.Cart.TaxTotal);
        }

        public Money GetShippingTaxTotal()
        {
            decimal amount = this.CartHelper.Cart.ShippingTotal + this.CartHelper.Cart.TaxTotal;

            return this.ConvertToMoney(amount);
        }

        public virtual Money GetOrderDiscountTotal()
        {
            decimal amount = this.GetOrderForms().SelectMany(x => x.Discounts.Cast<OrderFormDiscount>()).Sum(x => x.DiscountValue);

            return this.ConvertToMoney(amount);
        }

        public virtual Money GetShippingDiscountTotal()
        {
            decimal amount = this.GetOrderForms().SelectMany(x => x.Shipments).SelectMany(x => x.Discounts.Cast<ShipmentDiscount>()).Sum(x => x.DiscountValue);

            return this.ConvertToMoney(amount);
        }

        public virtual IEnumerable<OrderForm> GetOrderForms()
        {
            return this.CartHelper.Cart.OrderForms.Count == 0 ? new[] { new OrderForm() } : this.CartHelper.Cart.OrderForms.ToArray();
        }

        public virtual void RunWorkflow(string workFlowName)
        {
            if (this._cartName == Mediachase.Commerce.Website.Helpers.CartHelper.WishListName)
            {
                throw new ArgumentException("Running workflows are not supported for wishlist carts.");
            }

            this.CartHelper.RunWorkflow(workFlowName);
        }

        public virtual void SaveCart()
        {
            this.AcceptChanges();
        }

        public virtual void DeleteCart()
        {
            this.CartHelper.Cart.Delete();
            this.CartHelper.Cart.AcceptChanges();
        }

        private void AcceptChanges()
        {
            this.CartHelper.Cart.AcceptChanges();
        }

        private CartHelper CartHelper
        {
            get { return this._cartHelper(this._cartName); }
        }
    }
}