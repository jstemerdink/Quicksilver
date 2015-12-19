using Mediachase.Commerce;
using Mediachase.Commerce.Orders;

namespace EPiServer.Reference.Commerce.Extensions
{
    public static class OrderGroupExtensions
    {
        public static Money ToMoney(this OrderGroup orderGroup, decimal amount)
        {
            return new Money(amount, orderGroup.BillingCurrency);
        }
    }
}