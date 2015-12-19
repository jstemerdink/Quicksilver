using Mediachase.Commerce.Website;

namespace EPiServer.Reference.Commerce.Domain.Contracts.Services
{
    public interface IPaymentService
    {
        void ProcessPayment(IPaymentOption method);
    }
}
