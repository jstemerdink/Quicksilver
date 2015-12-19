using Mediachase.Commerce.Orders;

namespace EPiServer.Reference.Commerce.Domain.Contracts.Services
{
    public interface IConfirmationService
    {
        PurchaseOrder GetOrder(int orderNumber);
    }
}