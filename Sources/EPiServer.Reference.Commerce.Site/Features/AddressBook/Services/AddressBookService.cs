using EPiServer.Reference.Commerce.Site.Features.AddressBook.Pages;
using EPiServer.Reference.Commerce.Site.Features.AddressBook.ViewModels;
using EPiServer.Reference.Commerce.Site.Infrastructure.Facades;
using EPiServer.ServiceLocation;
using Mediachase.BusinessFoundation.Data;
using Mediachase.Commerce.Customers;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using EPiServer.Reference.Commerce.Domain.Contracts.Services;
using EPiServer.Reference.Commerce.Domain.Facades;
using EPiServer.Reference.Commerce.Domain.Models;

namespace EPiServer.Reference.Commerce.Site.Features.AddressBook.Services
{
    [ServiceConfiguration(typeof(IAddressBookService), Lifecycle = ServiceInstanceScope.Singleton)]
    public class AddressBookService : Domain.Services.AddressBookService
    {
        public AddressBookService(CustomerContextFacade customerContext, CountryManagerFacade countryManager)
            : base(customerContext, countryManager)
        {
        }
    }
}
