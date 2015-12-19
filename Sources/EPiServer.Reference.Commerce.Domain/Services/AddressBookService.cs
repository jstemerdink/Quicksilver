using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using EPiServer.Reference.Commerce.Domain.Contracts.Models;
using EPiServer.Reference.Commerce.Domain.Contracts.Services;
using EPiServer.Reference.Commerce.Domain.Facades;
using EPiServer.Reference.Commerce.Domain.Models;

using Mediachase.BusinessFoundation.Data;
using Mediachase.Commerce.Customers;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;

namespace EPiServer.Reference.Commerce.Domain.Services
{
    public abstract class AddressBookService : IAddressBookService
    {
        protected static readonly IEnumerable<CountryDto.StateProvinceRow> _emptyRegionList = Enumerable.Empty<CountryDto.StateProvinceRow>();
        protected readonly CustomerContextFacade _customercontext;
        protected readonly CountryManagerFacade _countryManager;

        protected AddressBookService(CustomerContextFacade customerContext, CountryManagerFacade countryManager)
        {
            this._customercontext = customerContext;
            this._countryManager = countryManager;
        }

        public virtual void MapCustomerAddressToModel(IAddress address, CustomerAddress customerAddress)
        {
            if (address == null)
            {
                return;
            }

            if (customerAddress == null)
            {
                return;
            }

            address.Line1 = customerAddress.Line1;
            address.Line2 = customerAddress.Line2;
            address.City = customerAddress.City;
            address.CountryName = customerAddress.CountryName;
            address.CountryCode = customerAddress.CountryCode;
            address.Email = customerAddress.Email;
            address.FirstName = customerAddress.FirstName;
            address.LastName = customerAddress.LastName;
            address.PostalCode = customerAddress.PostalCode;
            address.SaveAddress = HttpContext.Current.User.Identity.IsAuthenticated;
            address.Region = customerAddress.RegionName ?? customerAddress.State;
            address.ShippingDefault = customerAddress.Equals(this._customercontext.CurrentContact.Service.PreferredShippingAddress);
            address.BillingDefault = customerAddress.Equals(this._customercontext.CurrentContact.Service.PreferredBillingAddress);
            address.AddressId = customerAddress.AddressId;
            address.Modified = customerAddress.Modified;
            address.Name = customerAddress.Name;
            address.DaytimePhoneNumber = customerAddress.DaytimePhoneNumber;
        }

        public virtual void MapOrderAddressToModel(IAddress address, OrderAddress orderAddress)
        {
            if (address == null)
            {
                return;
            }

            if (orderAddress == null)
            {
                return;
            }

            address.Line1 = orderAddress.Line1;
            address.Line2 = orderAddress.Line2;
            address.City = orderAddress.City;
            address.CountryName = orderAddress.CountryName;
            address.CountryCode = orderAddress.CountryCode;
            address.Email = orderAddress.Email;
            address.FirstName = orderAddress.FirstName;
            address.LastName = orderAddress.LastName;
            address.PostalCode = orderAddress.PostalCode;
            address.SaveAddress = false;
            address.Region = orderAddress.RegionName ?? orderAddress.State;
            address.Modified = orderAddress.Modified;
            address.Name = orderAddress.Name;
            address.DaytimePhoneNumber = orderAddress.DaytimePhoneNumber;
        }

        public virtual void MapModelToOrderAddress(IAddress address, OrderAddress orderAddress)
        {
            if (address == null)
            {
                return;
            }

            if (orderAddress == null)
            {
                return;
            }

            orderAddress.City = address.City;
            orderAddress.CountryCode = address.CountryCode;
            orderAddress.CountryName = this.GetAllCountries().Where(x => x.Code == address.CountryCode).Select(x => x.Name).FirstOrDefault();
            orderAddress.FirstName = address.FirstName;
            orderAddress.LastName = address.LastName;
            orderAddress.Line1 = address.Line1;
            orderAddress.Line2 = address.Line2;
            orderAddress.DaytimePhoneNumber = address.DaytimePhoneNumber;
            orderAddress.PostalCode = address.PostalCode;
            orderAddress.RegionName = address.Region;
            // Commerce Manager expects State to be set for addresses in order management. Set it to be same as
            // RegionName to avoid issues.
            orderAddress.State = address.Region;
            orderAddress.Email = address.Email;
        }

