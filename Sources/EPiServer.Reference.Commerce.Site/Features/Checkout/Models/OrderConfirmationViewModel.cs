using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Registration.Models;
using Mediachase.Commerce;
using Mediachase.Commerce.Orders;
using System;
using System.Collections.Generic;

using EPiServer.Reference.Commerce.Domain.Contracts.Models;
using EPiServer.Reference.Commerce.Domain.Models;
using EPiServer.Reference.Commerce.Domain.Models.ViewModels;

namespace EPiServer.Reference.Commerce.Site.Features.Checkout.Models
{
    public class OrderConfirmationViewModel<T> : PageViewModel<T> where T : PageData
    {
        public bool HasOrder { get; set; }
        public string OrderId { get; set; }
        public OrderConfirmationRegistrationFormModel RegistrationFormModel { get; set; }
        public IEnumerable<LineItem> Items { get; set; }
        public Address BillingAddress { get; set; }
        public IList<IAddress> ShippingAddresses { get; set; }
        public IEnumerable<Mediachase.Commerce.Orders.Payment> Payments { get; set; }
        public Guid ContactId { get; set; }
        public DateTime Created { get; set; }
        public int GroupId { get; set; }
        public string NotificationMessage { get; set; }
        public Dictionary<int, decimal> ItemPrices { get; set; }

        public Money HandlingTotal { get; set; }
        public Money ShippingSubTotal { get; set; }
        public Money ShippingDiscountTotal { get; set; }
        public Money ShippingTotal { get; set; }
        public Money TaxTotal { get; set; }
        public Money CartTotal { get; set; }
        public Money OrderLevelDiscountTotal { get; set; }
    }
}