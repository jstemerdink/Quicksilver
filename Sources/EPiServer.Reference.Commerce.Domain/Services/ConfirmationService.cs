using System.Collections.Specialized;
using System.Linq;

using EPiServer.Reference.Commerce.Domain.Contracts.Services;

using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Search;

namespace EPiServer.Reference.Commerce.Domain.Services
{
    public class ConfirmationService : IConfirmationService
    {
        public virtual PurchaseOrder GetOrder(int orderNumber)
        {
            return OrderContext.Current.GetPurchaseOrder(orderNumber);
        }
    }
}