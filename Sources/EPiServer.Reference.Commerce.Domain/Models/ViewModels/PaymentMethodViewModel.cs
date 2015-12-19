using System;

using EPiServer.Reference.Commerce.Domain.Contracts.Models;

using Mediachase.Commerce;
using Mediachase.Commerce.Website;

namespace EPiServer.Reference.Commerce.Domain.Models.ViewModels
{
    public class PaymentMethodViewModel<T> : IPaymentMethodViewModel<T> where T : IPaymentOption
    {
        public Guid Id { get; set; }
        public string SystemName { get; set; }
        public string FriendlyName { get; set; }
        public string Description { get; set; }
        public MarketId MarketId { get; set; }
        public int Ordering { get; set; }
        public bool IsDefault { get; set; }
        public T PaymentMethod { get; set; }
    }
}