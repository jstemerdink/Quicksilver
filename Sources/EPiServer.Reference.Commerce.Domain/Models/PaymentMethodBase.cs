using System;

using EPiServer.Framework.Localization;

namespace EPiServer.Reference.Commerce.Domain.Models
{
    public abstract class PaymentMethodBase
    {
        protected readonly LocalizationService _localizationService;

        protected PaymentMethodBase(LocalizationService localizationService)
        {
            this._localizationService = localizationService;
        }

        public Guid PaymentMethodId { get; set; }

    }
}