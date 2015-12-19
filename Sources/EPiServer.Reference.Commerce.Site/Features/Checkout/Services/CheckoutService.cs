using EPiServer.Reference.Commerce.Site.Features.Payment.Models;
using EPiServer.ServiceLocation;
using Mediachase.Commerce;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Commerce.Website;
using Mediachase.Commerce.Website.Helpers;
using Mediachase.MetaDataPlus;
using System;
using System.Collections.Generic;
using System.Linq;

using EPiServer.Reference.Commerce.Domain.Contracts.Models;
using EPiServer.Reference.Commerce.Domain.Contracts.Services;
using EPiServer.Reference.Commerce.Domain.Facades;
using EPiServer.Reference.Commerce.Domain.Models.ViewModels;
using EPiServer.Reference.Commerce.Domain.Services;
using EPiServer.Reference.Commerce.Site.Infrastructure.Facades;
using EPiServer.Reference.Commerce.Site.Features.Market.Services;

namespace EPiServer.Reference.Commerce.Site.Features.Checkout.Services
{
    [ServiceConfiguration(typeof(ICheckoutService), Lifecycle = ServiceInstanceScope.Transient)]
    public class CheckoutService : Domain.Services.CheckoutService
    {
        public CheckoutService(Func<string, CartHelper> cartHelper, ICurrentMarket currentMarket, LanguageService languageService, CountryManagerFacade countryManager)
            : base(cartHelper, currentMarket, languageService, countryManager)
        {
        }
    }
}