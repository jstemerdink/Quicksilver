using System;

using EPiServer.Reference.Commerce.Domain.Contracts.Models;

using Mediachase.Commerce;

namespace EPiServer.Reference.Commerce.Domain.Models
{
    public class OrderDiscountModel : IOrderDiscountModel
    {
        public Money Discount { get; set; }
        public string Displayname { get; set; }
    }
}