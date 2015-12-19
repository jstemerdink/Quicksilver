using System;
using System.Collections.Generic;

using EPiServer.Reference.Commerce.Domain.Contracts.Models;

using Mediachase.Commerce.Customers;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;

namespace EPiServer.Reference.Commerce.Domain.Contracts.Services
{
    public interface IAddressBookService
    {
        IEnumerable<IAddress> GetAddressBook();
        bool CanSave(IAddress address);
        void Save(IAddress viewModel);
        void Delete(Guid addressId);
        void SetPreferredBillingAddress(Guid addressId);
        void SetPreferredShippingAddress(Guid addressId);
        void LoadAddress(IAddress address);
        void GetCountriesAndRegionsForAddress(IAddress address);
        IEnumerable<CountryDto.StateProvinceRow> GetRegionOptionsByCountryCode(string countryCode);
        void MapModelToCustomerAddress(IAddress viewModel, CustomerAddress customerAddress);
        void MapModelToOrderAddress(IAddress viewModel, OrderAddress orderAddress);
        void MapOrderAddressToModel(IAddress viewModel, OrderAddress orderAddress);
        void MapCustomerAddressToModel(IAddress address, CustomerAddress customerAddress);
    }
}
