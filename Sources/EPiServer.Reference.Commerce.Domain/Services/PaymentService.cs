using System;
using System.Linq;

using EPiServer.Framework.Localization;
using EPiServer.Reference.Commerce.Domain.Contracts.Services;
using EPiServer.Reference.Commerce.Domain.Models;
using EPiServer.ServiceLocation;

using Mediachase.Commerce.Website;
using Mediachase.Commerce.Website.Helpers;

namespace EPiServer.Reference.Commerce.Domain.Services
{
    public abstract class PaymentService : IPaymentService
    {
        protected readonly Func<string, CartHelper> _cartHelper;
        protected readonly LocalizationService _localizationService;

        protected PaymentService(Func<string, CartHelper> cartHelper, LocalizationService localizationService)
        {
            this._cartHelper = cartHelper;
            this._localizationService = localizationService;
        }

        public virtual void ProcessPayment(IPaymentOption method)
        {
            if (method == null)
            {
                return;
            }

            var cart = this._cartHelper(Mediachase.Commerce.Orders.Cart.DefaultName).Cart;

            if (!cart.OrderForms.Any())
            {
                cart.OrderForms.AddNew();
            }

            var payment = method.PreProcess(cart.OrderForms[0]);

            if (payment == null)
            {
                throw new PreProcessException();
            }

            cart.OrderForms[0].Payments.Add(payment);
            cart.AcceptChanges();

            method.PostProcess(cart.OrderForms[0]);
        }
    }
}