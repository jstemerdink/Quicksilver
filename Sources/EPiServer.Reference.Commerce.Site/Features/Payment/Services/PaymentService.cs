using EPiServer.Framework.Localization;
using EPiServer.Reference.Commerce.Site.Features.Payment.Exceptions;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Website;
using Mediachase.Commerce.Website.Helpers;
using System;
using System.Linq;

using EPiServer.Reference.Commerce.Domain.Contracts.Services;

namespace EPiServer.Reference.Commerce.Site.Features.Payment.Services
{
    [ServiceConfiguration(typeof(IPaymentService), Lifecycle = ServiceInstanceScope.Singleton)]
    public class PaymentService : Domain.Services.PaymentService
    {
        public PaymentService(Func<string, CartHelper> cartHelper, LocalizationService localizationService)
            : base(cartHelper, localizationService)
        {
        }
    }
}