        public virtual void MapModelToCustomerAddress(IAddress address, CustomerAddress customerAddress)
        {
            if (address == null)
            {
                return;
            }

            if (customerAddress == null)
            {
                return;
            }

            customerAddress.Name = address.Name;
            customerAddress.City = address.City;
            customerAddress.CountryCode = address.CountryCode;
            customerAddress.CountryName = this.GetAllCountries().Where(x => x.Code == address.CountryCode).Select(x => x.Name).FirstOrDefault();
            customerAddress.FirstName = address.FirstName;
            customerAddress.LastName = address.LastName;
            customerAddress.Line1 = address.Line1;
            customerAddress.Line2 = address.Line2;
            customerAddress.DaytimePhoneNumber = address.DaytimePhoneNumber;
            customerAddress.PostalCode = address.PostalCode;
            customerAddress.RegionName = address.Region;
            // Commerce Manager expects State to be set for addresses in order management. Set it to be same as
            // RegionName to avoid issues.
            customerAddress.State = address.Region;
            customerAddress.Email = address.Email;
            customerAddress.AddressType =
                CustomerAddressTypeEnum.Public |
                (address.ShippingDefault ? CustomerAddressTypeEnum.Shipping : 0) |
                (address.BillingDefault ? CustomerAddressTypeEnum.Billing : 0);
        }

        //public AddressCollectionViewModel GetAddressBookViewModel(AddressBookPage addressBookPage)
        public virtual IEnumerable<IAddress> GetAddressBook()
        {
            return  this._customercontext.CurrentContact.Service.ContactAddresses.Select(this.ConvertAddress);
        }

        public virtual bool CanSave(IAddress address)
        {
            return !this._customercontext.CurrentContact.Service.ContactAddresses.Any(x =>
                x.Name.Equals(address.Name, StringComparison.InvariantCultureIgnoreCase) &&
                x.AddressId != address.AddressId);
        }

        public virtual void Save(IAddress address)
        {
            var currentContact = this._customercontext.CurrentContact.Service;
            var customerAddress = this.CreateOrUpdateCustomerAddress(currentContact, address);

            if (address.BillingDefault)
            {
                currentContact.PreferredBillingAddress = customerAddress;
            }
            else if (currentContact.PreferredBillingAddressId == address.AddressId)
            {
                currentContact.PreferredBillingAddressId = null;
            }

            if (address.ShippingDefault)
            {
                currentContact.PreferredShippingAddress = customerAddress;
            }
            else if (currentContact.PreferredShippingAddressId == address.AddressId)
            {
                currentContact.PreferredShippingAddressId = null;
            }

            currentContact.SaveChanges();
        }

        public virtual void Delete(Guid addressId)
        {
            var currentContact = this._customercontext.CurrentContact.Service;
            var customerAddress = this.GetAddress(currentContact, addressId);
            if (customerAddress == null)
            {
                return;
            }
            if (currentContact.PreferredBillingAddressId == customerAddress.PrimaryKeyId || currentContact.PreferredShippingAddressId == customerAddress.PrimaryKeyId)
            {
                currentContact.PreferredBillingAddressId = currentContact.PreferredBillingAddressId == customerAddress.PrimaryKeyId ? null : currentContact.PreferredBillingAddressId;
                currentContact.PreferredShippingAddressId = currentContact.PreferredShippingAddressId == customerAddress.PrimaryKeyId ? null : currentContact.PreferredShippingAddressId;
                currentContact.SaveChanges();
            }
            currentContact.DeleteContactAddress(customerAddress);
            currentContact.SaveChanges();
        }

        public virtual void SetPreferredBillingAddress(Guid addressId)
        {
            var currentContact = this._customercontext.CurrentContact.Service;
            var customerAddress = this.GetAddress(currentContact, addressId);
            if (customerAddress == null)
            {
                return;
            }
            currentContact.PreferredBillingAddress = customerAddress;
            currentContact.SaveChanges();
        }

        public virtual void SetPreferredShippingAddress(Guid addressId)
        {
            var currentContact = this._customercontext.CurrentContact.Service;
            var customerAddress = this.GetAddress(currentContact, addressId);
            if (customerAddress == null)
            {
                return;
            }
            currentContact.PreferredShippingAddress = customerAddress;
            currentContact.SaveChanges();
        }

