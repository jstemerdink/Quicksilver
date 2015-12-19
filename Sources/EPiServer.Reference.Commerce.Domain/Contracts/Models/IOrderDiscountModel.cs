using System;

using Mediachase.Commerce;

namespace EPiServer.Reference.Commerce.Domain.Contracts.Models
{
    public interface IOrderDiscountModel
    {
        Money Discount { get; set; }

        String Displayname { get; set; }
    }
}