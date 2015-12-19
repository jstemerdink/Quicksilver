using Mediachase.Commerce.Orders;
using System.Collections.Generic;

using EPiServer.Reference.Commerce.Domain.Contracts.Models;
using EPiServer.Reference.Commerce.Domain.Models;

namespace EPiServer.Reference.Commerce.Site.Features.OrderHistory.Models
{
    public class Order
    {
        public PurchaseOrder PurchaseOrder { get; set; }
        public IEnumerable<OrderHistoryItem> Items { get; set; }
        public Address BillingAddress { get; set; }
        public IList<IAddress> ShippingAddresses { get; set; }
    }
}