        public virtual void LoadAddress(IAddress address)
        {
            var currentContact = this._customercontext.CurrentContact.Service;

            address.CountryOptions = this.GetAllCountries();

            if (address.CountryCode == null && address.CountryOptions.Any())
            {
                address.CountryCode = address.CountryOptions.First().Code;
            }

            if (address.AddressId.HasValue)
            {
                var existingCustomerAddress = this.GetAddress(currentContact, address.AddressId);

                if (existingCustomerAddress == null)
                {
                    throw new ArgumentException(string.Format("The address id {0} could not be found.", address.AddressId.Value));
                }

                this.MapCustomerAddressToModel(address, existingCustomerAddress);
            }

            if (!string.IsNullOrEmpty(address.CountryCode))
            {
                address.RegionOptions = this.GetRegionOptionsByCountryCode(address.CountryCode);
            }
        }

        public virtual IEnumerable<CountryDto.StateProvinceRow> GetRegionOptionsByCountryCode(string countryCode)
        {
            CountryDto.CountryRow country = this._countryManager.GetCountryByCountryCode(countryCode);
            if (country != null)
            {
                return this.GetRegionOptionsFromCountry(country);
            }
            return Enumerable.Empty<CountryDto.StateProvinceRow>();
        }

        public virtual void GetCountriesAndRegionsForAddress(IAddress address)
        {
            address.CountryOptions = this.GetAllCountries();

            //try get country first by code, then by name, then the first in list as final fallback
            var selectedCountry = (this.GetCountryByCode(address) ?? 
                                   this.GetCountryByName(address)) ??
                                   address.CountryOptions.FirstOrDefault();

            address.RegionOptions = this.GetRegionOptionsFromCountry(selectedCountry);
        }

        public virtual CountryDto.CountryRow GetCountryByCode(IAddress address)
        {
            var selectedCountry = address.CountryOptions.FirstOrDefault(x => x.Code == address.CountryCode);
            if (selectedCountry != null)
            {
                address.CountryName = selectedCountry.Name;
            }
            return selectedCountry;
        }

        public virtual CountryDto.CountryRow GetCountryByName(IAddress address)
        {
            var selectedCountry = address.CountryOptions.FirstOrDefault(x => x.Name == address.CountryName);
            if (selectedCountry != null)
            {
                address.CountryCode = selectedCountry.Code;
            }
            return selectedCountry;
        }

        public virtual IEnumerable<CountryDto.StateProvinceRow> GetRegionOptionsFromCountry(CountryDto.CountryRow country)
        {
            if (country == null)
            {
                return _emptyRegionList;
            }
            return country.GetStateProvinceRows().ToList();
        }

        private CustomerAddress CreateOrUpdateCustomerAddress(CurrentContactFacade contact, IAddress address)
        {
            var customerAddress = this.GetAddress(contact, address.AddressId);
            var isNew = customerAddress == null;
            IEnumerable<PrimaryKeyId> existingId = contact.ContactAddresses.Select(a => a.AddressId).ToList();
            if (isNew)
            {
                customerAddress = CustomerAddress.CreateInstance();
            }

            this.MapModelToCustomerAddress(address, customerAddress);

            if (isNew)
            {
                contact.AddContactAddress(customerAddress);
            }
            else
            {
                contact.UpdateContactAddress(customerAddress);
            }

            contact.SaveChanges();
            if (isNew)
            {
                customerAddress.AddressId = contact.ContactAddresses
                    .Where(a => !existingId.Contains(a.AddressId))
                    .Select(a => a.AddressId)
                    .Single();
                address.AddressId = customerAddress.AddressId;
            }
            return customerAddress;
        }

        private IAddress ConvertAddress(CustomerAddress customerAddress)
        {
            IAddress address = null;

            if (customerAddress != null)
            {
                address = new Address();
                this.MapCustomerAddressToModel(address, customerAddress);
            }

            return address;
        }

        public virtual CustomerAddress GetAddress(CurrentContactFacade contact, Guid? addressId)
        {
            return addressId.HasValue ?
                contact.ContactAddresses.FirstOrDefault(x => x.AddressId == addressId.GetValueOrDefault()) :
                null;
        }

        public virtual List<CountryDto.CountryRow> GetAllCountries()
        {
            return this._countryManager.GetCountries().Country.ToList();
        }
    }
}
