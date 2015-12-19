using System;

using Mediachase.Commerce;
using Mediachase.Commerce.Website;

namespace EPiServer.Reference.Commerce.Domain.Contracts.Models
{
    public interface IPaymentMethodViewModel<out T> where T : IPaymentOption
    {
        Guid Id { get; set; }
        T PaymentMethod { get; }
        string Description { get; set; }
        string SystemName { get; set; }

       
        string FriendlyName { get; set; }
        MarketId MarketId { get; set; }
        int Ordering { get; set; }
        bool IsDefault { get; set; }
       
    }
}
