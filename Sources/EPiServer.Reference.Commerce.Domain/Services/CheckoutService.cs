using System;
using System.Collections.Generic;
using System.Linq;

using EPiServer.Reference.Commerce.Domain.Contracts.Models;
using EPiServer.Reference.Commerce.Domain.Contracts.Services;
using EPiServer.Reference.Commerce.Domain.Facades;
using EPiServer.Reference.Commerce.Domain.Models;
using EPiServer.Reference.Commerce.Domain.Models.ViewModels;
using EPiServer.ServiceLocation;

using Mediachase.Commerce;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Commerce.Website;
using Mediachase.Commerce.Website.Helpers;
using Mediachase.MetaDataPlus;

namespace EPiServer.Reference.Commerce.Domain.Services
{
    public abstract class CheckoutService : ICheckoutService
    {
        protected readonly Func<string, CartHelper> _cartHelper;
        protected readonly ICurrentMarket _currentMarket;
        protected readonly LanguageService _languageService;
        protected readonly CountryManagerFacade _countryManager;

        protected CheckoutService(
            Func<string, CartHelper> cartHelper, 
            ICurrentMarket currentMarket, 
            LanguageService languageService, 
            CountryManagerFacade countryManager)
        {
            this._cartHelper = cartHelper;
            this._currentMarket = currentMarket;
            this._languageService = languageService;
            this._countryManager = countryManager;
        }

        public virtual Shipment CreateShipment()
        {
            if (this.CartHelper.Cart.ObjectState == MetaObjectState.Added)
            {
                this.CartHelper.Cart.AcceptChanges();
            }

            var orderForms = this.CartHelper.Cart.OrderForms;
            if (orderForms.Count == 0)
            {
                orderForms.AddNew().AcceptChanges();
                orderForms.Single().Name = this.CartHelper.Cart.Name;
            }

            var orderForm = orderForms.First();

            var shipments = orderForm.Shipments;
            if (shipments.Count != 0)
            {
                shipments.Clear();
            }

            var shipment = shipments.AddNew();
            for (var i = 0; i < orderForm.LineItems.Count; i++)
            {
                var item = orderForm.LineItems[i];
                shipment.AddLineItemIndex(i, item.Quantity);
            }
            shipment.AcceptChanges();

            return shipment;
        }

        public virtual void UpdateShipment(Shipment shipment, ShippingRate shippingCost)
        {
            if (shipment == null)
            {
                return;
            }

            if (shippingCost == null)
            {
                return;
            }

            shipment.ShippingMethodId = shippingCost.Id;
            shipment.ShippingMethodName = shippingCost.Name;
            shipment.SubTotal = shippingCost.Money.Amount;
            shipment.ShippingSubTotal = shippingCost.Money.Amount;
            shipment.AcceptChanges();
        }

        public virtual ShippingRate GetShippingRate(Shipment shipment, Guid shippingMethodId)
        {
            var method = ShippingManager.GetShippingMethod(shippingMethodId).ShippingMethod.Single();
            return this.GetRate(shipment, method);
        }

        private ShippingRate GetRate(Shipment shipment, ShippingMethodDto.ShippingMethodRow shippingMethodRow)
        {
            var type = Type.GetType(shippingMethodRow.ShippingOptionRow.ClassName);
            var shippingGateway = (IShippingGateway)Activator.CreateInstance(type, this._currentMarket.GetCurrentMarket());
            string message = null;
            return shippingGateway.GetRate(shippingMethodRow.ShippingMethodId, shipment, ref message);
        }

        public virtual IEnumerable<ShippingRate> GetShippingRates(Shipment shipment)
        {
            var methods = ShippingManager.GetShippingMethodsByMarket(this.CurrentMarketId.Value, false).ShippingMethod;
            var currentLanguage = this.CurrentLanguageIsoCode;
            var currencyId = this.CartHelper.Cart.BillingCurrency;
            return methods.
                Where(shippingMethodRow =>
                    currentLanguage.Equals(shippingMethodRow.LanguageId, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(currencyId, shippingMethodRow.Currency, StringComparison.OrdinalIgnoreCase)).
                OrderBy(shippingMethodRow => shippingMethodRow.Ordering).
                Select(shippingMethodRow => this.GetRate(shipment, shippingMethodRow));
        }

        public virtual IEnumerable<IPaymentMethodViewModel<IPaymentOption>> GetPaymentMethods()
        {
            var methods = PaymentManager.GetPaymentMethodsByMarket(this.CurrentMarketId.Value).PaymentMethod.Where(c => c.IsActive);
            var currentLanguage = this.CurrentLanguageIsoCode;
            return methods.
                Where(paymentRow => currentLanguage.Equals(paymentRow.LanguageId, StringComparison.OrdinalIgnoreCase)).
                OrderBy(paymentRow => paymentRow.Ordering).
                Select(paymentRow => new PaymentMethodViewModel<IPaymentOption>
                {
                    Id = paymentRow.PaymentMethodId,
                    SystemName = paymentRow.SystemKeyword,
                    FriendlyName = paymentRow.Name,
                    MarketId = this.CurrentMarketId,
                    Ordering = paymentRow.Ordering,
                    IsDefault = paymentRow.IsDefault,
                    Description = paymentRow.Description,
                }).ToList();
        }

        public virtual void DeleteCart()
        {
            var cart = this.CartHelper.Cart;
            foreach (OrderForm orderForm in cart.OrderForms)
            {
                foreach (Shipment shipment in orderForm.Shipments)
                {
                    shipment.Delete();
                }
                orderForm.Delete();
            }
            foreach (OrderAddress address in cart.OrderAddresses)
            {
                address.Delete();
            }
            
            this.CartHelper.Delete();

            cart.AcceptChanges();
        }

        private CartHelper CartHelper
        {
            get { return this._cartHelper(Cart.DefaultName); }
        }

        private MarketId CurrentMarketId
        {
            get { return this._currentMarket.GetCurrentMarket().MarketId; }
        }

        private string CurrentLanguageIsoCode
        {
            get { return this._languageService.GetCurrentLanguage().TwoLetterISOLanguageName; }
        }

        public virtual OrderAddress AddNewOrderAddress()
        {
            return  this.CartHelper.Cart.OrderAddresses.AddNew();
        }

        public virtual void UpdateBillingAddressId(string addressId)
        {
            this.CartHelper.Cart.OrderForms[0].BillingAddressId = addressId;
            this.CartHelper.Cart.OrderForms[0].AcceptChanges();
        }

        public virtual void ClearOrderAddresses()
        {
            this.CartHelper.Cart.OrderAddresses.Clear();
        }

        public virtual PurchaseOrder SaveCartAsPurchaseOrder()
        {
            return this.CartHelper.Cart.SaveAsPurchaseOrder();
        }
    }
}