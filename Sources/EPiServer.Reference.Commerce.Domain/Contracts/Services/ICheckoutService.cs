using System;
using System.Collections.Generic;

using EPiServer.Reference.Commerce.Domain.Contracts.Models;

using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Website;

namespace EPiServer.Reference.Commerce.Domain.Contracts.Services
{
    public interface ICheckoutService
    {
        Shipment CreateShipment();
        void UpdateShipment(Shipment shipment, ShippingRate shippingCost);
        ShippingRate GetShippingRate(Shipment shipment, Guid shippingMethodId);
        IEnumerable<ShippingRate> GetShippingRates(Shipment shipment);
        IEnumerable<IPaymentMethodViewModel<IPaymentOption>> GetPaymentMethods();
        OrderAddress AddNewOrderAddress();
        void UpdateBillingAddressId(string addressId);
        void ClearOrderAddresses();
        PurchaseOrder SaveCartAsPurchaseOrder();
        void DeleteCart();
    }
}